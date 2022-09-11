using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using HostVersion.Commands.Notifs;
using HostVersion.Commands.Pushes;
using HostVersion.Commands.Requests.Answers;
using HostVersion.Commands.Requests.App;
using HostVersion.Commands.Requests.Auth;
using HostVersion.Commands.Requests.Bot;
using HostVersion.Commands.Requests.Complex;
using HostVersion.Commands.Requests.Contact;
using HostVersion.Commands.Requests.File;
using HostVersion.Commands.Requests.Internal;
using HostVersion.Commands.Requests.Invite;
using HostVersion.Commands.Requests.Message;
using HostVersion.Commands.Requests.Module;
using HostVersion.Commands.Requests.Pulse;
using HostVersion.Commands.Requests.Questions;
using HostVersion.Commands.Requests.Room;
using HostVersion.Commands.Requests.User;
using HostVersion.DbContexts;
using HostVersion.Entities;
using HostVersion.Middles;
using HostVersion.Notifications;
using HostVersion.SharedArea;
using HostVersion.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Version = HostVersion.Entities.Version;
using File = System.IO.File;

namespace HostVersion
{
    public class QuestionConsumer : 
        IResponderAsync<RegisterRequest, RegisterResponse>,
        IResponderAsync<VerifyRequest, VerifyResponse>,
        IResponder<DeleteAccountRequest, DeleteAccountResponse>,
        IResponder<LogoutRequest, LogoutResponse>,
        IResponder<UpdateUserProfileRequest, UpdateUserProfileResponse>,
        IResponder<UpdateComplexProfileRequest, UpdateComplexProfileResponse>,
        IResponder<CreateComplexRequest, CreateComplexResponse>,
        IResponder<DeleteComplexRequest, DeleteComplexResponse>,
        IResponder<UpdateRoomProfileRequest, UpdateRoomProfileResponse>,
        IResponder<DeleteRoomRequest, DeleteRoomResponse>,
        IResponder<CreateContactRequest, CreateContactResponse>,
        IResponderAsync<CreateInviteRequest, CreateInviteResponse>,
        IResponder<CancelInviteRequest, CancelInviteResponse>,
        IResponder<AcceptInviteRequest, AcceptInviteResponse>,
        IResponder<IgnoreInviteRequest, IgnoreInviteResponse>,
        IResponderAsync<CreateRoomRequest, CreateRoomResponse>,
        IResponder<GetMyInvitesRequest, GetMyInvitesResponse>,
        IResponder<UpdateMemberAccessRequest, UpdateMemberAccessResponse>,
        IResponder<GetComplexAccessesRequest, GetComplexAccessesResponse>,
        IResponder<GetMeRequest, GetMeResponse>,
        IResponder<GetUserByIdRequest, GetUserByIdResponse>,
        IResponder<GetComplexByIdRequest, GetComplexByIdResponse>,
        IResponder<GetRoomByIdRequest, GetRoomByIdResponse>,
        IResponder<GetRoomsRequest, GetRoomsResponse>, 
        IResponder<GetComplexesRequest, GetComplexesResponse>,
        IResponder<GetContactsRequest, GetContactsResponse>,
        IResponderAsync<CreateAppRequest, CreateAppResponse>,
        IResponder<UpdateBotProfileRequest, UpdateBotProfileResponse>,
        IResponder<GetBotRequest, GetBotResponse>,
        IResponderAsync<CreateBotRequest, CreateBotResponse>,
        IResponderAsync<SubscribeBotRequest, SubscribeBotResponse>,
        IResponder<SearchBotsRequest, SearchBotsResponse>,
        IResponder<GetSubscribedBotsRequest, GetSubscribedBotsResponse>,
        IResponder<GetCreatedBotsRequest, GetCreatedBotsResponse>,
        IResponder<GetBotsRequest, GetBotsResponse>,
        IResponder<GetComplexWorkersRequest, GetComplexWorkersResponse>,
        IResponderAsync<AddBotToRoomRequest, AddBotToRoomResponse>,
        IResponder<UpdateWorkershipRequest, UpdateWorkershipResponse>,
        IResponder<RemoveBotFromRoomRequest, RemoveBotFromRoomResponse>,
        IResponder<GetWorkershipsRequest, GetWorkershipsResponse>,
        IResponder<GetBotStoreContentRequest, GetBotStoreContentResponse>,
        IResponderAsync<BotGetWorkershipsRequest, BotGetWorkershipsResponse>,
        IResponder<SearchUsersRequest, SearchUsersResponse>,
        IResponder<SearchComplexesRequest, SearchComplexesResponse>,
        IResponder<GetFileSizeRequest, GetFileSizeResponse>,
        IResponderAsync<WriteToFileRequest, WriteToFileResponse>,
        IResponderAsync<UploadPhotoRequest, UploadPhotoResponse>,
        IResponderAsync<UploadAudioRequest, UploadAudioResponse>,
        IResponderAsync<UploadVideoRequest, UploadVideoResponse>,
        IResponderAsync<DownloadFileRequest, DownloadFileResponse>,
        IResponder<ClickBotViewRequest, ClickBotViewResponse>,
        IResponder<RequestBotViewRequest, RequestBotViewResponse>,
        IResponder<SendBotViewRequest, SendBotViewResponse>,
        IResponder<UpdateBotViewRequest, UpdateBotViewResponse>,
        IResponder<AnimateBotViewRequest, AnimateBotViewResponse>,
        IResponder<RunCommandsOnBotViewRequest, RunCommandsOnBotViewResponse>,
        IResponder<GetMessagesRequest, GetMessagesResponse>,
        IResponderAsync<CreateTextMessageRequest, CreateTextMessageResponse>,
        IResponderAsync<CreateFileMessageRequest, CreateFileMessageResponse>,
        IResponderAsync<BotCreateTextMessageRequest, BotCreateTextMessageResponse>,
        IResponderAsync<BotCreateFileMessageRequest, BotCreateFileMessageResponse>,
        IResponder<PutServiceMessageRequest, PutServiceMessageResponse>,
        IResponder<GetLastActionsRequest, GetLastActionsResponse>,
        IResponder<NotifyMessageSeenRequest, NotifyMessageSeenResponse>,
        IResponder<GetMessageSeenCountRequest, GetMessageSeenCountResponse>,
        IResponder<CreateModuleRequest, CreateModuleResponse>,
        IResponder<UpdateModuleProfileRequest, UpdateModuleProfileResponse>,
        IResponder<SearchModulesRequest, SearchModulesResponse>,
        IResponderAsync<BotAppendTextToTxtFileRequest, BotAppendTextToTxtFileResponse>,
        IResponder<BotExecuteSqlOnSqlFileRequest, BotExecuteSqlOnSqlFileResponse>,
        IResponder<BotExecuteMongoComOnMongoFileRequest, BotExecuteMongoComOnMongoFileResponse>,
        IResponderAsync<BotCreateDocumentFileRequest, BotCreateDocumentFileResponse>,
        IResponderAsync<BotPermitModuleRequest, BotPermitModuleResponse>,
        IResponder<GetModuleServerAddressRequest, GetModuleServerAddressResponse>,
        IResponderAsync<AskAddBotScreenShot, AnswerAddBotScreenShot>,
        IResponder<AskRemoveBotScreenShot, AnswerRemoveBotScreenShot>,
        IResponder<AskRequestBotPreview, AnswerRequestBotPreview>,
        IResponder<BotViewResizedRequest, BotViewResizedResponse>
    {
        private readonly string _dirPath;
        private ConcurrentDictionary<string, bool> pendingPreviews = new ConcurrentDictionary<string, bool>();
        
        public QuestionConsumer()
        {
            _dirPath = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), "FileStorage");
            Directory.CreateDirectory(_dirPath);
        }
        
        public async Task<CreateAppResponse> AnswerQuestion(AnswerContext<CreateAppRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                dbContext.Entry(user).Collection(u => u.Apps).Load();

                if (user.Apps.Count < 5)
                {
                    var kt = new KafkaTransport();
                    var appId = await kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(App).Name});
                    var app = new App()
                    {
                        AppId = (long) appId.Index, 
                        Title = packet.App.Title,
                        Creator = user,
                        Token = Security.MakeKey64()
                    };
                    dbContext.Apps.Add(app);
                    dbContext.SaveChanges();

                    return new CreateAppResponse()
                    {
                        Packet = new Packet()
                        {
                            Status = "success",
                            App = app
                        }
                    };
                }

                return new CreateAppResponse()
                {
                    Packet = new Packet()
                    {
                        Status = "error_1"
                    }
                };
            }
        }

        public GetRoomByIdResponse AnswerQuestion(AnswerContext<GetRoomByIdRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new GetRoomByIdResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                dbContext.Entry(membership.Complex).Collection(c => c.Rooms).Load();
                var room = membership.Complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new GetRoomByIdResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    };
                }

                return new GetRoomByIdResponse()
                {
                    Packet = new Packet {Status = "success", BaseRoom = room}
                };
            }
        }

        public GetComplexByIdResponse AnswerQuestion(AnswerContext<GetComplexByIdRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complex = dbContext.Complexes.Find(packet.Complex.ComplexId);
                if (complex == null)
                {
                    return new GetComplexByIdResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                if (complex.Mode == 3)
                {
                    return new GetComplexByIdResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "success",
                            Complex = dbContext.Complexes.Find(packet.Complex.ComplexId)
                        }
                    };
                }

                if (complex.Mode == 2)
                {
                    dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                    var user = (User) session.BaseUser;
                    dbContext.Entry(user).Collection(u => u.Memberships).Load();
                    var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                    if (membership == null)
                    {
                        return new GetComplexByIdResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }

                    dbContext.Entry(membership).Reference(m => m.Complex).Load();
                    return new GetComplexByIdResponse()
                    {
                        Packet = new Packet {Status = "success", Complex = membership.Complex}
                    };
                }
                else
                {
                    dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                    var user = (User) session.BaseUser;
                    dbContext.Entry(user).Reference(u => u.UserSecret).Load();
                    if (user.UserSecret.HomeId == packet.Complex.ComplexId)
                    {
                        dbContext.Entry(user.UserSecret).Reference(us => us.Home).Load();
                        return new GetComplexByIdResponse()
                        {
                            Packet = new Packet {Status = "success", Complex = user.UserSecret.Home}
                        } ;
                    }

                    return new GetComplexByIdResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }
            }
        }

        public GetUserByIdResponse AnswerQuestion(AnswerContext<GetUserByIdRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var user = dbContext.BaseUsers.Find(packet.BaseUser.BaseUserId);

                return new GetUserByIdResponse()
                {
                    Packet = new Packet {Status = "success", BaseUser = user}
                };
            }
        }

        public GetMeResponse AnswerQuestion(AnswerContext<GetMeRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Reference(u => u.UserSecret).Load();
                dbContext.Entry(user.UserSecret).Reference(us => us.Home).Load();

                return new GetMeResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success",
                        User = user,
                        UserSecret = user.UserSecret,
                        Complex = user.UserSecret.Home
                    }
                };
            }
        }

        public GetRoomsResponse AnswerQuestion(AnswerContext<GetRoomsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                if (packet.Complex.ComplexId > 0)
                {
                    var membership = user.Memberships.Find(mem => mem.ComplexId == packet.Complex.ComplexId);
                    if (membership == null)
                    {
                        return (new GetRoomsResponse()
                        {
                            Packet = new Packet {Status = "error_0B0"}
                        });
                    }

                    dbContext.Entry(membership).Reference(mem => mem.Complex).Load();
                    dbContext.Entry(membership.Complex).Collection(c => c.SingleRooms).Load();
                    dbContext.Entry(membership.Complex).Collection(c => c.Rooms).Load();
                    var bRooms = membership.Complex.Rooms.Select(r => (BaseRoom) r).ToList();
                    bRooms.AddRange(membership.Complex.SingleRooms.Where(r => r.User1Id == user.BaseUserId || r.User2Id == user.BaseUserId));
                    return (new GetRoomsResponse()
                    {
                        Packet = new Packet {Status = "success", BaseRooms = bRooms}
                    });
                }

                var rooms = new List<BaseRoom>();
                foreach (var membership in user.Memberships)
                {
                    dbContext.Entry(membership).Reference(m => m.Complex).Query().Include(c => c.Rooms).Load();
                    rooms.AddRange(membership.Complex.Rooms);
                }

                return (new GetRoomsResponse()
                {
                    Packet = new Packet {Status = "success", BaseRooms = rooms}
                });
            }
        }

        public GetComplexesResponse AnswerQuestion(AnswerContext<GetComplexesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Query()
                    .Include(m => m.Complex).Include(mem => mem.MemberAccess).Load();
                var complexSecrets = new List<ComplexSecret>();
                var complexes = new List<Complex>();
                foreach (var membership in user.Memberships)
                {
                    var complex = membership.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    dbContext.Entry(complex).Collection(c => c.SingleRooms).Query()
                        .Where(r => r.User1Id == user.BaseUserId || r.User2Id == user.BaseUserId)
                        .Load();
                    dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                    if (complex.ComplexSecret == null) continue;
                    if (complex.ComplexSecret.AdminId == user.BaseUserId)
                    {
                        complexSecrets.Add(complex.ComplexSecret);
                        dbContext.Entry(complex).Collection(c => c.Invites).Query()
                            .Include(i => i.Complex).Include(i => i.User).Load();
                    }

                    if (membership.MemberAccess.CanModifyAccess)
                        dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User)
                            .Include(m => m.MemberAccess).Load();
                    else
                        dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User).Load();
                    complexes.Add(complex);
                }

                return (new GetComplexesResponse()
                {
                    Packet = new Packet {Status = "success", Complexes = complexes, ComplexSecrets = complexSecrets}
                });
            }
        }

        public GetContactsResponse AnswerQuestion(AnswerContext<GetContactsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Contacts).Load();
                var contacts = user.Contacts;
                foreach (var contact in contacts)
                {
                    dbContext.Entry(contact).Reference(c => c.Complex).Load();
                    dbContext.Entry(contact.Complex).Collection(c => c.Rooms).Load();
                    dbContext.Entry(contact).Reference(c => c.Peer).Load();
                    dbContext.Entry(contact).Reference(c => c.User).Load();
                }

                return (new GetContactsResponse()
                {
                    Packet = new Packet {Status = "success", Contacts = contacts}
                });
            }
        }

        public UpdateUserProfileResponse AnswerQuestion(AnswerContext<UpdateUserProfileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;

                if (string.IsNullOrEmpty(packet.User.Title))
                {
                    return (new UpdateUserProfileResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    });
                }

                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = session.BaseUser;
                user.Title = packet.User.Title;
                user.Avatar = packet.User.Avatar;
                dbContext.SaveChanges();

                var tx = dbContext.Database.BeginTransaction();
                IncreaseVersion(dbContext, "BaseUserId", user.BaseUserId, "BaseUsers");
                tx.Commit();
                dbContext.SaveChanges();

                var version = dbContext.BaseUsers.Find(user.BaseUserId).Version;

                var versions = new List<Version>()
                {
                    new Version()
                    {
                        VersionId = "BaseUser_" + user.BaseUserId + "_CityService",
                        Number = version
                    }
                };

                var kt = new KafkaTransport();

                NotifyVersionsUpdated(versions);
                

                return (new UpdateUserProfileResponse()
                {
                    Packet = new Packet()
                    {
                        Status = "success",
                        Versions = versions
                    }
                });
            }
        }

        public UpdateComplexProfileResponse AnswerQuestion(AnswerContext<UpdateComplexProfileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var complex = dbContext.Complexes.Include(c => c.ComplexSecret)
                    .SingleOrDefault(c => c.ComplexId == packet.Complex.ComplexId);
                if (complex == null)
                {
                    return (new UpdateComplexProfileResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    });
                }

                if (complex.Mode != 1 && complex.ComplexSecret.AdminId == session.BaseUser.BaseUserId)
                {
                    complex.Title = packet.Complex.Title;
                    complex.Avatar = packet.Complex.Avatar;
                    dbContext.SaveChanges();

                    var tx = dbContext.Database.BeginTransaction();
                    IncreaseVersion(dbContext, "ComplexId", complex.ComplexId, "Complexes");
                    tx.Commit();
                    dbContext.SaveChanges();

                    var version = dbContext.Complexes.Find(complex.ComplexId).Version;

                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Complex_" + complex.ComplexId + "_CityService",
                            Number = version
                        }
                    };

                    NotifyVersionsUpdated(versions);

                    var kt = new KafkaTransport();
       
                    
                    return (new UpdateComplexProfileResponse()
                    {
                        Packet = new Packet {Status = "success", Complex = complex, Versions = versions}
                    });
                }

                return (new UpdateComplexProfileResponse()
                {
                    Packet = new Packet {Status = "error_0"}
                });
            }
        }

        public CreateComplexResponse AnswerQuestion(AnswerContext<CreateComplexRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                
                var kt = new KafkaTransport();
                
                if (!string.IsNullOrEmpty(packet.Complex.Title) && packet.Complex.Title.ToLower() != "home")
                {
                    dbContext.Entry(user).Reference(u => u.UserSecret).Load();
                
                    var complexIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Complex).Name});
                    var complexSecretIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(ComplexSecret).Name});
                    var roomIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Room).Name});
                    var messageIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Message).Name});
                    var membershipIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Membership).Name});
                    var memberAccessIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(MemberAccess).Name});

                    Task.WaitAll(complexIdTask, complexSecretIdTask, roomIdTask, messageIdTask, membershipIdTask,
                        memberAccessIdTask);
                    
                    var complex = new Complex
                    {
                        ComplexId = (long)complexIdTask.Result.Index,
                        Title = packet.Complex.Title,
                        Avatar = packet.Complex.Avatar > 0 ? packet.Complex.Avatar : 0,
                        Mode = 3,
                        ComplexSecret = new ComplexSecret()
                        {
                            ComplexSecretId = (long)complexSecretIdTask.Result.Index,
                            Admin = user
                        },
                        Rooms = new List<Room>()
                        {
                            new Room()
                            {
                                RoomId = (long)roomIdTask.Result.Index,
                                Title = "Hall",
                                Avatar = 0,
                                Messages = new List<Message>()
                                {
                                    new ServiceMessage()
                                    {
                                        MessageId = (long)messageIdTask.Result.Index,
                                        Author = null,
                                        Text = "Room created.",
                                        Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                    }
                                }
                            }
                        },
                        Members = new List<Membership>()
                        {
                            new Membership()
                            {
                                MembershipId = (long)membershipIdTask.Result.Index,
                                User = user,
                                MemberAccess = new MemberAccess()
                                {
                                    MemberAccessId = (long)memberAccessIdTask.Result.Index,
                                    CanCreateMessage = true,
                                    CanModifyAccess = true,
                                    CanModifyWorkers = true,
                                    CanSendInvite = true,
                                    CanUpdateProfiles = true
                                }
                            }
                        }
                    };
                    complex.ComplexSecret.Complex = complex;
                    complex.Rooms[0].Complex = complex;
                    complex.Members[0].Complex = complex;
                    complex.Members[0].MemberAccess.Membership = complex.Members[0];
                    complex.Rooms[0].Messages[0].Room = complex.Rooms[0];
                    
                    dbContext.AddRange(complex);
                    dbContext.SaveChanges();

                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Complex_" + complex.ComplexId + "_CityService",
                            Number = complex.Version
                        },
                        new Version()
                        {
                            VersionId = "ComplexSecret_" + complex.ComplexSecret.ComplexSecretId + "_CityService",
                            Number = complex.ComplexSecret.Version
                        },
                        new Version()
                        {
                            VersionId = "Room_" + complex.Rooms[0].RoomId + "_CityService",
                            Number = complex.Rooms[0].Version
                        },
                        new Version()
                        {
                            VersionId = "Membership_" + complex.Members[0].MembershipId + "_CityService",
                            Number = complex.Members[0].Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + complex.Members[0].MemberAccess.MemberAccessId +
                                        "_CityService",
                            Number = complex.Members[0].MemberAccess.Version
                        }
                    };

                    NotifyVersionsUpdated(versions);
  

                    return (new CreateComplexResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "success",
                            Complex = complex,
                            ComplexSecret = complex.ComplexSecret,
                            ServiceMessage = (ServiceMessage) complex.Rooms[0].Messages[0],
                            Versions = versions
                        }
                    });
                }

                return (new CreateComplexResponse()
                {
                    Packet = new Packet {Status = "error_0"}
                });
            }
        }

        public DeleteComplexResponse AnswerQuestion(AnswerContext<DeleteComplexRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complex = dbContext.Complexes.Find(packet.Complex.ComplexId);
                if (complex != null)
                {
                    dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                    if (complex.ComplexSecret.AdminId == session.BaseUserId)
                    {
                        if (complex.Title != "" && complex.Title.ToLower() != "home")
                        {
                            dbContext.Entry(complex).Collection(c => c.Members).Query()
                                .Include(mem => mem.MemberAccess)
                                .Include(mem => mem.User)
                                .ThenInclude(u => u.Sessions)
                                .Load();
                            dbContext.Entry(complex).Collection(c => c.Rooms)
                                .Query().Include(r => r.Workers).Load();
                            var sessionIds =
                                (from m in complex.Members
                                    where m.UserId == session.BaseUserId
                                    from s in m.User.Sessions
                                    select s.SessionId).ToList();

                            var members = complex.Members.ToList();

                            foreach (var membership in members)
                            {
                                dbContext.MemberAccesses.Remove(membership.MemberAccess);
                                dbContext.Entry(membership).Reference(m => m.User).Load();
                                var user = membership.User;
                                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                                user.Memberships.Remove(membership);
                                dbContext.Memberships.Remove(membership);
                            }

                            var cdn = new ComplexDeletionNotification()
                            {
                                ComplexId = complex.ComplexId
                            };
                            var push = new ComplexDeletionPush()
                            {
                                SessionIds = sessionIds,
                                Notif = cdn
                            };
                            
                            var kt = new KafkaTransport();
                            kt.PushNotifToApiGateway(push);
                            
                            dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                            foreach (var room in complex.Rooms)
                            {
                                var messageIds = dbContext.Messages.Where(m => m.RoomId == room.RoomId)
                                    .Select(m => m.MessageId).ToList();
                                dbContext.MessageSeens.Where(m => messageIds.Contains(m.MessageId.Value)).DeleteFromQuery();
                                dbContext.Messages.Where(m => m.RoomId == room.RoomId).DeleteFromQuery();
                                dbContext.BaseRooms.Remove(room);
                            }

                            dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                            dbContext.ComplexSecrets.Remove(complex.ComplexSecret);
                            dbContext.Complexes.Remove(complex);

                            dbContext.SaveChanges();

            
                            
                            return (new DeleteComplexResponse()
                            {
                                Packet = new Packet {Status = "success"}
                            });
                        }

                        return (new DeleteComplexResponse()
                        {
                            Packet = new Packet {Status = "error_0"}
                        });
                    }

                    return (new DeleteComplexResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    });
                }

                return (new DeleteComplexResponse()
                {
                    Packet = new Packet {Status = "error_1"}
                });
            }
        }

        public UpdateRoomProfileResponse AnswerQuestion(AnswerContext<UpdateRoomProfileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complex = dbContext.Complexes.Find(packet.BaseRoom.ComplexId);
                dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                if (complex.ComplexSecret.AdminId == session.BaseUser.BaseUserId)
                {
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    if (complex.Rooms.Any(r => r.RoomId == packet.BaseRoom.RoomId))
                    {
                        var room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                        room.Title = packet.BaseRoom.Title;
                        room.Avatar = packet.BaseRoom.Avatar;
                        room.BackgroundUrl = packet.BaseRoom.BackgroundUrl;
                        dbContext.SaveChanges();

                        var tx = dbContext.Database.BeginTransaction();
                        IncreaseVersion(dbContext, "RoomId", room.RoomId, "Rooms");
                        tx.Commit();
                        dbContext.SaveChanges();

                        var version = dbContext.BaseRooms.Find(room.RoomId).Version;

                        var versions = new List<Version>()
                        {
                            new Version()
                            {
                                VersionId = "Room_" + room.RoomId + "_CityService",
                                Number = version
                            }
                        };

                        NotifyVersionsUpdated(versions);

                        var kt = new KafkaTransport();
          
                        
                        return (new UpdateRoomProfileResponse()
                        {
                            Packet = new Packet {Status = "success", Versions = versions}
                        });
                    }

                    return (new UpdateRoomProfileResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    });
                }

                if (complex.Mode == 2)
                {
                    BaseRoom baseRoom = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);
                    if (baseRoom != null)
                    {
                        baseRoom.BackgroundUrl = packet.BaseRoom.BackgroundUrl;
                        dbContext.SaveChanges();
                        return (new UpdateRoomProfileResponse()
                        {
                            Packet = new Packet {Status = "success"}
                        });
                    }
                }

                return (new UpdateRoomProfileResponse()
                {
                    Packet = new Packet {Status = "error_1"}
                });
            }
        }

        public DeleteRoomResponse AnswerQuestion(AnswerContext<DeleteRoomRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = (User) session.BaseUser;
                dbContext.Entry(human).Collection(u => u.Memberships).Load();
                if (human.Memberships.Any(m => m.ComplexId == packet.BaseRoom.ComplexId))
                {
                    var complex = dbContext.Complexes.Find(packet.BaseRoom.ComplexId);
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    if (complex.Rooms.Count > 1 && complex.Rooms.Any(r => r.RoomId == packet.BaseRoom.RoomId))
                    {
                        var room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                        complex.Rooms.Remove(room);
                        dbContext.BaseRooms.Remove(room);
                        dbContext.SaveChanges();
                        
                        var kt = new KafkaTransport();
         

                        dbContext.Entry(complex).Collection(c => c.Members).Load();
                        var sessionIds = new List<long>();
                        foreach (var ms in complex.Members)
                        {
                            try
                            {
                                dbContext.Entry(ms).Reference(mem => mem.User).Load();
                                dbContext.Entry(ms.User).Collection(u => u.Sessions).Load();
                                foreach (var userSession in ms.User.Sessions)
                                {
                                    sessionIds.Add(userSession.SessionId);
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }

                        var rdn = new RoomDeletionNotification()
                        {
                            ComplexId = complex.ComplexId,
                            RoomId = room.RoomId,
                        };

                        kt.PushNotifToApiGateway(new RoomDeletionPush()
                            {
                                SessionIds = sessionIds,
                                Notif = rdn
                            }
                        );
                        
                        return (new DeleteRoomResponse()
                        {
                            Packet = new Packet {Status = "success"}
                        });
                    }

                    return (new DeleteRoomResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    });
                }

                return (new DeleteRoomResponse()
                {
                    Packet = new Packet {Status = "error_1"}
                });
            }
        }

        public CreateContactResponse AnswerQuestion(AnswerContext<CreateContactRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var me = (User) session.BaseUser;
                var peer = (User) dbContext.BaseUsers.Find(packet.User.BaseUserId);
                if (me.BaseUserId == peer.BaseUserId)
                {
                    return (new CreateContactResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "error_1",
                        }
                    });
                }

                dbContext.Entry(me).Collection(u => u.Contacts).Load();

                if (me.Contacts.All(c => c.PeerId != packet.User.BaseUserId))
                {
                    var kt = new KafkaTransport();
                    var complexIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Complex).Name});
                    var complexSecretIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(ComplexSecret).Name});
                    var roomIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Room).Name});
                    var messageIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Message).Name});
                    var membershipIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Membership).Name});
                    var memberAccessIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(MemberAccess).Name});
                    var membershipIdTask2 = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Membership).Name});
                    var memberAccessIdTask2 = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(MemberAccess).Name});
                    var contactIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Contact).Name});
                    var contactIdTask2 = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Contact).Name});

                    Task.WaitAll(complexIdTask, complexSecretIdTask, roomIdTask, messageIdTask, membershipIdTask,
                        memberAccessIdTask, membershipIdTask2, memberAccessIdTask2, contactIdTask, contactIdTask2);

                    var complex = new Complex
                    {
                        ComplexId = (long)complexIdTask.Result.Index,
                        Title = "",
                        Avatar = -1,
                        Mode = 2
                    };
                    var complexSecret = new ComplexSecret
                    {
                        ComplexSecretId = (long)complexSecretIdTask.Result.Index,
                        Admin = null,
                        Complex = complex
                    };
                    complex.ComplexSecret = complexSecret;
                    var room = new Room()
                    {
                        RoomId = (long)roomIdTask.Result.Index,
                        Title = "Hall",
                        Avatar = 0,
                        Complex = complex,
                        Messages = new List<Message>()
                        {
                            new ServiceMessage
                            {
                                MessageId = (long)messageIdTask.Result.Index,
                                Text = "Room created.",
                                Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                Author = null
                            }
                        }
                    };
                    room.Messages[0].Room = room;
                    var m1 = new Membership()
                    {
                        MembershipId = (long)membershipIdTask.Result.Index,
                        User = me,
                        Complex = complex,
                        MemberAccess = new MemberAccess()
                        {
                            MemberAccessId = (long)memberAccessIdTask.Result.Index,
                            CanCreateMessage = true,
                            CanModifyAccess = false,
                            CanModifyWorkers = true,
                            CanSendInvite = true,
                            CanUpdateProfiles = true
                        }
                    };
                    var m2 = new Membership()
                    {
                        MembershipId = (long)membershipIdTask2.Result.Index,
                        User = peer,
                        Complex = complex,
                        MemberAccess = new MemberAccess()
                        {
                            MemberAccessId = (long)memberAccessIdTask2.Result.Index,
                            CanCreateMessage = true,
                            CanModifyAccess = false,
                            CanModifyWorkers = true,
                            CanSendInvite = true,
                            CanUpdateProfiles = true
                        }
                    };
                    var myContact = new Contact
                    {
                        ContactId = (long)contactIdTask.Result.Index,
                        Complex = complex,
                        User = me,
                        Peer = peer
                    };
                    var peerContact = new Contact
                    {
                        ContactId = (long)contactIdTask2.Result.Index,
                        Complex = complex,
                        User = peer,
                        Peer = me
                    };
                    m1.MemberAccess.Membership = m1;
                    m2.MemberAccess.Membership = m2;
                    dbContext.AddRange(complex, complexSecret, room, m1, m2, myContact, peerContact);
                    dbContext.SaveChanges();

                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Complex_" + complex.ComplexId + "_CityService",
                            Number = complex.Version
                        },
                        new Version()
                        {
                            VersionId = "ComplexSecret_" + complexSecret.ComplexSecretId + "_CityService",
                            Number = complexSecret.Version
                        },
                        new Version()
                        {
                            VersionId = "Room_" + room.RoomId + "_CityService",
                            Number = room.Version
                        },
                        new Version()
                        {
                            VersionId = "Membership_" + m1.MembershipId + "_CityService",
                            Number = m1.Version
                        },
                        new Version()
                        {
                            VersionId = "Membership_" + m2.MembershipId + "_CityService",
                            Number = m2.Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + m1.MemberAccess.MemberAccessId + "_CityService",
                            Number = m1.MemberAccess.Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + m2.MemberAccess.MemberAccessId + "_CityService",
                            Number = m2.MemberAccess.Version
                        },
                        new Version()
                        {
                            VersionId = "Contact_" + myContact.ContactId + "_CityService",
                            Number = myContact.Version
                        },
                        new Version()
                        {
                            VersionId = "Contact_" + peerContact.ContactId + "_CityService",
                            Number = peerContact.Version
                        }
                    };

                    NotifyVersionsUpdated(versions);
                    

                    dbContext.Entry(peerContact.Complex).Collection(c => c.Rooms).Load();
                    dbContext.Entry(peer).Collection(u => u.Sessions).Load();
                    var sessionIds = peer.Sessions.Select(s => s.SessionId).ToList();

                    dbContext.Entry(myContact).State = EntityState.Detached;
                    dbContext.Entry(peerContact).State = EntityState.Detached;
                    dbContext.Entry(room).State = EntityState.Detached;
                    
                    myContact = dbContext.Contacts.Find(myContact.ContactId);
                    dbContext.Entry(myContact).Reference(c => c.Complex)
                        .Query().Include(c => c.Rooms).Include(c => c.SingleRooms)
                        .Include(c => c.Members)
                        .ThenInclude(m => m.User).Include(c => c.Members)
                        .ThenInclude(m => m.MemberAccess).Load();
                    dbContext.Entry(myContact).Reference(c => c.User).Load();
                    dbContext.Entry(myContact).Reference(c => c.Peer).Load();

                    peerContact = dbContext.Contacts.Find(peerContact.ContactId);
                    dbContext.Entry(peerContact).Reference(c => c.Complex)
                        .Query().Include(c => c.Rooms).Include(c => c.SingleRooms)
                        .Include(c => c.Members)
                        .ThenInclude(m => m.User).Include(c => c.Members)
                        .ThenInclude(m => m.MemberAccess).Load();
                    dbContext.Entry(peerContact).Reference(c => c.User).Load();
                    dbContext.Entry(peerContact).Reference(c => c.Peer).Load();
                    
                    room = (Room) dbContext.BaseRooms.Find(room.RoomId);
                    dbContext.Entry(room).Reference(r => r.Complex).Load();
                    dbContext.Entry(room).Collection(r => r.Messages).Load();
                    dbContext.Entry(room.Messages[0]).Reference(m => m.Room);
                    
                    var ccn = new ContactCreationNotification
                    {
                        Contact = peerContact,
                        ComplexSecret = complexSecret,
                        Message = (ServiceMessage)  room.Messages[0]
                    };
                    
                    kt.PushNotifToApiGateway(
                        new ContactCreationPush()
                        {
                            Notif = ccn,
                            SessionIds = sessionIds
                        });
                    
                    return (new CreateContactResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "success",
                            Contact = myContact,
                            ServiceMessage = (ServiceMessage) room.Messages[0],
                            ComplexSecret = complexSecret,
                            Versions = versions
                        }
                    });
                }

                return (new CreateContactResponse()
                {
                    Packet = new Packet {Status = "error_050"}
                });
            }
        }

        public async Task<CreateInviteResponse> AnswerQuestion(AnswerContext<CreateInviteRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = dbContext.Users.Find(packet.User.BaseUserId);
                dbContext.Entry(human).Collection(h => h.Memberships).Load();

                if (human.Memberships.Any(m => m.ComplexId == packet.Complex.ComplexId))
                {
                    return (new CreateInviteResponse()
                    {
                        Packet = new Packet {Status = "error_0H2"}
                    });
                }

                dbContext.Entry(human).Collection(h => h.Invites).Load();
                if (human.Invites.Any(i => i.ComplexId == packet.Complex.ComplexId))
                {
                    return (new CreateInviteResponse()
                    {
                        Packet = new Packet {Status = "error_0H1"}
                    });
                }

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var me = (User) session.BaseUser;
                dbContext.Entry(me).Collection(u => u.Memberships).Load();
                var mem = me.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (mem == null)
                {
                    return (new CreateInviteResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    });
                }

                dbContext.Entry(mem).Reference(m => m.Complex).Load();
                var complex = mem.Complex;
                if (complex.Mode == 1 || complex.Mode == 2)
                {
                    return (new CreateInviteResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    });
                }

                dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                dbContext.Entry(mem).Reference(m => m.MemberAccess).Load();
                if (mem.MemberAccess.CanSendInvite)
                {
                    var kt = new KafkaTransport();
                    var inviteId = await kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Invite).Name});

                    var invite = new Invite()
                    {
                        InviteId = (long) inviteId.Index,
                        Complex = complex,
                        User = human
                    };
                    dbContext.AddRange(invite);
                    dbContext.SaveChanges();

                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Invite_" + invite.InviteId + "_CityService",
                            Number = invite.Version
                        }
                    };

                    NotifyVersionsUpdated(versions);

       
                    
                    dbContext.Entry(human).Collection(h => h.Sessions).Load();
                    var sessionIds = new List<long>();
                    foreach (var targetSession in human.Sessions)
                    {
                        sessionIds.Add(targetSession.SessionId);
                    }

                    var inviteNotification = new InviteCreationNotification()
                    {
                        Invite = invite
                    };

                    kt.PushNotifToApiGateway(new InviteCreationPush()
                    {
                        SessionIds = sessionIds,
                        Notif = inviteNotification
                    });
                    
                    return (new CreateInviteResponse()
                    {
                        Packet = new Packet {Status = "success", Invite = invite, Versions = versions}
                    });
                }

                return (new CreateInviteResponse()
                {
                    Packet = new Packet {Status = "error_0H0"}
                });
            }
        }

        public CancelInviteResponse AnswerQuestion(AnswerContext<CancelInviteRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var mem = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (mem == null)
                {
                    return (new CancelInviteResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    });
                }

                dbContext.Entry(mem).Reference(m => m.Complex).Load();
                var complex = mem.Complex;
                if (complex != null)
                {
                    dbContext.Entry(complex).Collection(c => c.Invites).Load();
                    var invite = complex.Invites.Find(i => i.UserId == packet.User.BaseUserId);
                    if (invite != null)
                    {
                        dbContext.Entry(invite).Reference(i => i.User).Load();
                        var human = invite.User;
                        dbContext.Entry(human).Collection(h => h.Invites).Load();
                        human.Invites.Remove(invite);
                        dbContext.Invites.Remove(invite);
                        dbContext.SaveChanges();

                        var kt = new KafkaTransport();
               
                        
                        dbContext.Entry(human).Collection(h => h.Sessions).Load();
                        var sessionIds = human.Sessions.Select(s => s.SessionId).ToList();
                        var notification = new InviteCancellationNotification
                        {
                            Invite = invite
                        };
                        
                        kt.PushNotifToApiGateway(
                            new InviteCancellationPush()
                            {
                                Notif = notification,
                                SessionIds = sessionIds
                            });
                        
                        return (new CancelInviteResponse()
                        {
                            Packet = new Packet {Status = "success"}
                        });
                    }

                    return (new CancelInviteResponse()
                    {
                        Packet = new Packet {Status = "error_0I0"}
                    });
                }

                return (new CancelInviteResponse()
                {
                    Packet = new Packet {Status = "error_0I2"}
                });
            }
        }

        public AcceptInviteResponse AnswerQuestion(AnswerContext<AcceptInviteRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = (User) session.BaseUser;
                dbContext.Entry(human).Collection(h => h.Invites).Load();
                var invite = human.Invites.Find(i => i.ComplexId == packet.Complex.ComplexId);
                
                var kt = new KafkaTransport();
                
                if (invite != null)
                {
                    dbContext.Entry(invite).Reference(i => i.Complex).Load();
                    var complex = invite.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User).Load();
                    human.Invites.Remove(invite);
                    dbContext.Invites.Remove(invite);
                    
                    var membershipIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Membership).Name});
                    var memberAccessIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(MemberAccess).Name});
                    var messageIdTask = kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Message).Name});

                    Task.WaitAll(membershipIdTask, memberAccessIdTask, messageIdTask);
                    
                    var membership = new Membership
                    {
                        MembershipId = (long)membershipIdTask.Result.Index,
                        User = human,
                        Complex = complex,
                        MemberAccess = new MemberAccess()
                        {
                            MemberAccessId = (long)memberAccessIdTask.Result.Index,
                            CanCreateMessage = true,
                            CanModifyAccess = false,
                            CanModifyWorkers = false,
                            CanSendInvite = false,
                            CanUpdateProfiles = false
                        }
                    };
                    membership.MemberAccess.Membership = membership;

                    dbContext.Entry(complex).Collection(c => c.Members).Load();
                    complex.Members.Add(membership);
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    var hall = complex.Rooms.FirstOrDefault();

                    var message = new ServiceMessage()
                    {
                        MessageId = (long)messageIdTask.Result.Index,
                        Room = hall,
                        Author = null,
                        Text = human.Title + " entered complex by invite.",
                        Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };

                    dbContext.Messages.Add(message);
                    
                    dbContext.SaveChanges();

                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Membership_" + membership.MembershipId + "_CityService",
                            Number = membership.Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + membership.MemberAccess.MemberAccessId + "_CityService",
                            Number = membership.MemberAccess.Version
                        }
                    };

                    NotifyVersionsUpdated(versions);

                    membership = dbContext.Memberships.Find(membership.MembershipId);
                    dbContext.Entry(membership).Reference(m => m.MemberAccess).Load();
                    dbContext.Entry(membership).Reference(m => m.User).Load();
                    dbContext.Entry(membership).Reference(m => m.Complex).Query()
                        .Include(c => c.Rooms).Include(c => c.Members)
                        .ThenInclude(m => m.User).Load();


                    dbContext.Entry(human).State = EntityState.Detached;
                    human = (User) dbContext.BaseUsers.Find(human.BaseUserId);
                    
                    dbContext.Entry(complex)
                        .Collection(c => c.Members)
                        .Query().Include(m => m.User)
                        .ThenInclude(u => u.Sessions)
                        .Include(m => m.MemberAccess)
                        .Load();

                    dbContext.Entry(complex).Collection(c => c.Rooms).Query()
                        .Include(r => r.Workers).Load();

                    var adminsSessionIds = (from m in complex.Members
                        where m.MemberAccess.CanModifyAccess &&
                              m.UserId != human.BaseUserId
                        from s
                            in m.User.Sessions
                        select s.SessionId).ToList();

                    var allSessionIds = (from sess in (from m in complex.Members
                            where m.UserId != human.BaseUserId
                            from s
                                in m.User.Sessions
                            select s)
                        select sess.SessionId).ToList();

                    var nonAdminSessionIds = new List<long>(allSessionIds);
                    nonAdminSessionIds.RemoveAll(sId => adminsSessionIds.Contains(sId));
                    
                    var botIds = new HashSet<long>(dbContext.Workerships.Where(w => complex.Rooms
                        .Any(r => r.RoomId == w.RoomId)).Select(w => w.BotId).ToList());
                    var botsQuery = dbContext.Bots.Where(b => botIds.Contains(b.BaseUserId));
                    botsQuery.Include(b => b.Sessions).Load();
                    var bots = botsQuery.ToList();
                    var botSessionIds = bots.Select(b => b.Sessions.First().SessionId);
                    nonAdminSessionIds.AddRange(botSessionIds);

                    Membership lightMembership;
                    using (var dbContextFinal = new DatabaseContext())
                    {
                        lightMembership = dbContextFinal.Memberships.Find(membership.MembershipId);
                        dbContextFinal.Entry(lightMembership).Reference(mem => mem.User).Load();
                        dbContextFinal.Entry(lightMembership).Reference(mem => mem.Complex).Load();
                        dbContextFinal.Entry(lightMembership.Complex).Collection(c => c.Rooms).Load();
                    }

                    var ujnFull = new UserJointComplexNotification
                    {
                        Membership = membership,
                        Message = message
                    };
                    var ujnLight = new UserJointComplexNotification
                    {
                        Membership = lightMembership,
                        Message = message
                    };
                    
                    kt.PushNotifToApiGateway(
                        new UserJointComplexPush()
                        {
                            Notif = ujnFull,
                            SessionIds = adminsSessionIds
                        });
                    
                    kt.PushNotifToApiGateway(
                        new UserJointComplexPush()
                        {
                            Notif = ujnLight,
                            SessionIds = nonAdminSessionIds
                        });

                    dbContext.SaveChanges();
                    dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                    dbContext.Entry(complex.ComplexSecret).Reference(cs => cs.Admin).Load();
                    var inviter = complex.ComplexSecret.Admin;
                    dbContext.Entry(inviter).Collection(i => i.Sessions).Load();

                    var inviterSessionIds = inviter.Sessions.Select(s => s.SessionId).ToList();
                    var notification = new InviteAcceptanceNotification
                    {
                        Invite = invite
                    };
                    
                    kt.PushNotifToApiGateway(
                        new InviteAcceptancePush()
                        {
                            Notif = notification,
                            SessionIds = inviterSessionIds
                        });
                    
                    dbContext.Entry(membership.Complex).Collection(c => c.Rooms).Query()
                        .Include(r => r.Workers).Load();

                    return (new AcceptInviteResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "success",
                            Membership = membership,
                            Bots = bots,
                            ServiceMessage = message,
                            ComplexSecret = complex.ComplexSecret,
                            Versions = versions
                        }
                    });
                }

                return (new AcceptInviteResponse()
                {
                    Packet = new Packet {Status = "error_0J0"}
                });
            }
        }

        public IgnoreInviteResponse AnswerQuestion(AnswerContext<IgnoreInviteRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = (User) session.BaseUser;
                dbContext.Entry(human).Collection(h => h.Invites).Load();
                var invite = human.Invites.Find(i => i.ComplexId == packet.Complex.ComplexId);
                if (invite != null)
                {
                    dbContext.Entry(invite).Reference(i => i.Complex).Load();
                    var complex = invite.Complex;
                    human.Invites.Remove(invite);
                    dbContext.Invites.Remove(invite);
                    dbContext.SaveChanges();

                    var kt = new KafkaTransport();
          

                    dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                    dbContext.Entry(complex.ComplexSecret).Reference(cs => cs.Admin).Load();
                    var inviter = complex.ComplexSecret.Admin;
                    dbContext.Entry(inviter).Collection(i => i.Sessions).Load();

                    var sessionIds = inviter.Sessions.Select(s => s.SessionId).ToList();
                    var notification = new InviteIgnoranceNotification
                    {
                        Invite = invite
                    };

                    kt.PushNotifToApiGateway(
                        new InviteIgnoredPush()
                        {
                            SessionIds = sessionIds,
                            Notif = notification
                        });
                    
                    return (new IgnoreInviteResponse()
                    {
                        Packet = new Packet {Status = "success"}
                    });
                }

                return (new IgnoreInviteResponse()
                {
                    Packet = new Packet {Status = "error_0K0"}
                });
            }
        }

        public async Task<CreateRoomResponse> AnswerQuestion(AnswerContext<CreateRoomRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var mem = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (mem == null)
                {
                    return (new CreateRoomResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    });
                }

                dbContext.Entry(mem).Reference(m => m.Complex).Load();
                var complex = mem.Complex;
                
                var kt = new KafkaTransport();
                
                if (complex != null)
                {
                    BaseRoom room;
                    
                    if (packet.SingleRoomMode.HasValue && packet.SingleRoomMode.Value)
                    {
                        if (user.BaseUserId == packet.User.BaseUserId)
                        {
                            return (new CreateRoomResponse()
                            {
                                Packet = new Packet {Status = "error_5"}
                            });
                        }
                        
                        room = (SingleRoom) dbContext.BaseRooms.FirstOrDefault(r => r is SingleRoom && 
                                                                                    r.ComplexId == packet.Complex.ComplexId 
                                                                                    && (
                                                                                        (((SingleRoom) r).User1Id == user.BaseUserId &&
                                                                                         ((SingleRoom) r).User2Id == packet.User.BaseUserId) || 
                                                                                        (((SingleRoom) r).User1Id == packet.User.BaseUserId &&
                                                                                         ((SingleRoom) r).User2Id == user.BaseUserId)
                                                                                        ));
                        if (room != null)
                        {
                            return (new CreateRoomResponse()
                            {
                                Packet = new Packet {Status = "error_4"}
                            });
                        }

                        var targetUser = (User) dbContext.BaseUsers.Find(packet.User.BaseUserId);
                        if (targetUser == null)
                        {
                            return (new CreateRoomResponse()
                            {
                                Packet = new Packet {Status = "error_3"}
                            });
                        }

                        var roomId = kt.AskForNewId(
                            new AskIndexEntity {EntityType = typeof(Room).Name});
                        var messageId = kt.AskForNewId(
                            new AskIndexEntity {EntityType = typeof(Message).Name});

                        Task.WaitAll(roomId, messageId);
                        
                        var sRoom = new SingleRoom()
                        {
                            RoomId = (long)roomId.Result.Index,
                            Title = "",
                            Avatar = -1,
                            Complex = complex,
                            User1 = user,
                            User2 = targetUser,
                            Messages = new List<Message>()
                            {
                                new ServiceMessage()
                                {
                                    MessageId = messageId.Result.Index,
                                    Author = null,
                                    Text = "Room created.",
                                    Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                }
                            }
                        };
                        sRoom.Messages[0].Room = sRoom;
                        dbContext.AddRange(sRoom);
                        dbContext.SaveChanges();

                        dbContext.Entry(sRoom).State = EntityState.Detached;

                        sRoom = (SingleRoom) dbContext.BaseRooms.Find(sRoom.RoomId);
                        dbContext.Entry(sRoom).Reference(r => r.User1).Load();
                        dbContext.Entry(sRoom).Reference(r => r.User2).Load();
                        dbContext.Entry(sRoom).Reference(r => r.Complex).Load();
                        
                        room = sRoom;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(packet.BaseRoom.Title))
                        {
                            return (new CreateRoomResponse()
                            {
                                Packet = new Packet {Status = "error_0"}
                            });
                        }
                        
                        var roomId = kt.AskForNewId(
                            new AskIndexEntity {EntityType = typeof(Room).Name});
                        var messageId = kt.AskForNewId(
                            new AskIndexEntity {EntityType = typeof(Message).Name});

                        Task.WaitAll(roomId, messageId);
                        
                        room = new Room()
                        {
                            RoomId = (long)roomId.Result.Index,
                            Title = packet.BaseRoom.Title,
                            Avatar = packet.BaseRoom.Avatar,
                            Complex = complex,
                            Messages = new List<Message>()
                            {
                                new ServiceMessage()
                                {
                                    MessageId = messageId.Result.Index,
                                    Author = null,
                                    Text = "Room created.",
                                    Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                }
                            }
                        };
                        room.Messages[0].Room = room;
                        dbContext.AddRange(room);
                        dbContext.SaveChanges();
                    }


                    var versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Room_" + room.RoomId + "_CityService",
                            Number = room.Version
                        }
                    };

                    NotifyVersionsUpdated(versions);
                    

                    List<long> sessionIds;

                    if (packet.SingleRoomMode.HasValue && packet.SingleRoomMode.Value)
                    {
                        var targetUser = (User) dbContext.BaseUsers.Find(packet.User.BaseUserId);
                        dbContext.Entry(targetUser).Collection(u => u.Sessions).Load();
                        sessionIds = (from s in targetUser.Sessions select s.SessionId).ToList();
                    }
                    else
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User)
                            .ThenInclude(u => u.Sessions).Load();
                        sessionIds = (from m in complex.Members from s in m.User.Sessions select s.SessionId).ToList();
                    }

                    var roomNotif = new RoomCreationNotification();
                    if (room is Room ro)
                    {
                        roomNotif.Room = ro;
                        roomNotif.SingleRoom = null;
                    }
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    else if (room is SingleRoom sr)
                    {
                        roomNotif.SingleRoom = sr;
                        roomNotif.Room = null;
                    }

                    roomNotif.Message = (ServiceMessage) room.Messages[0];

                    kt.PushNotifToApiGateway(new RoomCreationPush()
                    {
                        Notif = roomNotif,
                        SessionIds = sessionIds
                    });
                    
                    return (new CreateRoomResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "success",
                            BaseRoom = room,
                            ServiceMessage = (ServiceMessage) room.Messages[0],
                            Versions = versions
                        }
                    });
                }

                return (new CreateRoomResponse()
                {
                    Packet = new Packet {Status = "error_1"}
                });
            }
        }

        public GetMyInvitesResponse AnswerQuestion(AnswerContext<GetMyInvitesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Invites).Query()
                    .Include(i => i.User).Include(i => i.Complex).Load();
                var invites = user.Invites.ToList();

                return (new GetMyInvitesResponse()
                {
                    Packet = new Packet() {Status = "success", Invites = invites}
                });
            }
        }

        public UpdateMemberAccessResponse AnswerQuestion(AnswerContext<UpdateMemberAccessRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();

                var membership = user.Memberships.Find(mem => mem.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return (new UpdateMemberAccessResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    });
                }

                dbContext.Entry(membership).Reference(mem => mem.MemberAccess).Load();
                dbContext.Entry(membership).Reference(mem => mem.Complex).Load();
                var complex = membership.Complex;
                if (complex.Mode == 1 || complex.Mode == 2)
                {
                    return (new UpdateMemberAccessResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    });
                }

                dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                if (membership.UserId != complex.ComplexSecret.AdminId && !membership.MemberAccess.CanModifyAccess)
                {
                    return (new UpdateMemberAccessResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    });
                }

                dbContext.Entry(complex).Collection(c => c.Members).Load();
                var targetMem = complex.Members.Find(mem => mem.UserId == packet.User.BaseUserId);
                if (targetMem == null)
                {
                    return (new UpdateMemberAccessResponse()
                    {
                        Packet = new Packet() {Status = "error_4"}
                    });
                }

                if (packet.User.BaseUserId == complex.ComplexSecret.AdminId ||
                    packet.User.BaseUserId == user.BaseUserId)
                {
                    return (new UpdateMemberAccessResponse()
                    {
                        Packet = new Packet() {Status = "error_5"}
                    });
                }

                dbContext.Entry(targetMem).Reference(tmem => tmem.MemberAccess).Load();
                targetMem.MemberAccess.CanCreateMessage = packet.MemberAccess.CanCreateMessage;
                targetMem.MemberAccess.CanSendInvite = packet.MemberAccess.CanSendInvite;
                targetMem.MemberAccess.CanModifyWorkers = packet.MemberAccess.CanModifyWorkers;
                targetMem.MemberAccess.CanUpdateProfiles = packet.MemberAccess.CanUpdateProfiles;

                if (complex.ComplexSecret.AdminId == user.BaseUserId)
                    targetMem.MemberAccess.CanModifyAccess = packet.MemberAccess.CanModifyAccess;

                dbContext.SaveChanges();

                var tx = dbContext.Database.BeginTransaction();
                IncreaseVersion(dbContext, "MemberAccessId", targetMem.MemberAccess.MemberAccessId, "MemberAccesses");
                tx.Commit();
                dbContext.SaveChanges();

                var version = dbContext.MemberAccesses.Find(targetMem.MemberAccess.MemberAccessId).Version;

                var versions = new List<Version>()
                {
                    new Version()
                    {
                        VersionId = "MemberAccess_" + targetMem.MemberAccess.MemberAccessId + "_CityService",
                        Number = version
                    }
                };

                NotifyVersionsUpdated(versions);

                var kt = new KafkaTransport();
    
                
                dbContext.Entry(complex.ComplexSecret).Reference(cs => cs.Admin).Load();
                dbContext.Entry(complex.ComplexSecret.Admin).Collection(u => u.Sessions).Load();

                dbContext.Entry(complex).Collection(c => c.Members).Query()
                    .Include(m => m.User)
                    .ThenInclude(u => u.Sessions)
                    .Include(m => m.MemberAccess)
                    .Load();

                var sessionIds = (from m in complex.Members
                    where
                        (m.MemberAccess.CanModifyAccess &&
                         m.User.BaseUserId != user.BaseUserId) ||
                        m.User.BaseUserId == targetMem.User.BaseUserId
                    from s in m.User.Sessions
                    select s.SessionId).ToList();

                kt.PushNotifToApiGateway(
                    new MemberAccessUpdatedPush()
                    {
                        Notif = new MemberAccessUpdatedNotification()
                        {
                            MemberAccess = targetMem.MemberAccess
                        },
                        SessionIds = sessionIds
                    });
                
                return (new UpdateMemberAccessResponse()
                {
                    Packet = new Packet() {Status = "success", Versions = versions}
                });
            }
        }

        public GetComplexAccessesResponse AnswerQuestion(AnswerContext<GetComplexAccessesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(mem => mem.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return (new GetComplexAccessesResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    });
                }

                dbContext.Entry(membership).Reference(mem => mem.MemberAccess).Load();
                if (!membership.MemberAccess.CanModifyAccess)
                {
                    return (new GetComplexAccessesResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    });
                }

                dbContext.Entry(membership).Reference(mem => mem.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Members).Query()
                    .Include(mem => mem.MemberAccess)
                    .ThenInclude(ma => ma.Membership)
                    .Load();

                var mas = complex.Members.Select(mem => mem.MemberAccess).ToList();

                return (new GetComplexAccessesResponse()
                {
                    Packet = new Packet() {Status = "success", MemberAccesses = mas}
                });
            }
        }

        public async Task<RegisterResponse> AnswerQuestion(AnswerContext<RegisterRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var code = Security.MakeKey8();
                SendEmail(packet.Email,
                    "Verify new device",
                    "Your verification code is : " + code);

                var kt = new KafkaTransport();
                
                var pending = dbContext.Pendings.FirstOrDefault(p => p.Email == packet.Email);
                if (pending == null)
                {
                    var pendingId = await kt.AskForNewId(
                        new AskIndexEntity {EntityType = typeof(Pending).Name});

                    pending = new Pending
                    {
                        PendingId = (long)pendingId.Index,
                        Email = packet.Email,
                        VerifyCode = code
                    };
                    dbContext.AddRange(pending);
                    dbContext.SaveChanges();
                }
                else
                {
                    pending.VerifyCode = code;
                    dbContext.SaveChanges();
                }
                
                
                return new RegisterResponse()
                    {
                        Packet = new Packet()
                        {
                            Status = "success"
                        }
                    };
            }
        }

        public async Task<VerifyResponse> AnswerQuestion(AnswerContext<VerifyRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var pending = dbContext.Pendings.FirstOrDefault(p => p.Email == packet.Email);
                if (pending != null)
                {
                    var kt = new KafkaTransport();

                    if (packet.VerifyCode == pending.VerifyCode)
                    {
                        var totalVersions = new List<Version>();
                        User user;
                        var token = Security.MakeKey64();
                        var userAuth = dbContext.UserSecrets.FirstOrDefault(ua => ua.Email == packet.Email);
                        if (userAuth == null)
                        {
                            var userIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(User).Name});
                            var userSecretIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(UserSecret).Name});
                            var membershipIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(Membership).Name});
                            var complexIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(Complex).Name});
                            var complexSecretIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(ComplexSecret).Name});
                            var roomIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(Room).Name});
                            var messageIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(Message).Name});
                            var memberAccessIdTask = kt.AskForNewId(
                                new AskIndexEntity {EntityType = typeof(MemberAccess).Name});

                            await Task.WhenAll(userIdTask, userSecretIdTask, membershipIdTask, complexIdTask,
                                complexSecretIdTask, roomIdTask, messageIdTask, memberAccessIdTask);

                            user = new User()
                            {
                                BaseUserId = (long)userIdTask.Result.Index,
                                Title = "New User",
                                Avatar = -1,
                                UserSecret = new UserSecret()
                                {
                                    UserSecretId = (long)userSecretIdTask.Result.Index,
                                    Email = packet.Email
                                },
                                Memberships = new List<Membership>()
                                {
                                    new Membership()
                                    {
                                        MembershipId = (long)membershipIdTask.Result.Index,
                                        Complex = new Complex()
                                        {
                                            ComplexId = (long)complexIdTask.Result.Index,
                                            Title = "Home",
                                            Avatar = 0,
                                            Mode = 1,
                                            ComplexSecret = new ComplexSecret()
                                            {
                                                ComplexSecretId = (long)complexSecretIdTask.Result.Index,
                                            },
                                            Rooms = new List<Room>()
                                            {
                                                new Room()
                                                {
                                                    RoomId = (long)roomIdTask.Result.Index,
                                                    Title = "Main",
                                                    Avatar = 0,
                                                    Messages = new List<Message>()
                                                    {
                                                      new ServiceMessage()
                                                        {
                                                            MessageId = (long)messageIdTask.Result.Index,
                                                            Author = null,
                                                            Text = "Room created.",
                                                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        MemberAccess = new MemberAccess()
                                        {
                                            MemberAccessId = (long)memberAccessIdTask.Result.Index,
                                            CanCreateMessage = true,
                                            CanModifyAccess = true,
                                            CanModifyWorkers = true,
                                            CanSendInvite = true,
                                            CanUpdateProfiles = true
                                        }
                                    }
                                }
                            };
                            
                            user.UserSecret.User = user;
                            user.Memberships[0].User = user;
                            user.Memberships[0].Complex.ComplexSecret.Complex = user.Memberships[0].Complex;
                            user.Memberships[0].Complex.ComplexSecret.Admin = user;
                            user.Memberships[0].Complex.Rooms[0].Complex = user.Memberships[0].Complex;
                            user.UserSecret.Home = user.Memberships[0].Complex;
                            user.Memberships[0].User = user;
                            user.Memberships[0].MemberAccess.Membership = user.Memberships[0];
                            user.Memberships[0].Complex.Rooms[0].Messages[0].Room =
                                user.Memberships[0].Complex.Rooms[0];
                            
                            dbContext.AddRange(user);

                            dbContext.SaveChanges();

                            var accVersions = new List<Version>()
                            {
                                new Version()
                                {
                                    VersionId = "BaseUser_" + user.BaseUserId + "_CityService",
                                    Number = user.Version
                                },
                                new Version()
                                {
                                    VersionId = "UserSecret_" + user.UserSecret.UserSecretId + "_CityService",
                                    Number = user.UserSecret.Version
                                },
                                new Version()
                                {
                                    VersionId = "Membership_" + user.Memberships[0].MembershipId + "_CityService",
                                    Number = user.Memberships[0].Version
                                },
                                new Version()
                                {
                                    VersionId = "MemberAccess_" + user.Memberships[0].MemberAccess.MemberAccessId +
                                                "_CityService",
                                    Number = user.Memberships[0].MemberAccess.Version
                                },
                                new Version()
                                {
                                    VersionId = "Complex_" + user.Memberships[0].Complex.ComplexId + "_CityService",
                                    Number = user.Memberships[0].Complex.Version
                                },
                                new Version()
                                {
                                    VersionId = "ComplexSecret_" +
                                                user.Memberships[0].Complex.ComplexSecret.ComplexId +
                                                "_CityService",
                                    Number = user.Memberships[0].Complex.ComplexSecret.Version
                                },
                                new Version()
                                {
                                    VersionId = "Room_" + user.Memberships[0].Complex.Rooms[0].RoomId +
                                                "_CityService",
                                    Number = user.Memberships[0].Complex.Rooms[0].Version
                                }
                            };

                            totalVersions.AddRange(accVersions);

                            NotifyVersionsUpdated(accVersions);

                            userAuth = user.UserSecret;
                        }
                        else
                        {
                            dbContext.Entry(userAuth).Reference(us => us.User).Load();
                            user = userAuth.User;
                        }
                        
                        var sessionId = await kt.AskForNewId(
                            new AskIndexEntity {EntityType = typeof(Session).Name});
                        
                        var session = new Session()
                        {
                            SessionId = (long)sessionId.Index,
                            Token = token,
                            ConnectionId = "",
                            Online = false,
                            BaseUser = user
                        };
                        
                        dbContext.AddRange(session);
                        dbContext.Pendings.Remove(pending);
                        dbContext.SaveChanges();

                        var sessVersions = new List<Version>()
                        {
                            new Version()
                            {
                                VersionId = "Session_" + session.SessionId + "_CityService",
                                Number = session.Version
                            }
                        };

                        totalVersions.AddRange(sessVersions);
                        
                        NotifyVersionsUpdated(sessVersions);
                        
                        
                        dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                        dbContext.Entry(user).Reference(u => u.UserSecret).Query().Include(us => us.Home)
                            .ThenInclude(h => h.ComplexSecret).Include(us => us.Home).ThenInclude(h => h.Members)
                            .ThenInclude(m => m.MemberAccess).Load();
                        dbContext.Entry(user.UserSecret.Home).Collection(h => h.Rooms).Load();

                        return (
                            new VerifyResponse()
                            {
                                Packet = new Packet()
                                {
                                    Status = "success",
                                    Session = session,
                                    User = user,
                                    UserSecret = userAuth,
                                    ComplexSecret = user.UserSecret.Home.ComplexSecret,
                                    Versions = totalVersions
                                }
                            });
                    }

                    return (
                        new VerifyResponse()
                        {
                            Packet = new Packet()
                            {
                                Status = "error_020"
                            }
                        });
                }

                return (
                    new VerifyResponse()
                    {
                        Packet = new Packet()
                        {
                            Status = "error_021"
                        }
                    });
            }
        }

        public DeleteAccountResponse AnswerQuestion(AnswerContext<DeleteAccountRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Sessions).Load();
                dbContext.Entry(user).Reference(u => u.UserSecret).Load();

                user.Title = "Deleted User";
                user.Avatar = -1;
                user.UserSecret.Email = "";
                dbContext.Sessions.RemoveRange(user.Sessions);

                dbContext.SaveChanges();

                var sesses = user.Sessions.ToList();

                var kt = new KafkaTransport();
                
                return (new DeleteAccountResponse()
                {
                    Packet = new Packet() {Status = "success"}
                });
            }
        }

        public LogoutResponse AnswerQuestion(AnswerContext<LogoutRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                if (session != null)
                {
                    dbContext.Sessions.Remove(session);
                    dbContext.SaveChanges();
                }
                else
                {
                    session = new Session {SessionId = context.Question.SessionId};
                }
                
                var kt = new KafkaTransport();

                return (
                    new LogoutResponse()
                    {
                        Packet = new Packet()
                        {
                            Status = "success"
                        }
                    });
            }
        }
        
        public GetModuleServerAddressResponse AnswerQuestion(AnswerContext<GetModuleServerAddressRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;

                var module = (Module) dbContext.BaseUsers.Find(packet.Module.BaseUserId);

                dbContext.Entry(module).Reference(m => m.ModuleSecret).Load();

                return new GetModuleServerAddressResponse()
                {
                    Packet = new Packet()
                    {
                        ModuleSecret = new ModuleSecret() {ServerAddress = module.ModuleSecret.ServerAddress}
                    }
                };
            }
        }

        public CreateModuleResponse AnswerQuestion(AnswerContext<CreateModuleRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                var module = new Module();

                var token = "-" + Security.MakeKey64();

                var modSess = new Session()
                {
                    Token = token
                };

                var kt = new KafkaTransport();
                var moduleIdTask = kt.AskForNewId(
                    new AskIndexEntity {EntityType = typeof(BaseUser).Name});
                var sessionIdTask = kt.AskForNewId(
                    new AskIndexEntity {EntityType = typeof(Session).Name});
                var modSecretIdTask = kt.AskForNewId(
                    new AskIndexEntity() {EntityType = typeof(ModuleSecret).Name});
                var modCreationIdTask = kt.AskForNewId(
                    new AskIndexEntity() {EntityType = typeof(ModuleCreation).Name});
                Task.WaitAll(moduleIdTask, sessionIdTask, modSecretIdTask, modCreationIdTask);

                module.BaseUserId = (long) moduleIdTask.Result.Index;
                module.Title = packet.Module.Title;
                module.Avatar = packet.Module.Avatar > 0 ? packet.Module.Avatar : 0;
                module.Description = packet.Module.Description;

                modSess.SessionId = (long) sessionIdTask.Result.Index;
                modSess.BaseUser = module;


                var modSecret = new ModuleSecret()
                {
                    ModuleSecretId = (long)modSecretIdTask.Result.Index,
                    Module = module,
                    Creator = user,
                    Token = token,
                    ServerAddress = packet.ModuleSecret.ServerAddress
                };
                module.ModuleSecret = modSecret;

                var modCreation = new ModuleCreation()
                {
                    ModuleCreationId = (long)modCreationIdTask.Result.Index,
                    Module = module,
                    Creator = user
                };
                dbContext.AddRange(module, modSecret, modSess, modCreation);
                dbContext.SaveChanges();

                var versions = new List<Version>()
                {
                    new Version()
                    {
                        VersionId = "BaseUser_" + module.BaseUserId + "_MessengerService",
                        Number = module.Version
                    },
                    new Version()
                    {
                        VersionId = "Session_" + modSess.SessionId + "_MessengerService",
                        Number = module.Version
                    }
                };

                return new CreateModuleResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success",
                        Module = module,
                        ModuleSecret = modSecret,
                        ModuleCreation = modCreation,
                        Versions = versions
                    }
                };
            }
        }

        public UpdateModuleProfileResponse AnswerQuestion(AnswerContext<UpdateModuleProfileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.CreatedModules).Load();
                var modCreation = user.CreatedModules.Find(bc => bc.ModuleId == packet.Module.BaseUserId);
                if (modCreation == null)
                {
                    return new UpdateModuleProfileResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(modCreation).Reference(bc => bc.Module).Load();
                dbContext.Entry(modCreation.Module).Reference(m => m.ModuleSecret).Load();
                var module = modCreation.Module;
                module.Title = packet.Module.Title;
                module.Avatar = packet.Module.Avatar;
                module.Description = packet.Module.Description;
                module.ModuleSecret.ServerAddress = packet.ModuleSecret.ServerAddress;
                dbContext.SaveChanges();

                return new UpdateModuleProfileResponse()
                {
                    Packet = new Packet {Status = "success"}
                };
            }
        }

        public SearchModulesResponse AnswerQuestion(AnswerContext<SearchModulesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var modules = (from u in dbContext.BaseUsers
                    where u is Module
                    where EF.Functions.Like(u.Title, "%" + packet.SearchQuery + "%")
                    select u).Select(bu => (Module) bu).ToList();

                return new SearchModulesResponse()
                {
                    Packet = new Packet {Status = "success", Modules = modules}
                };
            }
        }

        public async Task<BotPermitModuleResponse> AnswerQuestion(AnswerContext<BotPermitModuleRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                dbContext.Entry(bot).Collection(b => b.ModulePermissions).Load();
                var modPermis = bot.ModulePermissions.Find(mp => mp.ModuleId == packet.Module.BaseUserId);

                if (modPermis != null)
                {
                    return new BotPermitModuleResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                var module = dbContext.BaseUsers.Find(packet.Module.BaseUserId) as Module;
                if (module == null)
                {
                    return new BotPermitModuleResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }
                
                var kt = new KafkaTransport();
                var id = await kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(ModulePermission).Name
                    });

                modPermis = new ModulePermission()
                {
                    ModulePermissionId = (long)id.Index,
                    Bot = bot,
                    Module = module
                };

                dbContext.ModulePermissions.Add(modPermis);

                dbContext.SaveChanges();

                dbContext.Entry(module).Collection(m => m.Sessions).Load();
                var sessionIds = new List<long>();
                if (module.Sessions.Count > 0)
                {
                    sessionIds = new List<long>() {module.Sessions.First().SessionId};
                }
                
                ModulePermission finalModPermis;
                using (var finalContext = new DatabaseContext())
                {
                    finalModPermis = finalContext.ModulePermissions.Find(modPermis.ModulePermissionId);
                    finalContext.Entry(finalModPermis).Reference(mp => mp.Bot).Load();
                    finalContext.Entry(finalModPermis).Reference(mp => mp.Module).Load();
                }

                kt.PushNotifToApiGateway(new ModulePermissionGrantedPush()
                {
                    Notif = new ModulePermissionGrantedNotification()
                    {
                        ModulePermission = finalModPermis
                    },
                    SessionIds = sessionIds
                });

                return new BotPermitModuleResponse()
                {
                    Packet = new Packet() {Status = "success", ModulePermission = finalModPermis}
                };
            }
        }

        public async Task<BotCreateDocumentFileResponse> AnswerQuestion(AnswerContext<BotCreateDocumentFileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                Document document;
                FileUsage fileUsage;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = session.BaseUser;
                
                var kt = new KafkaTransport();

                if (packet.BaseRoom.RoomId > 0)
                {
                    if (user is Module mod)
                    {
                        dbContext.Entry(mod).Collection(m => m.ModulePermissions).Load();
                        var modPermis = mod.ModulePermissions.Find(mp => mp.BotId == packet.Bot.BaseUserId);
                        if (modPermis == null)
                        {
                            return new BotCreateDocumentFileResponse()
                            {
                                Packet = new Packet() {Status = "error_1"}
                            };
                        }

                        dbContext.Entry(modPermis).Reference(mp => mp.Bot).Load();
                        user = modPermis.Bot;
                    }

                    var bot = (Bot) user;

                    dbContext.Entry(bot).Collection(b => b.Workerships).Load();
                    var ws = bot.Workerships.Find(w => w.RoomId == packet.BaseRoom.RoomId);
                    if (ws == null)
                    {
                        return new BotCreateDocumentFileResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }

                    dbContext.Entry(ws).Reference(w => w.Room).Load();
                    var room = ws.Room;
                    if (room == null)
                    {
                        return new BotCreateDocumentFileResponse()
                        {
                            Packet = new Packet {Status = "error_3"}
                        };
                    }

                    var id = kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    var fileUsageId = kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(FileUsage).Name
                    });

                    Task.WaitAll(id, fileUsageId);
                    
                    document = new Document()
                    {
                        FileId = id.Result.Index,
                        IsPublic = false,
                        Uploader = user,
                        Name = packet.Document.Name
                    };
                    dbContext.Files.Add(document);
                    fileUsage = new FileUsage()
                    {
                        FileUsageId = fileUsageId.Result.Index,
                        File = document,
                        Room = room
                    };
                    dbContext.FileUsages.Add(fileUsage);
                }
                else
                {
                    var id = await kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    
                    document = new Document()
                    {
                        FileId = (long)id.Index,
                        IsPublic = true,
                        Uploader = user,
                        Name = packet.Document.Name
                    };
                    dbContext.Files.Add(document);
                    fileUsage = null;
                }

                dbContext.SaveChanges();

                var filePath = Path.Combine(_dirPath, document.FileId.ToString());
                File.Create(filePath).Close();


                return new BotCreateDocumentFileResponse()
                {
                    Packet = new Packet {Status = "success", File = document, FileUsage = fileUsage}
                };
            }
        }

        public async Task<BotAppendTextToTxtFileResponse> AnswerQuestion(
            AnswerContext<BotAppendTextToTxtFileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = session.BaseUser;

                if (user is Module mod)
                {
                    dbContext.Entry(mod).Collection(m => m.ModulePermissions).Load();
                    var modPermis = mod.ModulePermissions.Find(mp => mp.BotId == packet.Bot.BaseUserId);
                    if (modPermis == null)
                    {
                        return new BotAppendTextToTxtFileResponse()
                        {
                            Packet = new Packet() {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(modPermis).Reference(mp => mp.Bot).Load();
                    user = modPermis.Bot;
                }

                var bot = (Bot) user;

                var file = dbContext.Files.Find(packet.File.FileId);

                if (!(file is Document))
                    return new BotAppendTextToTxtFileResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                
                if (file.UploaderId != bot.BaseUserId)
                    return new BotAppendTextToTxtFileResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                
                await File.AppendAllTextAsync(Path.Combine(_dirPath, file.FileId.ToString()), packet.Text);
                
                var kt = new KafkaTransport();


                return new BotAppendTextToTxtFileResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public BotExecuteSqlOnSqlFileResponse AnswerQuestion(AnswerContext<BotExecuteSqlOnSqlFileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = session.BaseUser;

                if (user is Module mod)
                {
                    dbContext.Entry(mod).Collection(m => m.ModulePermissions).Load();
                    var modPermis = mod.ModulePermissions.Find(mp => mp.BotId == packet.Bot.BaseUserId);
                    if (modPermis == null)
                    {
                        return new BotExecuteSqlOnSqlFileResponse()
                        {
                            Packet = new Packet() {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(modPermis).Reference(mp => mp.Bot).Load();
                    user = modPermis.Bot;
                }

                var bot = (Bot) user;

                var file = dbContext.Files.Find(packet.File.FileId);

                if (file.UploaderId != bot.BaseUserId)
                {
                    return new BotExecuteSqlOnSqlFileResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                var dbConnection =
                    new SqliteConnection("Data Source=" + Path.Combine(_dirPath, file.FileId.ToString()) + ";");
                dbConnection.Open();
                var command = new SqliteCommand(packet.SqlCommand.SqlScript, dbConnection);
                Packet res = null;
                try
                {
                    if (packet.SqlCommand.IsQuery && !packet.SqlCommand.MustReturnId)
                    {
                        var sqliteDataReader = command.ExecuteReader();
                        var r = ConvertSqliteDataReaderToDict(sqliteDataReader);
                        var result = JsonConvert.SerializeObject(r);
                        res = new Packet()
                        {
                            Status = "success",
                            SqlResult = new SqlResult() {QueryResultJson = result}
                        };
                    }
                    else if (!packet.SqlCommand.IsQuery && !packet.SqlCommand.MustReturnId)
                    {
                        var result = command.ExecuteNonQuery();
                        res = new Packet()
                        {
                            Status = "success",
                            SqlResult = new SqlResult() {NonQueryResultNumber = result}
                        };
                    }
                    else if (packet.SqlCommand.MustReturnId)
                    {
                        var result = Convert.ToInt64(command.ExecuteScalar());
                        res = new Packet()
                        {
                            Status = "success",
                            SqlResult = new SqlResult() {ScalarResultNumber = result}
                        };
                    }
                }
                catch (Exception ex)
                {
                    res = new Packet()
                    {
                        Status = "error_3",
                        SqlResult = new SqlResult() {ErrorMessage = ex.ToString()}
                    };
                }
                finally
                {
                    dbConnection.Close();
                }

                return new BotExecuteSqlOnSqlFileResponse()
                {
                    Packet = res
                };
            }
        }

        private static IEnumerable<Dictionary<string, object>> ConvertSqliteDataReaderToDict(SqliteDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(BuildSqliteRowInDict(cols, reader));

            return results;
        }

        private static Dictionary<string, object> BuildSqliteRowInDict(IEnumerable<string> cols,
            SqliteDataReader reader)
        {
            return cols.ToDictionary(col => col, col => reader[col]);
        }

        public BotExecuteMongoComOnMongoFileResponse AnswerQuestion(
            AnswerContext<BotExecuteMongoComOnMongoFileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = session.BaseUser;

                if (user is Module mod)
                {
                    dbContext.Entry(mod).Collection(m => m.ModulePermissions).Load();
                    var modPermis = mod.ModulePermissions.Find(mp => mp.BotId == packet.Bot.BaseUserId);
                    if (modPermis == null)
                    {
                        return new BotExecuteMongoComOnMongoFileResponse()
                        {
                            Packet = new Packet() {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(modPermis).Reference(mp => mp.Bot).Load();
                    user = modPermis.Bot;
                }

                var bot = (Bot) user;

                var file = dbContext.Files.Find(packet.File.FileId);

                if (file.UploaderId != bot.BaseUserId)
                {
                    return new BotExecuteMongoComOnMongoFileResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                return new BotExecuteMongoComOnMongoFileResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public NotifyMessageSeenResponse AnswerQuestion(AnswerContext<NotifyMessageSeenRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                var message = dbContext.Messages.Find(context.Question.Packet.Message.MessageId);
                if (message == null)
                {
                    return new NotifyMessageSeenResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                var messageSeen = dbContext.MessageSeens.Find(user.BaseUserId + "_" + message.MessageId);
                if (messageSeen != null)
                {
                    return new NotifyMessageSeenResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                dbContext.Entry(message).Reference(m => m.Room).Load();
                var room = message.Room;
                dbContext.Entry(room).Reference(r => r.Complex).Load();
                var complex = room.Complex;
                dbContext.Entry(complex).Collection(c => c.Members).Load();
                if (complex.Members.Any(m => m.UserId == user.BaseUserId))
                {
                    if (message.AuthorId == user.BaseUserId)
                    {
                        return new NotifyMessageSeenResponse()
                        {
                            Packet = new Packet() {Status = "error_5"}
                        };
                    }
                    
                    messageSeen = new MessageSeen()
                    {
                        MessageSeenId = user.BaseUserId + "_" + message.MessageId,
                        Message = message,
                        User = user
                    };
                    dbContext.MessageSeens.Add(messageSeen);

                    dbContext.SaveChanges();
                    
                    var kt = new KafkaTransport();


                    var notif = new MessageSeenNotification()
                    {
                        MessageId = message.MessageId,
                        MessageSeenCount =
                            dbContext.MessageSeens.LongCount(ms => ms.MessageId == message.MessageId)
                    };
                    dbContext.Entry(complex)
                        .Collection(c => c.Members).Query()
                        .Include(m => m.User)
                        .ThenInclude(u => u.Sessions)
                        .Load();
                    var push = new MessageSeenPush()
                    {
                        Notif = notif,
                        SessionIds = (from m in complex.Members
                            where m.User.BaseUserId != user.BaseUserId
                            from s in m.User.Sessions
                            select s.SessionId).ToList()
                    };

                    kt.PushNotifToApiGateway(push);

                    return new NotifyMessageSeenResponse()
                    {
                        Packet = new Packet() {Status = "success"}
                    };
                }

                return new NotifyMessageSeenResponse()
                {
                    Packet = new Packet() {Status = "error_4"}
                };
            }
        }

        public GetMessageSeenCountResponse AnswerQuestion(AnswerContext<GetMessageSeenCountRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                var message = dbContext.Messages.Find(context.Question.Packet.Message.MessageId);
                if (message == null)
                {
                    return new GetMessageSeenCountResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(message).Reference(m => m.Room).Load();
                var room = message.Room;
                dbContext.Entry(room).Reference(r => r.Complex).Load();
                var complex = room.Complex;
                dbContext.Entry(complex).Collection(c => c.Members).Load();
                if (complex.Members.Any(m => m.UserId == user.BaseUserId))
                {
                    var seenCount = dbContext.MessageSeens.LongCount(ms => ms.MessageId == message.MessageId);

                    return new GetMessageSeenCountResponse()
                    {
                        Packet = new Packet() {Status = "success", MessageSeenCount = seenCount}
                    };
                }

                return new GetMessageSeenCountResponse()
                {
                    Packet = new Packet() {Status = "error_3"}
                };
            }
        }

        public GetLastActionsResponse AnswerQuestion(AnswerContext<GetLastActionsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var rooms = context.Question.Packet.BaseRooms;

                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();

                foreach (var roomC in rooms)
                {
                    var membership = user.Memberships.Find(mem => mem.ComplexId == roomC.ComplexId);
                    if (membership == null)
                    {
                        return new GetLastActionsResponse()
                        {
                            Packet = new Packet() {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(membership).Reference(m => m.Complex).Load();
                    var complex = membership.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    dbContext.Entry(complex).Collection(c => c.SingleRooms).Load();
                    var room = complex.Rooms.Find(r => r.RoomId == roomC.RoomId);
                    var sroom = complex.SingleRooms.Find(sr => sr.RoomId == roomC.RoomId);
                    if (room == null && sroom == null)
                    {
                        return new GetLastActionsResponse()
                        {
                            Packet = new Packet() {Status = "error_2"}
                        };
                    }

                    roomC.LastAction = dbContext.Messages.Last(m => m.RoomId == room.RoomId);

                    if (roomC.LastAction == null) continue;

                    dbContext.Entry(roomC.LastAction).Reference(m => m.Author).Load();

                    if (roomC.LastAction.GetType() == typeof(PhotoMessage))
                    {
                        dbContext.Entry(roomC.LastAction).Reference(m => ((PhotoMessage) m).Photo).Load();
                        dbContext.Entry(((PhotoMessage) roomC.LastAction).Photo).Collection(f => f.FileUsages)
                            .Query().Where(fu => fu.RoomId == roomC.RoomId).Load();
                    }
                    else if (roomC.LastAction.GetType() == typeof(AudioMessage))
                    {
                        dbContext.Entry(roomC.LastAction).Reference(m => ((AudioMessage) m).Audio).Load();
                        dbContext.Entry(((AudioMessage) roomC.LastAction).Audio).Collection(f => f.FileUsages)
                            .Query().Where(fu => fu.RoomId == roomC.RoomId).Load();
                    }
                    else if (roomC.LastAction.GetType() == typeof(VideoMessage))
                    {
                        dbContext.Entry(roomC.LastAction).Reference(m => ((VideoMessage) m).Video).Load();
                        dbContext.Entry(((VideoMessage) roomC.LastAction).Video).Collection(f => f.FileUsages)
                            .Query().Where(fu => fu.RoomId == roomC.RoomId).Load();
                    }

                    roomC.LastAction.SeenByMe =
                        (dbContext.MessageSeens.Find(user.BaseUserId + "_" + roomC.LastAction.MessageId) != null);
                    roomC.LastAction.SeenCount =
                        dbContext.MessageSeens.LongCount(ms => ms.MessageId == roomC.LastAction.MessageId);
                }

                return new GetLastActionsResponse()
                {
                    Packet = new Packet() {Status = "success", BaseRooms = rooms}
                };
            }
        }

        public GetMessagesResponse AnswerQuestion(AnswerContext<GetMessagesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                if (packet.FetchNext == null)
                {
                    return new GetMessagesResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new GetMessagesResponse()
                    {
                        Packet = new Packet {Status = "error_0U1"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new GetMessagesResponse()
                    {
                        Packet = new Packet {Status = "error_0U2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Messages).Load();

                List<Message> messages;

                if (packet.Message.MessageId > 0)
                {
                    messages = packet.FetchNext.Value
                        ? room.Messages.Where(m =>
                                m.MessageId > packet.Message.MessageId && m.MessageId - packet.Message.MessageId <= 100)
                            .ToList()
                        : room.Messages.Where(m =>
                                m.MessageId < packet.Message.MessageId && packet.Message.MessageId - m.MessageId <= 100)
                            .ToList();
                }
                else
                {
                    messages = room.Messages.TakeLast(100).ToList();
                }

                foreach (var msg in messages)
                {
                    if (msg.GetType() == typeof(PhotoMessage))
                    {
                        dbContext.Entry(msg).Reference(m => ((PhotoMessage) m).Photo).Load();
                        dbContext.Entry(((PhotoMessage) msg).Photo).Collection(f => f.FileUsages).Load();
                    }
                    else if (msg.GetType() == typeof(AudioMessage))
                    {
                        dbContext.Entry(msg).Reference(m => ((AudioMessage) m).Audio).Load();
                        dbContext.Entry(((AudioMessage) msg).Audio).Collection(f => f.FileUsages).Load();
                    }
                    else if (msg.GetType() == typeof(VideoMessage))
                    {
                        dbContext.Entry(msg).Reference(m => ((VideoMessage) m).Video).Load();
                        dbContext.Entry(((VideoMessage) msg).Video).Collection(f => f.FileUsages).Load();
                    }

                    msg.SeenByMe = (dbContext.MessageSeens.Find(user.BaseUserId + "_" + msg.MessageId) != null);
                }

                return new GetMessagesResponse()
                {
                    Packet = new Packet {Status = "success", Messages = messages}
                };
            }
        }

        public async Task<CreateTextMessageResponse> AnswerQuestion(AnswerContext<CreateTextMessageRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = (User) session.BaseUser;
                dbContext.Entry(human).Collection(h => h.Memberships).Load();
                var membership = human.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new CreateTextMessageResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new CreateTextMessageResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    };
                }

                var kt = new KafkaTransport();
                var id = await kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(Message).Name
                    });
                
                var message = new TextMessage()
                {
                    MessageId = (long)id.Index,
                    Author = human,
                    Room = room,
                    Text = packet.TextMessage.Text,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                dbContext.Messages.Add(message);
                dbContext.SaveChanges();
                
                
                dbContext.Entry(human).Collection(h => h.Sessions).Load();
                dbContext.Entry(complex)
                    .Collection(c => c.Members)
                    .Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions)
                    .Load();
                dbContext.Entry(room).Collection(r => r.Workers).Load();
                TextMessage nextMessage;
                using (var nextContext = new DatabaseContext())
                {
                    nextMessage = (TextMessage) nextContext.Messages.Find(message.MessageId);
                    nextContext.Entry(nextMessage).Reference(m => m.Room).Load();
                    nextContext.Entry(nextMessage.Room).Reference(r => r.Complex).Load();
                    nextContext.Entry(nextMessage).Reference(m => m.Author).Load();
                }

                List<long> sessionIds;
                if (room is SingleRoom sr)
                {
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                    sessionIds = new List<long>();
                    if (sr.User1.BaseUserId != human.BaseUserId)
                        sessionIds.AddRange(from s in sr.User1.Sessions select s.SessionId);
                    if (sr.User2.BaseUserId != human.BaseUserId)
                        sessionIds.AddRange(from s in sr.User2.Sessions select s.SessionId);
                    sessionIds.AddRange((from w in room.Workers
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }
                else
                {
                    sessionIds = (from m in complex.Members
                        where m.User.BaseUserId != human.BaseUserId
                        from s in m.User.Sessions
                        select s.SessionId).ToList();
                    sessionIds.AddRange((from w in room.Workers
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }

                var mcn = new TextMessageNotification()
                {
                    Message = nextMessage
                };
                
                kt.PushNotifToApiGateway(new TextMessagePush()
                {
                    Notif = mcn,
                    SessionIds = sessionIds
                });

                return new CreateTextMessageResponse()
                {
                    Packet = new Packet {Status = "success", TextMessage = nextMessage}
                };
            }
        }

        public async Task<CreateFileMessageResponse> AnswerQuestion(AnswerContext<CreateFileMessageRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var human = (User) session.BaseUser;
                dbContext.Entry(human).Collection(h => h.Memberships).Load();
                var membership = human.Memberships.Find(mem => mem.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new CreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(mem => mem.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new CreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    };
                }

                Message message = null;
                dbContext.Entry(room).Collection(r => r.Files).Load();
                var fileUsage = room.Files.Find(fu => fu.FileId == packet.File.FileId);
                if (fileUsage == null)
                {
                    return new CreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }

                dbContext.Entry(fileUsage).Reference(fu => fu.File).Load();
                var file = fileUsage.File;
                
                var kt = new KafkaTransport();
                
                switch (file)
                {
                    case Photo photo:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new PhotoMessage()
                        {
                            MessageId = (long)id.Index,
                            Author = human,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Photo = photo
                        };
                        break;
                    }

                    case Audio audio:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new AudioMessage()
                        {
                            MessageId = (long)id.Index,
                            Author = human,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Audio = audio
                        };
                        break;
                    }

                    case Video video:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new VideoMessage()
                        {
                            MessageId = (long)id.Index,
                            Author = human,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Video = video
                        };
                        break;
                    }
                }

                if (message == null)
                {
                    return new CreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_4"}
                    };
                }

                dbContext.Messages.Add(message);
                dbContext.SaveChanges();

                Message nextMessage;
                using (var nextContext = new DatabaseContext())
                {
                    nextMessage = nextContext.Messages.Find(message.MessageId);
                    nextContext.Entry(nextMessage).Reference(m => m.Room).Load();
                    nextContext.Entry(nextMessage.Room).Reference(r => r.Complex).Load();
                    nextContext.Entry(nextMessage).Reference(m => m.Author).Load();
                    switch (nextMessage)
                    {
                        case PhotoMessage photoMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((PhotoMessage) msg).Photo).Load();
                            nextContext.Entry(photoMessage.Photo).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                        case AudioMessage audioMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((AudioMessage) msg).Audio).Load();
                            nextContext.Entry(audioMessage.Audio).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                        case VideoMessage videoMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((VideoMessage) msg).Video).Load();
                            nextContext.Entry(videoMessage.Video).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                    }
                }

                dbContext.Entry(complex)
                    .Collection(c => c.Members)
                    .Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions)
                    .Load();
                dbContext.Entry(room).Collection(r => r.Workers).Load();

                List<long> sessionIds;
                if (room is SingleRoom sr)
                {
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                    sessionIds = new List<long>();
                    if (sr.User1.BaseUserId != human.BaseUserId)
                        sessionIds.AddRange(from s in sr.User1.Sessions select s.SessionId);
                    if (sr.User2.BaseUserId != human.BaseUserId)
                        sessionIds.AddRange(from s in sr.User2.Sessions select s.SessionId);
                    sessionIds.AddRange((from w in room.Workers
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }
                else
                {
                    sessionIds = (from m in complex.Members
                        where m.User.BaseUserId != human.BaseUserId
                        from s in m.User.Sessions
                        select s.SessionId).ToList();
                    sessionIds.AddRange((from w in room.Workers
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }

                switch (nextMessage)
                {
                    case PhotoMessage msg:
                    {
                        var notif = new PhotoMessageNotification()
                        {
                            Message = msg,
                        };

                        kt.PushNotifToApiGateway(new PhotoMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }

                    case AudioMessage msg:
                    {
                        var notif = new AudioMessageNotification()
                        {
                            Message = msg
                        };

                        kt.PushNotifToApiGateway(new AudioMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }

                    case VideoMessage msg:
                    {
                        var notif = new VideoMessageNotification()
                        {
                            Message = msg
                        };

                        kt.PushNotifToApiGateway(new VideoMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }
                }

                switch (nextMessage)
                {
                    case PhotoMessage msg:
                        return new CreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", PhotoMessage = msg}
                        };
                    case AudioMessage msg:
                        return new CreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", AudioMessage = msg}
                        };
                    case VideoMessage msg:
                        return new CreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", VideoMessage = msg}
                        };
                    default:
                        return new CreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "error_5"}
                        };
                }
            }
        }

        public async Task<BotCreateTextMessageResponse> AnswerQuestion(AnswerContext<BotCreateTextMessageRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;
                dbContext.Entry(bot).Reference(b => b.BotSecret).Load();
                var complex = dbContext.Complexes.Find(packet.Complex.ComplexId);
                if (complex == null)
                {
                    return new BotCreateTextMessageResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new BotCreateTextMessageResponse()
                    {
                        Packet = new Packet {Status = "error_4"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workership = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (workership == null)
                {
                    return new BotCreateTextMessageResponse()
                    {
                        Packet = new Packet {Status = "error_5"}
                    };
                }

                var kt = new KafkaTransport();
                var id = await kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(Message).Name
                    });
                
                var message = new TextMessage()
                {
                    MessageId = (long)id.Index,
                    Author = bot,
                    Room = room,
                    Text = packet.TextMessage.Text,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                dbContext.Messages.Add(message);
                dbContext.SaveChanges();

                TextMessage nextMessage;
                using (var nextContext = new DatabaseContext())
                {
                    nextMessage = (TextMessage) nextContext.Messages.Find(message.MessageId);
                    nextContext.Entry(nextMessage).Reference(m => m.Room).Load();
                    nextContext.Entry(nextMessage.Room).Reference(r => r.Complex).Load();
                    nextContext.Entry(nextMessage).Reference(m => m.Author).Load();
                }

                dbContext.Entry(complex)
                    .Collection(c => c.Members)
                    .Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions)
                    .Load();
                dbContext.Entry(room).Collection(r => r.Workers).Load();

                List<long> sessionIds;
                if (room is SingleRoom sr)
                {
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                    sessionIds = new List<long>();
                    sessionIds.AddRange(from s in sr.User1.Sessions select s.SessionId);
                    sessionIds.AddRange(from s in sr.User2.Sessions select s.SessionId);
                    sessionIds.AddRange((from w in room.Workers
                        where w.BotId != bot.BaseUserId
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }
                else
                {
                    sessionIds = (from m in complex.Members
                        from s in m.User.Sessions
                        select s.SessionId).ToList();
                    sessionIds.AddRange((from w in room.Workers
                        where w.BotId != bot.BaseUserId
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }

                var mcn = new TextMessageNotification()
                {
                    Message = nextMessage
                };

                kt.PushNotifToApiGateway(new TextMessagePush()
                {
                    Notif = mcn,
                    SessionIds = sessionIds
                });

                return new BotCreateTextMessageResponse()
                {
                    Packet = new Packet {Status = "success", TextMessage = nextMessage}
                };
            }
        }

        public async Task<BotCreateFileMessageResponse> AnswerQuestion(AnswerContext<BotCreateFileMessageRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;
                dbContext.Entry(bot).Reference(b => b.BotSecret).Load();
                var complex = dbContext.Complexes.Find(packet.Complex.ComplexId);
                if (complex == null)
                {
                    return new BotCreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new BotCreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_4"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workership = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (workership == null)
                {
                    return new BotCreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_5"}
                    };
                }

                Message message = null;
                dbContext.Entry(room).Collection(r => r.Files).Load();
                var fileUsage = room.Files.Find(fu => fu.FileId == packet.File.FileId);
                if (fileUsage == null)
                {
                    return new BotCreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_6"}
                    };
                }

                dbContext.Entry(fileUsage).Reference(fu => fu.File).Load();
                var file = fileUsage.File;

                var kt = new KafkaTransport();

                switch (file)
                {
                    case Photo photo:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new PhotoMessage()
                        {
                            MessageId = (long)id.Index,
                            Author = bot,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Photo = photo
                        };
                        break;
                    }

                    case Audio audio:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new AudioMessage()
                        {
                            MessageId = (long) id.Index,
                            Author = bot,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Audio = audio
                        };
                        break;
                    }
                    case Video video:
                    {
                        var id = await kt.AskForNewId(new AskIndexEntity()
                            {
                                EntityType = typeof(Message).Name
                            });
                        message = new VideoMessage()
                        {
                            MessageId = (long) id.Index,
                            Author = bot,
                            Room = room,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Video = video
                        };
                        break;
                    }
                }

                if (message == null)
                {
                    return new BotCreateFileMessageResponse()
                    {
                        Packet = new Packet {Status = "error_7"}
                    };
                }

                dbContext.Messages.Add(message);
                dbContext.SaveChanges();

                Message nextMessage;
                using (var nextContext = new DatabaseContext())
                {
                    nextMessage = nextContext.Messages.Find(message.MessageId);
                    nextContext.Entry(nextMessage).Reference(msg => msg.Room).Load();
                    nextContext.Entry(nextMessage.Room).Reference(r => r.Complex).Load();
                    nextContext.Entry(nextMessage).Reference(msg => msg.Author).Load();
                    switch (nextMessage)
                    {
                        case PhotoMessage photoMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((PhotoMessage) msg).Photo).Load();
                            nextContext.Entry(photoMessage.Photo).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                        case AudioMessage audioMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((AudioMessage) msg).Audio).Load();
                            nextContext.Entry(audioMessage.Audio).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                        case VideoMessage videoMessage:
                            nextContext.Entry(nextMessage).Reference(msg => ((VideoMessage) msg).Video).Load();
                            nextContext.Entry(videoMessage.Video).Collection(f => f.FileUsages)
                                .Query().Where(fu => fu.FileUsageId == fileUsage.FileUsageId).Load();
                            break;
                    }
                }

                dbContext.Entry(complex)
                    .Collection(c => c.Members)
                    .Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions)
                    .Load();
                dbContext.Entry(room).Collection(r => r.Workers).Load();

                List<long> sessionIds;
                if (room is SingleRoom sr)
                {
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                    sessionIds = new List<long>();
                    sessionIds.AddRange(from s in sr.User1.Sessions select s.SessionId);
                    sessionIds.AddRange(from s in sr.User2.Sessions select s.SessionId);
                    sessionIds.AddRange((from w in room.Workers
                        where w.BotId != bot.BaseUserId
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }
                else
                {
                    sessionIds = (from m in complex.Members
                        from s in m.User.Sessions
                        select s.SessionId).ToList();
                    sessionIds.AddRange((from w in room.Workers
                        where w.BotId != bot.BaseUserId
                        from s in dbContext.Bots.Include(b => b.Sessions)
                            .FirstOrDefault(b => b.BaseUserId == w.BotId)
                            ?.Sessions
                        select s.SessionId).ToList());
                }

                switch (nextMessage)
                {
                    case PhotoMessage msg:
                    {
                        var notif = new PhotoMessageNotification()
                        {
                            Message = msg
                        };

                        kt.PushNotifToApiGateway(new PhotoMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }

                    case AudioMessage msg:
                    {
                        var notif = new AudioMessageNotification()
                        {
                            Message = msg
                        };

                        kt.PushNotifToApiGateway(new AudioMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }

                    case VideoMessage msg:
                    {
                        var notif = new VideoMessageNotification()
                        {
                            Message = msg
                        };

                        kt.PushNotifToApiGateway(new VideoMessagePush()
                        {
                            Notif = notif,
                            SessionIds = sessionIds
                        });

                        break;
                    }
                }

                switch (nextMessage)
                {
                    case PhotoMessage msg:
                        return new BotCreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", PhotoMessage = msg}
                        };
                    case AudioMessage msg:
                        return new BotCreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", AudioMessage = msg}
                        };
                    case VideoMessage msg:
                        return new BotCreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "success", VideoMessage = msg}
                        };
                    default:
                        return new BotCreateFileMessageResponse()
                        {
                            Packet = new Packet {Status = "error_5"}
                        };
                }
            }
        }

        public PutServiceMessageResponse AnswerQuestion(AnswerContext<PutServiceMessageRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var message = context.Question.Packet.ServiceMessage;

                var room = dbContext.BaseRooms.Find(message.Room.RoomId);

                message.Room = room;

                dbContext.Messages.Add(message);

                message = (ServiceMessage) dbContext.Messages.Find(message.MessageId);

                dbContext.SaveChanges();

                return new PutServiceMessageResponse()
                {
                    Packet = new Packet() {ServiceMessage = message}
                };
            }
        }

        public RequestBotViewResponse AnswerQuestion(AnswerContext<RequestBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                var complexId = packet.Complex.ComplexId;
                var roomId = packet.BaseRoom.RoomId;
                var botId = packet.Bot.BaseUserId;
                var isWindow = packet.BotWindowMode;

                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var mem = user.Memberships.Find(m => m.ComplexId == complexId);
                if (mem == null)
                {
                    return new RequestBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(mem).Reference(m => m.Complex).Load();
                var complex = mem.Complex;
                if (complex == null)
                {
                    return new RequestBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                BaseRoom room = complex.Rooms.Find(r => r.RoomId == roomId);
                if (room == null)
                {
                    dbContext.Entry(complex).Collection(c => c.SingleRooms).Load();
                    room = complex.SingleRooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                    if (room == null)
                    {
                        return new RequestBotViewResponse()
                        {
                            Packet = new Packet() {Status = "error_3"}
                        };
                    }
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == botId);
                if (worker == null)
                {
                    return new RequestBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_4"}
                    };
                }

                var bot = dbContext.Bots.Find(botId);
                dbContext.Entry(bot).Collection(b => b.Sessions).Load();

                User finalUser;
                using (var finalContext = new DatabaseContext())
                {
                    finalUser = (User) finalContext.BaseUsers.Find(user.BaseUserId);
                }

                var notif = new UserRequestedBotViewNotification()
                {
                    ComplexId = complexId,
                    RoomId = roomId,
                    BotId = botId,
                    User = finalUser,
                    WorkerWidth = isWindow ? packet.Workership.Width : worker.Width,
                    WorkerHeight = isWindow ? packet.Workership.Height : worker.Height,
                    BotWindowMode = isWindow
                };

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new UserRequestedBotViewPush()
                {
                    Notif = notif,
                    SessionIds = new List<long> {bot.Sessions.First().SessionId}
                });

                return new RequestBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }
        
        public AnswerRequestBotPreview AnswerQuestion(AnswerContext<AskRequestBotPreview> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                var botId = packet.Bot.BaseUserId;
                var semiWorker = packet.Workership;

                var bot = dbContext.Bots.Find(botId);
                if (bot == null)
                {
                    return new AnswerRequestBotPreview()
                    {
                        Packet = new Packet() {Status = "e10"}
                    };
                }
                dbContext.Entry(bot).Collection(b => b.Sessions).Load();

                User finalUser;
                using (var finalContext = new DatabaseContext())
                {
                    finalUser = (User) finalContext.BaseUsers.Find(user.BaseUserId);
                }

                var notif = new UserRequestedBotPreviewNotification()
                {
                    BotId = botId,
                    User = finalUser,
                    WorkerWidth = semiWorker.Width,
                    WorkerHeight = semiWorker.Height
                };

                pendingPreviews[user.BaseUserId + "-" + bot.BaseUserId] = true;

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new UserRequestedBotPreviewPush()
                {
                    Notif = notif,
                    SessionIds = new List<long> {bot.Sessions.First().SessionId}
                });

                return new AnswerRequestBotPreview()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public SendBotViewResponse AnswerQuestion(AnswerContext<SendBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complexId = packet.Complex.ComplexId;
                var roomId = packet.BaseRoom.RoomId;
                var userId = packet.User.BaseUserId;
                var viewData = packet.RawJson;
                var isWindow = packet.BotWindowMode;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                var kt = new KafkaTransport();
                
                if (complexId == 0 && roomId == 0)
                {
                    if (pendingPreviews.ContainsKey(userId + "-" + bot.BaseUserId) &&
                        pendingPreviews[userId + "-" + bot.BaseUserId])
                    {
                        pendingPreviews[userId + "-" + bot.BaseUserId] = false;
                        
                        var n = new BotSentBotViewNotification()
                        {
                            ComplexId = complexId,
                            RoomId = roomId,
                            BotId = bot.BaseUserId,
                            ViewData = viewData,
                            BotWindowMode = isWindow
                        };

                        var u = (User)dbContext.BaseUsers.Find(userId);
                        dbContext.Entry(u).Collection(us => us.Sessions).Load();

                        var sIds = u.Sessions.Select(s => s.SessionId).ToList();
                        kt.PushNotifToApiGateway(new BotSentBotViewPush()
                        {
                            SessionIds = sIds,
                            Notif = n
                        });

                        return new SendBotViewResponse()
                        {
                            Packet = new Packet() {Status = "success"}
                        };
                    }
                }

                var complex = dbContext.Complexes.Find(complexId);
                if (complex == null)
                {
                    return new SendBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new SendBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (worker == null)
                {
                    return new SendBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                SingleRoom sr = null;
                User user = null;
                if (room is SingleRoom siro)
                {
                    sr = siro;
                    dbContext.Entry(sr).Reference(singleRoom => singleRoom.User1).Query().Include(u => u.Sessions)
                        .Load();
                    dbContext.Entry(sr).Reference(singleRoom => singleRoom.User2).Query().Include(u => u.Sessions)
                        .Load();
                }
                else
                {
                    if (userId > 0)
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Load();
                        user = (User) dbContext.BaseUsers.Find(userId);
                        dbContext.Entry(user).Collection(u => u.Sessions).Load();
                        var membership = complex.Members.Find(m => m.UserId == user.BaseUserId);
                        if (membership == null)
                        {
                            return new SendBotViewResponse()
                            {
                                Packet = new Packet() {Status = "error_4"}
                            };
                        }
                    }
                    else
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Query()
                            .Include(m => m.User).ThenInclude(u => u.Sessions).Load();
                    }
                }

                var notif = new BotSentBotViewNotification()
                {
                    ComplexId = complexId,
                    RoomId = roomId,
                    BotId = bot.BaseUserId,
                    ViewData = viewData,
                    BotWindowMode = isWindow
                };

                var sessionIds = new List<long>();
                if (sr == null)
                {
                    if (user == null)
                        sessionIds = (from m in complex.Members
                            from s in m.User.Sessions
                            select s.SessionId).ToList();
                    else
                        sessionIds = user.Sessions.Select(s => s.SessionId).ToList();
                }
                else
                {
                    sessionIds.AddRange((from s in sr.User1.Sessions select s.SessionId).ToList());
                    sessionIds.AddRange((from s in sr.User2.Sessions select s.SessionId).ToList());
                }

                kt.PushNotifToApiGateway(new BotSentBotViewPush()
                {
                    SessionIds = sessionIds,
                    Notif = notif
                });

                return new SendBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public UpdateBotViewResponse AnswerQuestion(AnswerContext<UpdateBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complexId = packet.Complex.ComplexId;
                var roomId = packet.BaseRoom.RoomId;
                var userId = packet.User.BaseUserId;
                var viewData = packet.RawJson;
                var batchData = packet.BatchData;
                var isWindow = packet.BotWindowMode;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                var complex = dbContext.Complexes.Find(complexId);
                if (complex == null)
                {
                    return new UpdateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new UpdateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (worker == null)
                {
                    return new UpdateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                SingleRoom sr = null;
                User user = null;
                if (room is SingleRoom singleRoom)
                {
                    sr = singleRoom;
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                }
                else
                {
                    if (userId > 0)
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Load();
                        user = (User) dbContext.BaseUsers.Find(userId);
                        dbContext.Entry(user).Collection(u => u.Sessions).Load();
                        var membership = complex.Members.Find(m => m.UserId == user.BaseUserId);
                        if (membership == null)
                        {
                            return new UpdateBotViewResponse()
                            {
                                Packet = new Packet() {Status = "error_4"}
                            };
                        }
                    }
                    else
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Query()
                            .Include(m => m.User).ThenInclude(u => u.Sessions).Load();
                    }
                }

                var notif = new BotUpdatedBotViewNotification()
                {
                    ComplexId = complexId,
                    RoomId = roomId,
                    BotId = bot.BaseUserId,
                    UpdateData = viewData,
                    BatchData = batchData ?? false,
                    BotWindowMode = isWindow
                };

                var sessionIds = new List<long>();
                if (sr == null)
                {
                    if (user == null)
                        sessionIds = (from m in complex.Members
                            from s in m.User.Sessions
                            select s.SessionId).ToList();
                    else
                        sessionIds = user.Sessions.Select(s => s.SessionId).ToList();
                }
                else
                {
                    sessionIds.AddRange((from s in sr.User1.Sessions select s.SessionId).ToList());
                    sessionIds.AddRange((from s in sr.User2.Sessions select s.SessionId).ToList());
                }

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new BotUpdatedBotViewPush()
                {
                    SessionIds = sessionIds,
                    Notif = notif
                });

                return new UpdateBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public AnimateBotViewResponse AnswerQuestion(AnswerContext<AnimateBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complexId = packet.Complex.ComplexId;
                var roomId = packet.BaseRoom.RoomId;
                var userId = packet.User.BaseUserId;
                var viewData = packet.RawJson;
                var batchData = packet.BatchData;
                var isWindow = packet.BotWindowMode;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                var complex = dbContext.Complexes.Find(complexId);
                if (complex == null)
                {
                    return new AnimateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new AnimateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (worker == null)
                {
                    return new AnimateBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                SingleRoom sr = null;
                User user = null;
                if (room is SingleRoom singleRoom)
                {
                    sr = singleRoom;
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                }
                else
                {
                    if (userId > 0)
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Load();
                        user = (User) dbContext.BaseUsers.Find(userId);
                        dbContext.Entry(user).Collection(u => u.Sessions).Load();
                        var membership = complex.Members.Find(m => m.UserId == user.BaseUserId);
                        if (membership == null)
                        {
                            return new AnimateBotViewResponse()
                            {
                                Packet = new Packet() {Status = "error_4"}
                            };
                        }
                    }
                    else
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Query()
                            .Include(m => m.User).ThenInclude(u => u.Sessions).Load();
                    }
                }

                var notif = new BotAnimatedBotViewNotification()
                {
                    ComplexId = complexId,
                    RoomId = roomId,
                    BotId = bot.BaseUserId,
                    AnimData = viewData,
                    BatchData = batchData ?? false,
                    BotWindowMode = isWindow
                };

                var sessionIds = new List<long>();
                if (sr == null)
                {
                    if (user == null)
                        sessionIds = (from m in complex.Members
                            from s in m.User.Sessions
                            select s.SessionId).ToList();
                    else
                        sessionIds = user.Sessions.Select(s => s.SessionId).ToList();
                }
                else
                {
                    sessionIds.AddRange((from s in sr.User1.Sessions select s.SessionId).ToList());
                    sessionIds.AddRange((from s in sr.User2.Sessions select s.SessionId).ToList());
                }

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new BotAnimatedBotViewPush()
                {
                    SessionIds = sessionIds,
                    Notif = notif
                });

                return new AnimateBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public RunCommandsOnBotViewResponse AnswerQuestion(AnswerContext<RunCommandsOnBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var complexId = packet.Complex.ComplexId;
                var roomId = packet.BaseRoom.RoomId;
                var userId = packet.User.BaseUserId;
                var viewData = packet.RawJson;
                var batchData = packet.BatchData;
                var isWindow = packet.BotWindowMode;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                var complex = dbContext.Complexes.Find(complexId);
                if (complex == null)
                {
                    return new RunCommandsOnBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = dbContext.BaseRooms.Find(packet.BaseRoom.RoomId);

                if (room == null || room.ComplexId != complex.ComplexId)
                {
                    return new RunCommandsOnBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == bot.BaseUserId);
                if (worker == null)
                {
                    return new RunCommandsOnBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                SingleRoom sr = null;
                User user = null;
                if (room is SingleRoom singleRoom)
                {
                    sr = singleRoom;
                    dbContext.Entry(sr).Reference(siro => siro.User1).Query().Include(u => u.Sessions).Load();
                    dbContext.Entry(sr).Reference(siro => siro.User2).Query().Include(u => u.Sessions).Load();
                }
                else
                {
                    if (userId > 0)
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Load();
                        user = (User) dbContext.BaseUsers.Find(userId);
                        dbContext.Entry(user).Collection(u => u.Sessions).Load();
                        var membership = complex.Members.Find(m => m.UserId == user.BaseUserId);
                        if (membership == null)
                        {
                            return new RunCommandsOnBotViewResponse()
                            {
                                Packet = new Packet() {Status = "error_4"}
                            };
                        }
                    }
                    else
                    {
                        dbContext.Entry(complex).Collection(c => c.Members).Query()
                            .Include(m => m.User).ThenInclude(u => u.Sessions).Load();
                    }
                }

                var notif = new BotRanCommandsOnBotViewNotification()
                {
                    ComplexId = complexId,
                    RoomId = roomId,
                    BotId = bot.BaseUserId,
                    CommandsData = viewData,
                    BatchData = batchData ?? false,
                    BotWindowMode = isWindow
                };

                var sessionIds = new List<long>();
                if (sr == null)
                {
                    if (user == null)
                        sessionIds = (from m in complex.Members
                            from s in m.User.Sessions
                            select s.SessionId).ToList();
                    else
                        sessionIds = user.Sessions.Select(s => s.SessionId).ToList();
                }
                else
                {
                    sessionIds.AddRange((from s in sr.User1.Sessions select s.SessionId).ToList());
                    sessionIds.AddRange((from s in sr.User2.Sessions select s.SessionId).ToList());
                }

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new BotRanCommandsOnBotViewPush()
                {
                    SessionIds = sessionIds,
                    Notif = notif
                });

                return new RunCommandsOnBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public ClickBotViewResponse AnswerQuestion(AnswerContext<ClickBotViewRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                var membership = dbContext.Memberships.FirstOrDefault(mem =>
                    mem.ComplexId == packet.Complex.ComplexId && mem.UserId == user.BaseUserId);
                if (membership == null)
                {
                    return new ClickBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(mem => mem.Complex).Load();
                var room = dbContext.BaseRooms.FirstOrDefault(r =>
                    r.ComplexId == packet.Complex.ComplexId && r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new ClickBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                var workership = dbContext.Workerships.FirstOrDefault(w =>
                    w.RoomId == packet.BaseRoom.RoomId && w.BotId == packet.Bot.BaseUserId);
                if (workership == null)
                {
                    return new ClickBotViewResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                var notif = new UserClickedBotViewNotification()
                {
                    Complex = membership.Complex,
                    Room = room,
                    User = user,
                    ControlId = packet.ControlId
                };

                var bot = (Bot) dbContext.BaseUsers.Find(packet.Bot.BaseUserId);
                dbContext.Entry(bot).Collection(b => b.Sessions).Load();

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new UserClickedBotViewPush()
                {
                    Notif = notif,
                    SessionIds = new List<long> {bot.Sessions.First().SessionId}
                });

                return new ClickBotViewResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public GetFileSizeResponse AnswerQuestion(AnswerContext<GetFileSizeRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var file = dbContext.Files.Find(context.Question.Packet.File.FileId);
                if (file == null)
                {
                    return new GetFileSizeResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                return new GetFileSizeResponse()
                {
                    Packet = new Packet() {Status = "success", File = file}
                };
            }
        }

        public async Task<WriteToFileResponse> AnswerQuestion(AnswerContext<WriteToFileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                var streamCode = context.Question.Packet.StreamCode;

                var file = dbContext.Files.Find(context.Question.Packet.File.FileId);
                if (file == null)
                {
                    return new WriteToFileResponse()
                    {
                        Packet = new Packet() {Status = "error_1"}
                    };
                }

                if (file.UploaderId != session.BaseUserId)
                {
                    return new WriteToFileResponse()
                    {
                        Packet = new Packet() {Status = "error_2"}
                    };
                }

                if (file.FileTransferFinished)
                {
                    return new WriteToFileResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                var myContent = JsonConvert.SerializeObject(new Packet()
                {
                    Username = GlobalVariables.FileTransferUsername,
                    Password = GlobalVariables.FileTransferPassword,
                    StreamCode = streamCode
                });
                var buffer = Encoding.UTF8.GetBytes(myContent);

                Directory.CreateDirectory(_dirPath);
                var filePath = Path.Combine(_dirPath, file.FileId.ToString());
                File.Create(filePath).Close();

                var responseStream = StreamRepo.FileStreams[streamCode];
                StreamRepo.FileStreams.Remove(streamCode);

                using (var stream = new FileStream(filePath, FileMode.Append))
                {
                    var b = new byte[128 * 1024];
                    int read;
                    while ((read = await responseStream.ReadAsync(b, 0, b.Length)) > 0)
                    {
                        stream.Write(b, 0, read);
                        file.Size += read;
                        dbContext.SaveChanges();
                    }

                    if (context.Question.Packet.FinishFileTransfer.HasValue &&
                        context.Question.Packet.FinishFileTransfer.Value)
                    {
                        file.FileTransferFinished = true;
                        dbContext.SaveChanges();
                    }
                }

                if (file is Photo photo && photo.IsAvatar)
                {
                    using (var image = Image.Load(filePath))
                    {
                        float width, height;
                        if (image.Width > image.Height)
                        {
                            width = image.Width > 256 ? 256 : image.Width;
                            height = image.Height * width / image.Width ;
                        }
                        else
                        {
                            height = image.Height > 256 ? 256 : image.Height;
                            width = image.Width * height / image.Height;
                        }

                        image.Mutate(x => x.Resize((int) width, (int) height));
                        File.Delete(filePath);
                        image.Save(filePath + ".png");
                    }
                }

                PhotoIndexerService.EnqueuePhotoForScan(filePath);

                return new WriteToFileResponse()
                {
                    Packet = new Packet() {Status = "success"}
                };
            }
        }

        public async Task<UploadPhotoResponse> AnswerQuestion(AnswerContext<UploadPhotoRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var form = context.Question.Form;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                Photo photo;
                FileUsage fileUsage;

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                var kt = new KafkaTransport();

                if (form.RoomId > 0)
                {
                    dbContext.Entry(user).Collection(u => u.Memberships).Load();
                    var membership = user.Memberships.Find(m => m.ComplexId == form.ComplexId);
                    if (membership == null)
                    {
                        return new UploadPhotoResponse()
                        {
                            Packet = new Packet {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(membership).Reference(m => m.Complex).Load();
                    var complex = membership.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    var room = complex.Rooms.Find(r => r.RoomId == form.RoomId);
                    if (room == null)
                    {
                        return new UploadPhotoResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }
                    
                    var id = kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    var fileUsageId = kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(FileUsage).Name
                    });

                    Task.WaitAll(id, fileUsageId);

                    photo = new Photo()
                    {
                        FileId = (long)id.Result.Index,
                        Width = form.Width,
                        Height = form.Height,
                        IsPublic = false,
                        Uploader = user,
                        IsAvatar = form.IsAvatar,
                        AsRawFile = form.AsRawFile,
                    };
                    dbContext.Files.Add(photo);
                    fileUsage = new FileUsage()
                    {
                        FileUsageId = fileUsageId.Result.Index,
                        File = photo,
                        Room = room
                    };
                    dbContext.FileUsages.Add(fileUsage);
                }
                else
                {
                    var id = await kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    
                    photo = new Photo()
                    {
                        FileId = (long)id.Index,
                        Width = form.Width,
                        Height = form.Height,
                        IsPublic = true,
                        Uploader = user,
                        IsAvatar = form.IsAvatar,
                        AsRawFile = form.AsRawFile
                    };
                    dbContext.Files.Add(photo);
                    fileUsage = null;
                }

                dbContext.SaveChanges();
                

                var filePath = Path.Combine(_dirPath, photo.FileId.ToString());
                File.Create(filePath).Close();


                return new UploadPhotoResponse()
                {
                    Packet = new Packet {Status = "success", File = photo, FileUsage = fileUsage}
                };
            }
        }

        public async Task<UploadAudioResponse> AnswerQuestion(AnswerContext<UploadAudioRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var form = context.Question.Form;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                Audio audio;
                FileUsage fileUsage;
                
                var kt = new KafkaTransport();

                if (form.RoomId > 0)
                {
                    dbContext.Entry(user).Collection(u => u.Memberships).Load();
                    var membership = user.Memberships.Find(m => m.ComplexId == form.ComplexId);
                    if (membership == null)
                    {
                        return new UploadAudioResponse()
                        {
                            Packet = new Packet {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(membership).Reference(m => m.Complex).Load();
                    var complex = membership.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    var room = complex.Rooms.Find(r => r.RoomId == form.RoomId);
                    if (room == null)
                    {
                        return new UploadAudioResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }

                    var id = kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    var fileUsageId = kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(FileUsage).Name
                    });

                    Task.WaitAll(id, fileUsageId);
                    
                    audio = new Audio()
                    {
                        FileId = id.Result.Index,
                        Title = form.Title,
                        Duration = form.Duration,
                        IsPublic = false,
                        Uploader = user,
                        AsRawFile = form.AsRawFile
                    };
                    dbContext.Files.Add(audio);
                    fileUsage = new FileUsage()
                    {
                        FileUsageId = fileUsageId.Result.Index,
                        File = audio,
                        Room = room
                    };
                    dbContext.FileUsages.Add(fileUsage);
                }
                else
                {
                    var id = await kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    
                    audio = new Audio()
                    {
                        FileId = (long)id.Index,
                        Title = form.Title,
                        Duration = form.Duration,
                        IsPublic = true,
                        Uploader = user,
                        AsRawFile = form.AsRawFile
                    };
                    dbContext.Files.Add(audio);
                    fileUsage = null;
                }

                dbContext.SaveChanges();

                
                var filePath = Path.Combine(_dirPath, audio.FileId.ToString());
                File.Create(filePath).Close();


                return new UploadAudioResponse()
                {
                    Packet = new Packet {Status = "success", File = audio, FileUsage = fileUsage}
                };
            }
        }

        public async Task<UploadVideoResponse> AnswerQuestion(AnswerContext<UploadVideoRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var form = context.Question.Form;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;

                Video video;
                FileUsage fileUsage;
                
                var kt = new KafkaTransport();

                if (form.RoomId > 0)
                {
                    dbContext.Entry(user).Collection(u => u.Memberships).Load();
                    var membership = user.Memberships.Find(m => m.ComplexId == form.ComplexId);
                    if (membership == null)
                    {
                        return new UploadVideoResponse()
                        {
                            Packet = new Packet {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(membership).Reference(m => m.Complex).Load();
                    var complex = membership.Complex;
                    dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                    var room = complex.Rooms.Find(r => r.RoomId == form.RoomId);
                    if (room == null)
                    {
                        return new UploadVideoResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }

                    var id = kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    var fileUsageId = kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(FileUsage).Name
                    });

                    Task.WaitAll(id, fileUsageId);
                    
                    video = new Video()
                    {
                        FileId = id.Result.Index,
                        Title = form.Title,
                        Duration = form.Duration,
                        IsPublic = false,
                        Uploader = user,
                        AsRawFile = form.AsRawFile
                    };
                    dbContext.Files.Add(video);
                    fileUsage = new FileUsage()
                    {
                        FileUsageId = fileUsageId.Result.Index,
                        File = video,
                        Room = room
                    };
                    dbContext.FileUsages.Add(fileUsage);
                }
                else
                {
                    var id = await kt.AskForNewId(new AskIndexEntity()
                        {
                            EntityType = typeof(Entities.File).Name
                        });
                    
                    video = new Video()
                    {
                        FileId = (long)id.Index,
                        Title = form.Title,
                        Duration = form.Duration,
                        IsPublic = true,
                        Uploader = user,
                        AsRawFile = form.AsRawFile
                    };
                    dbContext.Files.Add(video);
                    fileUsage = null;
                }

                dbContext.SaveChanges();
                

                var filePath = Path.Combine(_dirPath, video.FileId.ToString());
                File.Create(filePath).Close();
                

                return new UploadVideoResponse()
                {
                    Packet = new Packet {Status = "success", File = video, FileUsage = fileUsage}
                };
            }
        }

        public async Task<DownloadFileResponse> AnswerQuestion(AnswerContext<DownloadFileRequest> context)
        {
            var fileId = context.Question.Packet.File.FileId;

            if (File.Exists(Path.Combine(_dirPath, fileId.ToString())) ||
                File.Exists(Path.Combine(_dirPath, fileId + ".png")))
            {
                var streamCode = context.Question.Packet.StreamCode;
                var offset = context.Question.Packet.Offset;
                if (!offset.HasValue)
                    return new DownloadFileResponse()
                    {
                        Packet = new Packet() {Status = "error_4"}
                    };
                Stream stream;
                using (var dbContext = new DatabaseContext())
                {
                    var session = dbContext.Sessions.Find(context.Question.SessionId);
                    if (session == null)
                    {
                        return new DownloadFileResponse()
                        {
                            Packet = new Packet {Status = "error_0"}
                        };
                    }

                    var file = dbContext.Files.Find(fileId);
                    if (file == null)
                    {
                        return new DownloadFileResponse()
                        {
                            Packet = new Packet {Status = "error_1"}
                        };
                    }

                    dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                    var user = (User) session.BaseUser;
                    dbContext.Entry(user).Collection(u => u.Memberships).Load();
                    if (file.IsPublic)
                    {
                        stream = File.OpenRead(File.Exists(Path.Combine(_dirPath, fileId.ToString()))
                            ? Path.Combine(_dirPath, fileId.ToString())
                            : Path.Combine(_dirPath, fileId + ".png"));
                        StreamRepo.FileStreams[streamCode] = stream;
                        return new DownloadFileResponse()
                        {
                            Packet = new Packet {Status = "success"}
                        };
                    }

                    dbContext.Entry(file).Collection(f => f.FileUsages).Query().Include(fu => fu.Room).Load();
                    var foundPath = (from fu in file.FileUsages select fu.Room.ComplexId)
                        .Intersect(from mem in user.Memberships select mem.ComplexId).Any();
                    
                    if (!foundPath)
                        return new DownloadFileResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    stream = File.OpenRead(File.Exists(Path.Combine(_dirPath, fileId.ToString()))
                        ? Path.Combine(_dirPath, fileId.ToString())
                        : Path.Combine(_dirPath, fileId + ".png"));

                    StreamRepo.FileStreams[streamCode] = stream;
                    return new DownloadFileResponse()
                    {
                        Packet = new Packet {Status = "success"}
                    };

                }
            }

            return new DownloadFileResponse()
            {
                Packet = new Packet {Status = "error_3"}
            };
        }

        private async Task UploadFileToApiGateWay(string streamCode, long fileId, long offset)
        {
            NameValueCollection nvc = new NameValueCollection
            {
                {"StreamCode", streamCode},
                {"Username", GlobalVariables.FileTransferUsername},
                {"Password", GlobalVariables.FileTransferPassword}
            };
            HttpUploader.HttpUploadFile("" +
                                        GlobalVariables.FileTransferDownloadStreamAction, 
                File.Exists(Path.Combine(_dirPath, fileId.ToString()))
                    ? Path.Combine(_dirPath, fileId.ToString())
                    : Path.Combine(_dirPath, fileId + ".png")
                , "File", "image/png", nvc);
        }

        public SearchUsersResponse AnswerQuestion(AnswerContext<SearchUsersRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var users = (from u in dbContext.BaseUsers
                    where u is User
                    where EF.Functions.Like(u.Title, "%" + packet.SearchQuery + "%")
                    select u).Select(bu => (User) bu).ToList();

                return new SearchUsersResponse()
                {
                    Packet = new Packet {Status = "success", Users = users}
                };
            }
        }

        public SearchComplexesResponse AnswerQuestion(AnswerContext<SearchComplexesRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Query().Include(m => m.Complex).Load();
                var complexes = (from c in (from m in user.Memberships
                        where EF.Functions.Like(m.Complex.Title, "%" + packet.SearchQuery + "%")
                        select m)
                    select c.Complex).ToList();

                return new SearchComplexesResponse()
                {
                    Packet = new Packet {Status = "success", Complexes = complexes}
                };
            }
        }

        public async Task<BotGetWorkershipsResponse> AnswerQuestion(AnswerContext<BotGetWorkershipsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bot = (Bot) session.BaseUser;

                var workershipsQuery = dbContext.Workerships
                    .Where(w => w.BotId == bot.BaseUserId);

                await workershipsQuery.ForEachAsync((w) =>
                {
                    dbContext.Entry(w).Reference(worker => worker.Room).Load();
                    dbContext.Entry(w.Room)
                        .Reference(r => r.Complex).Query()
                        .Include(c => c.Members)
                        .ThenInclude(m => m.User).Load();
                });

                var workerships = workershipsQuery.ToList();

                return new BotGetWorkershipsResponse()
                {
                    Packet = new Packet() {Status = "success", Workerships = workerships}
                };
            }
        }

        public GetBotStoreContentResponse AnswerQuestion(AnswerContext<GetBotStoreContentRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var botStoreHeader = dbContext.BotStoreHeader
                    .Include(bsh => bsh.Banners)
                    .ThenInclude(b => b.Bot)
                    .FirstOrDefault();
                var botStoreSection = new BotStoreSection();
                var botStoreBots = dbContext.Bots.Select(bot => new BotStoreBot()
                    {
                        Bot = bot,
                        BotStoreSection = botStoreSection
                    })
                    .ToList();
                botStoreSection.BotStoreBots = botStoreBots;

                return new GetBotStoreContentResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success",
                        BotStoreHeader = botStoreHeader,
                        BotStoreSections = new List<BotStoreSection>() {botStoreSection}
                    }
                };
            }
        }

        public GetWorkershipsResponse AnswerQuestion(AnswerContext<GetWorkershipsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new GetWorkershipsResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                BaseRoom room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    dbContext.Entry(complex).Collection(c => c.SingleRooms).Load();
                    room = complex.SingleRooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                    if (room == null)
                    {
                        return new GetWorkershipsResponse()
                        {
                            Packet = new Packet {Status = "error_2"}
                        };
                    }
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workers = room.Workers.ToList();
                return new GetWorkershipsResponse()
                {
                    Packet = new Packet {Status = "success", Workerships = workers}
                };
            }
        }

        public async Task<AddBotToRoomResponse> AnswerQuestion(AnswerContext<AddBotToRoomRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new AddBotToRoomResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.MemberAccess).Load();
                if (!membership.MemberAccess.CanModifyWorkers)
                {
                    return new AddBotToRoomResponse()
                    {
                        Packet = new Packet() {Status = "error_3"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                BaseRoom room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    dbContext.Entry(complex).Collection(c => c.SingleRooms).Load();
                    room = complex.SingleRooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                    if (room == null)
                    {
                        return new AddBotToRoomResponse()
                        {
                            Packet = new Packet {Status = "error_4"}
                        };
                    }
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workership = room.Workers.Find(w => w.BotId == packet.Bot.BaseUserId);
                if (workership != null)
                {
                    return new AddBotToRoomResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                var bot = dbContext.Bots.Find(packet.Bot.BaseUserId);
                if (bot == null)
                {
                    return new AddBotToRoomResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }
                
                var kt = new KafkaTransport();
                var id = await kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(Workership).Name
                    });

                workership = new Workership()
                {
                    WorkershipId = (long)id.Index,
                    BotId = bot.BaseUserId,
                    Room = room,
                    PosX = packet.Workership.PosX,
                    PosY = packet.Workership.PosY,
                    Width = packet.Workership.Width,
                    Height = packet.Workership.Height,
                    Angle = packet.Workership.Angle
                };
                dbContext.AddRange(workership);
                dbContext.SaveChanges();

                dbContext.Entry(workership).State = EntityState.Detached;
                var finalWorkership = dbContext.Workerships.Find(workership.WorkershipId);
                dbContext.Entry(finalWorkership)
                    .Reference(w => w.Room)
                    .Load(); 
                dbContext.Entry(finalWorkership.Room)
                    .Reference(r => r.Complex)
                    .Load();
                dbContext.Entry(finalWorkership.Room.Complex)
                    .Collection(c => c.Members)
                    .Query().Include(m => m.User)
                    .Load();

                dbContext.Entry(bot).Collection(b => b.Sessions).Load();
                var botSess = bot.Sessions.FirstOrDefault();
                var addition = new BotAdditionToRoomNotification()
                {
                    Workership = finalWorkership,
                    Bot = bot
                };
                if (botSess != null)
                    kt.PushNotifToApiGateway(new BotAdditionToRoomPush()
                    {
                        Notif = addition,
                        SessionIds = new[] {botSess.SessionId}.ToList()
                    });

                Bot finalBot;
                using (var finalContext = new DatabaseContext())
                {
                    finalBot = (Bot) finalContext.BaseUsers.Find(bot.BaseUserId);
                }

                var addition2 = new BotAdditionToRoomNotification()
                {
                    Bot = finalBot,
                    Workership = finalWorkership
                };

                dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions).Load();

                var sessionIds = (from m in complex.Members
                    where m.User.BaseUserId != user.BaseUserId
                    from s in m.User.Sessions
                    select s.SessionId).ToList();

                kt.PushNotifToApiGateway(new BotAdditionToRoomPush()
                {
                    Notif = addition2,
                    SessionIds = sessionIds
                });

                return new AddBotToRoomResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success",
                        Workership = workership
                    }
                };
            }
        }

        public UpdateWorkershipResponse AnswerQuestion(AnswerContext<UpdateWorkershipRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new UpdateWorkershipResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new UpdateWorkershipResponse()
                    {
                        Packet = new Packet {Status = "error_2"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workership = room.Workers.Find(w => w.BotId == packet.Bot.BaseUserId);
                if (workership == null)
                {
                    return new UpdateWorkershipResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }

                workership.PosX = packet.Workership.PosX;
                workership.PosY = packet.Workership.PosY;
                workership.Width = packet.Workership.Width;
                workership.Height = packet.Workership.Height;
                workership.Angle = packet.Workership.Angle;
                dbContext.SaveChanges();

                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(new WorkershipUpdatedNotif()
                {
                    Packet = new Packet() {Workership = workership}
                });

                var notif = new BotPropertiesChangedNotification()
                {
                    Workership = workership
                };

                dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions).Load();

                var sessionIds = (from m in complex.Members
                    where m.User.BaseUserId != user.BaseUserId
                    from s in m.User.Sessions
                    select s.SessionId).ToList();

                kt.PushNotifToApiGateway(new BotPropertiesChangedPush()
                {
                    Notif = notif,
                    SessionIds = sessionIds
                });

                return new UpdateWorkershipResponse()
                {
                    Packet = new Packet {Status = "success"}
                };
            }
        }

        public RemoveBotFromRoomResponse AnswerQuestion(AnswerContext<RemoveBotFromRoomRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership = user.Memberships.Find(m => m.ComplexId == packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new RemoveBotFromRoomResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();
                var room = complex.Rooms.Find(r => r.RoomId == packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new RemoveBotFromRoomResponse()
                    {
                        Packet = new Packet {Status = "error_3"}
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var workership = room.Workers.Find(w => w.BotId == packet.Bot.BaseUserId);
                if (workership == null)
                {
                    return new RemoveBotFromRoomResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }

                dbContext.Entry(workership).Reference(w => w.Room).Load();
                dbContext.Entry(workership.Room).Reference(r => r.Complex).Load();
                room.Workers.Remove(workership);
                dbContext.Workerships.Remove(workership);
                dbContext.SaveChanges();

                var kt = new KafkaTransport();

                var bot = dbContext.Bots.Find(workership.BotId);
                dbContext.Entry(bot).Collection(b => b.Sessions).Load();
                var botSess = bot.Sessions.FirstOrDefault();

                var removation = new BotRemovationFromRoomNotification()
                {
                    Workership = workership
                };

                if (botSess != null)
                    kt.PushNotifToApiGateway(new BotRemovationFromRoomPush()
                    {
                        Notif = removation,
                        SessionIds = new[] {botSess.SessionId}.ToList()
                    });

                dbContext.Entry(complex).Collection(c => c.Members).Query().Include(m => m.User)
                    .ThenInclude(u => u.Sessions).Load();

                var sessionIds = (from m in complex.Members
                    where m.User.BaseUserId != user.BaseUserId
                    from s in m.User.Sessions
                    select s.SessionId).ToList();

                kt.PushNotifToApiGateway(new BotRemovationFromRoomPush()
                {
                    Notif = removation,
                    SessionIds = sessionIds
                });

                return new RemoveBotFromRoomResponse()
                {
                    Packet = new Packet {Status = "success"}
                };
            }
        }

        public GetComplexWorkersResponse AnswerQuestion(AnswerContext<GetComplexWorkersRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var complex = dbContext.Complexes.Find(context.Question.Packet.Complex.ComplexId);

                dbContext.Entry(complex).Collection(c => c.Rooms).Query().Include(r => r.Workers).Load();

                var workers = new List<Workership>();
                foreach (var room in complex.Rooms)
                {
                    workers.AddRange(room.Workers.ToList());
                }

                return new GetComplexWorkersResponse
                {
                    Packet = new Packet() {Workerships = workers}
                };
            }
        }

        public GetBotsResponse AnswerQuestion(AnswerContext<GetBotsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var bots = dbContext.Bots.Include(b => b.BotSecret).ToList();
                var finalBots = new List<Bot>();
                var bannedAccessIds = new List<long>();
                foreach (var bot in bots)
                {
                    if (bot.BotSecret.CreatorId != session.BaseUserId)
                    {
                        bannedAccessIds.Add(bot.BaseUserId);
                    }
                    else
                    {
                        finalBots.Add(bot);
                    }
                }

                using (var nextContext = new DatabaseContext())
                {
                    foreach (var id in bannedAccessIds)
                    {
                        var finalBot = nextContext.Bots.Find(id);
                        finalBots.Add(finalBot);
                    }
                }

                return new GetBotsResponse()
                {
                    Packet = new Packet {Status = "success", Bots = finalBots}
                };
            }
        }

        public GetCreatedBotsResponse AnswerQuestion(AnswerContext<GetCreatedBotsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                var bots = new List<Bot>();
                dbContext.Entry(user).Collection(u => u.CreatedBots).Load();
                var creations = user.CreatedBots.ToList();
                foreach (var botCreation in user.CreatedBots)
                {
                    dbContext.Entry(botCreation).Reference(bc => bc.Bot).Load();
                    dbContext.Entry(botCreation.Bot).Reference(b => b.BotSecret).Load();
                    bots.Add(botCreation.Bot);
                }

                return new GetCreatedBotsResponse()
                {
                    Packet = new Packet {Status = "success", Bots = bots, BotCreations = creations}
                };
            }
        }

        public GetSubscribedBotsResponse AnswerQuestion(AnswerContext<GetSubscribedBotsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Question.SessionId);
                if (session == null)
                {
                    return new GetSubscribedBotsResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                var bots = new List<Bot>();
                dbContext.Entry(user).Collection(u => u.SubscribedBots).Load();
                var subscriptions = user.SubscribedBots.ToList();
                var noAccessBotIds = new List<long>();
                foreach (var botSubscription in user.SubscribedBots)
                {
                    dbContext.Entry(botSubscription).Reference(bc => bc.Bot).Load();
                    dbContext.Entry(botSubscription.Bot).Reference(b => b.BotSecret).Load();
                    if (botSubscription.Bot.BotSecret.CreatorId == user.BaseUserId)
                    {
                        bots.Add(botSubscription.Bot);
                    }
                    else
                    {
                        noAccessBotIds.Add(botSubscription.Bot.BaseUserId);
                    }
                }

                using (var nextContext = new DatabaseContext())
                {
                    foreach (var id in noAccessBotIds)
                    {
                        var bot = nextContext.Bots.Find(id);
                        bots.Add(bot);
                    }
                }

                return new GetSubscribedBotsResponse()
                {
                    Packet = new Packet {Status = "success", Bots = bots, BotSubscriptions = subscriptions}
                };
            }
        }

        public SearchBotsResponse AnswerQuestion(AnswerContext<SearchBotsRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;

                var bots = (from b in dbContext.Bots
                    where EF.Functions.Like(b.Title, "%" + packet.SearchQuery + "%")
                    select b).ToList();

                return new SearchBotsResponse()
                {
                    Packet = new Packet {Status = "success", Bots = bots}
                };
            }
        }

        public UpdateBotProfileResponse AnswerQuestion(AnswerContext<UpdateBotProfileRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.CreatedBots).Load();
                var botCreation = user.CreatedBots.Find(bc => bc.BotId == packet.Bot.BaseUserId);
                if (botCreation == null)
                {
                    return new UpdateBotProfileResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }

                dbContext.Entry(botCreation).Reference(bc => bc.Bot).Load();
                var bot = botCreation.Bot;
                bot.Title = packet.Bot.Title;
                bot.Avatar = packet.Bot.Avatar;
                bot.Description = packet.Bot.Description;
                bot.ViewURL = packet.Bot.ViewURL;
                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();

                return new UpdateBotProfileResponse()
                {
                    Packet = new Packet {Status = "success"}
                };
            }
        }

        public GetBotResponse AnswerQuestion(AnswerContext<GetBotRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var robot = (Bot) dbContext.BaseUsers.Find(packet.Bot.BaseUserId);
                if (robot == null)
                {
                    return new GetBotResponse()
                    {
                        Packet = new Packet {Status = "error_080"}
                    };
                }

                dbContext.Entry(robot).Reference(r => r.BotSecret).Load();
                if (robot.BotSecret.CreatorId == session.BaseUserId)
                {
                    return new GetBotResponse()
                    {
                        Packet = new Packet {Status = "success", Bot = robot}
                    };
                }

                using (var nextContext = new DatabaseContext())
                {
                    var nextBot = nextContext.Bots.Find(packet.Bot.BaseUserId);
                    return new GetBotResponse()
                    {
                        Packet = new Packet {Status = "success", Bot = nextBot}
                    };
                }
            }
        }

        public async Task<CreateBotResponse> AnswerQuestion(AnswerContext<CreateBotRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                var bot = new Bot();

                var token = "+" + Security.MakeKey64();

                var botSess = new Session()
                {
                    Token = token
                };

                var kt = new KafkaTransport();
                var botId = await kt.AskForNewId(
                    new AskIndexEntity {EntityType = typeof(BaseUser).Name});
                var sessionId = await kt.AskForNewId(
                    new AskIndexEntity {EntityType = typeof(Session).Name});

                bot.BaseUserId = (long) botId.Index;
                bot.Title = packet.Bot.Title;
                bot.Avatar = packet.Bot.Avatar > 0 ? packet.Bot.Avatar : 0;
                bot.Description = packet.Bot.Description;

                botSess.SessionId = (long) sessionId.Index;
                botSess.BaseUser = bot;

                var botSecret = new BotSecret()
                {
                    Bot = bot,
                    Creator = user,
                    Token = token
                };
                bot.BotSecret = botSecret;

                var botCreation = new BotCreation()
                {
                    Bot = bot,
                    Creator = user
                };
                var subscription = new BotSubscription()
                {
                    Bot = bot,
                    Subscriber = user
                };
                dbContext.AddRange(bot, botSecret, botSess, botCreation, subscription);
                dbContext.SaveChanges();

                var versions = new List<Version>()
                {
                    new Version()
                    {
                        VersionId = "BaseUser_" + bot.BaseUserId + "_MessengerService",
                        Number = bot.Version
                    },
                    new Version()
                    {
                        VersionId = "Session_" + botSess.SessionId + "_MessengerService",
                        Number = bot.Version
                    }
                };

                return new CreateBotResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success",
                        Bot = bot,
                        BotCreation = botCreation,
                        BotSubscription = subscription,
                        Versions = versions
                    }
                };
            }
        }

        public async Task<SubscribeBotResponse> AnswerQuestion(AnswerContext<SubscribeBotRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = context.Question.Packet;
                var session = dbContext.Sessions.Find(context.Question.SessionId);

                var bot = dbContext.Bots.Find(packet.Bot.BaseUserId);
                if (bot == null)
                {
                    return new SubscribeBotResponse()
                    {
                        Packet = new Packet {Status = "error_1"}
                    };
                }

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();
                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.SubscribedBots).Load();
                if (user.SubscribedBots.Any(b => b.BotId == packet.Bot.BaseUserId))
                {
                    return new SubscribeBotResponse()
                    {
                        Packet = new Packet {Status = "error_0"}
                    };
                }

                var kt = new KafkaTransport();
                var id = await kt.AskForNewId(new AskIndexEntity()
                    {
                        EntityType = typeof(BotSubscription).Name
                    });
                    
                var subscription = new BotSubscription()
                {
                    BotSubscriptionId = (long)id.Index,
                    Bot = bot,
                    Subscriber = user
                };
                dbContext.AddRange(subscription);
                dbContext.SaveChanges();

                return new SubscribeBotResponse()
                {
                    Packet = new Packet {Status = "success", BotSubscription = subscription}
                };
            }
        }
        
        private const string EmailAddress = "keyhan.mohammadi1997@gmail.com";
        private const string EmailPassword = "2&b165sf4j)684tkt87El^o9w68i87u6s*4h48#98aq";
        
        private static void SendEmail(string to, string subject, string content)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(EmailAddress);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true;
                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(EmailAddress, EmailPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        private static void IncreaseVersion(DatabaseContext dbContext, string eIdField, long eId, string tableName)
        {
            if (tableName == "Rooms") tableName = "BaseRooms";
            dbContext.Database.ExecuteSqlCommand($"update \"{tableName}\" " +
                                                 "set \"Version\" = \"Version\" + 1 " +
                                                 $"where \"{eIdField}\" = {eId}");
        }

        private static void NotifyVersionsUpdated(List<Version> versions)
        {
            
        }

        public async Task<AnswerAddBotScreenShot> AnswerQuestion(AnswerContext<AskAddBotScreenShot> question)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = question.Question.Packet;
                var session = dbContext.Sessions.Find(question.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();

                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.CreatedBots).Load();

                var botCreation = user.CreatedBots.Find(cb => cb.BotId == packet.Bot.BaseUserId);

                if (botCreation == null)
                {
                    return new AnswerAddBotScreenShot()
                    {
                        Packet = new Packet()
                        {
                            Status = "e10"
                        }
                    };
                }
                
                dbContext.Entry(botCreation).Reference(bc => bc.Bot).Load();
                var bot = botCreation.Bot;
                
                dbContext.Entry(bot).Collection(b => b.ScreenShots).Load();

                var kt = new KafkaTransport();
                var bssId = await kt.AskForNewId(new AskIndexEntity()
                {
                    EntityType = typeof(BotScreenShot).Name
                });
                var photo = dbContext.Files.Find(packet.Photo.FileId);
                if (!(photo is Photo))
                {
                    return new AnswerAddBotScreenShot()
                    {
                        Packet = new Packet()
                        {
                            Status = "e11"
                        }
                    };
                }

                var bss = new BotScreenShot()
                {
                    BotScreenShotId = bssId.Index,
                    Bot = bot,
                    Photo = (Photo) photo
                };

                dbContext.BotScreenShots.Add(bss);
                dbContext.SaveChanges();
                
                return new AnswerAddBotScreenShot()
                {
                    Packet = new Packet()
                    {
                        Status = "success",
                        BotScreenShot = bss
                    }
                };
            }
        }

        public AnswerRemoveBotScreenShot AnswerQuestion(AnswerContext<AskRemoveBotScreenShot> question)
        {
            using (var dbContext = new DatabaseContext())
            {
                var packet = question.Question.Packet;
                var session = dbContext.Sessions.Find(question.Question.SessionId);

                dbContext.Entry(session).Reference(s => s.BaseUser).Load();

                var user = (User) session.BaseUser;
                dbContext.Entry(user).Collection(u => u.CreatedBots).Load();

                var botCreation = user.CreatedBots.Find(cb => cb.BotId == packet.Bot.BaseUserId);

                if (botCreation == null)
                {
                    return new AnswerRemoveBotScreenShot()
                    {
                        Packet = new Packet()
                        {
                            Status = "e10"
                        }
                    };
                }
                
                dbContext.Entry(botCreation).Reference(bc => bc.Bot).Load();
                var bot = botCreation.Bot;
                
                dbContext.Entry(bot).Collection(b => b.ScreenShots).Load();
                var bss = bot.ScreenShots.Find(botSs => botSs.BotScreenShotId == packet.BotScreenShot.BotScreenShotId);
                if (bss == null)
                {
                    return new AnswerRemoveBotScreenShot()
                    {
                        Packet = new Packet()
                        {
                            Status = "e11"
                        }
                    };
                }

                bot.ScreenShots.Remove(bss);
                dbContext.BotScreenShots.Remove(bss);
                dbContext.SaveChanges();

                var kt = new KafkaTransport();

                return new AnswerRemoveBotScreenShot()
                {
                    Packet = new Packet()
                    {
                        Status = "success"
                    }
                };
            }
        }

        public BotViewResizedResponse AnswerQuestion(AnswerContext<BotViewResizedRequest> question)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(question.Question.SessionId);
                dbContext.Entry(session).Reference(s => s.BaseUser).Load();

                var user = (User) session.BaseUser;
                
                dbContext.Entry(user).Collection(u => u.Memberships).Load();
                var membership =
                    user.Memberships.Find(mem => mem.ComplexId == question.Question.Packet.Complex.ComplexId);
                if (membership == null)
                {
                    return new BotViewResizedResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "error_04"
                        }
                    };
                }
                
                dbContext.Entry(membership).Reference(m => m.Complex).Load();
                var complex = membership.Complex;
                
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();

                var room = complex.Rooms.Find(r => r.RoomId == question.Question.Packet.BaseRoom.RoomId);
                if (room == null)
                {
                    return new BotViewResizedResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "error_03"
                        }
                    };
                }

                dbContext.Entry(room).Collection(r => r.Workers).Load();
                var worker = room.Workers.Find(w => w.BotId == question.Question.Packet.Bot.BaseUserId);
                if (worker == null)
                {
                    return new BotViewResizedResponse()
                    {
                        Packet = new Packet
                        {
                            Status = "error_02"
                        }
                    };
                }

                var bot = dbContext.BaseUsers.Find(question.Question.Packet.Bot.BaseUserId);
                dbContext.Entry(bot).Collection(b => b.Sessions).Load();

                var notif = new BotViewResizedNotification()
                {
                    UserId = user.BaseUserId,
                    ComplexId = complex.ComplexId,
                    RoomId = room.RoomId,
                    NewWidth = question.Question.Packet.Workership.Width,
                    NewHeight = question.Question.Packet.Workership.Height
                };

                var push = new BotViewResizedPush()
                {
                    Notif = notif,
                    SessionIds = bot.Sessions.Select(s => s.SessionId).ToList()
                };
                
                var kt = new KafkaTransport();
                kt.PushNotifToApiGateway(push);

                return new BotViewResizedResponse()
                {
                    Packet = new Packet
                    {
                        Status = "success"
                    }
                };
            }
        }
    }
}