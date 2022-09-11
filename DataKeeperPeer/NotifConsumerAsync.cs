
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharedArea;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Pushes;
using SharedArea.Entities;
using SharedArea.Notifications;

namespace DataKeeperPeer
{
    public class NotifConsumerAsync :
        IConsumerAsync<ComplexCreatedNotif>,
        IConsumerAsync<RoomCreatedNotif>, 
        IConsumerAsync<MembershipCreatedNotif>, 
        IConsumerAsync<SessionCreatedNotif>, 
        IConsumerAsync<UserProfileUpdatedNotif>, 
        IConsumerAsync<ComplexProfileUpdatedNotif>,
        IConsumerAsync<RoomProfileUpdatedNotif>,
        IConsumerAsync<RoomDeletionNotif>,
        IConsumerAsync<ContactCreatedNotif>, 
        IConsumerAsync<InviteCreatedNotif>,
        IConsumerAsync<InviteCancelledNotif>,
        IConsumerAsync<InviteAcceptedNotif>, 
        IConsumerAsync<InviteIgnoredNotif>, 
        IConsumerAsync<SessionUpdatedNotif>,
        IConsumerAsync<AccountCreatedNotif>, 
        IConsumerAsync<AccountDeletedNotif>,
        IConsumerAsync<LogoutNotif>,
        IConsumerAsync<ComplexDeletionNotif>,
        IConsumerAsync<PendingCreatedNotif>,
        
        IConsumerAsync<ModuleProfileUpdatedNotif>,
        IConsumerAsync<ModulePermissionCreated>,
        IConsumerAsync<MessageSeenNotif>,
        IConsumerAsync<WorkershipCreatedNotif>,
        IConsumerAsync<WorkershipDeletedNotif>,
        IConsumerAsync<WorkershipUpdatedNotif>,
        IConsumerAsync<BotCreatedNotif>,
        IConsumerAsync<BotProfileUpdatedNotif>,
        IConsumerAsync<ModuleCreatedNotif>,
        IConsumerAsync<BotScreenShotCreatedNotif>,
        IConsumerAsync<BotScreenShotRemovedNotif>

    {
        private const string ServiceName = "DataKeeperPeer1";
        
        public Task Consume(ConsumeContext<AccountDeletedNotif> context)
        {
            var gUser = context.Message.Packet.User;

            using (var dbContext = new DatabaseContext())
            {
                var user = (User) dbContext.BaseUsers.Find(gUser.BaseUserId);

                if (user != null)
                {
                    dbContext.Entry(user).Collection(u => u.Sessions).Load();
                    dbContext.Entry(user).Reference(u => u.UserSecret).Load();

                    user.Title = "Deleted User";
                    user.Avatar = -1;
                    user.UserSecret.Email = "";
                    dbContext.Sessions.RemoveRange(user.Sessions);

                    dbContext.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ComplexCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var admin = (User) dbContext.BaseUsers.Find(context.Message.Packet.User.BaseUserId);
                var complex = context.Message.Packet.Complex;
                var complexSecret = context.Message.Packet.ComplexSecret;

                complex.ComplexSecret = complexSecret;
                complexSecret.Complex = complex;
                complex.Rooms[0].Complex = complex;
                complex.Members[0].Complex = complex;
                complexSecret.Admin = admin;
                complex.Members[0].User = admin;
                complex.Members[0].MemberAccess.Membership = complex.Members[0];

                dbContext.AddRange(complex);

                dbContext.SaveChanges();

                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Complex_" + complex.ComplexId + "_" + ServiceName,
                            Number = complex.Version
                        },
                        new Version()
                        {
                            VersionId = "ComplexSecret_" + complex.ComplexSecret.ComplexSecretId + "_" + ServiceName,
                            Number = complex.ComplexSecret.Version
                        },
                        new Version()
                        {
                            VersionId = "Room_" + complex.Rooms[0].RoomId + "_" + ServiceName,
                            Number = complex.Rooms[0].Version
                        },
                        new Version()
                        {
                            VersionId = "Membership_" + complex.Members[0].MembershipId + "_" + ServiceName,
                            Number = complex.Members[0].Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + complex.Members[0].MemberAccess.MemberAccessId + "_" + ServiceName,
                            Number = complex.Members[0].MemberAccess.Version
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoomCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var room = context.Message.Packet.BaseRoom;
                var complex = dbContext.Complexes.Find(room.ComplexId);
                
                room.Complex = complex;

                dbContext.AddRange(room);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Room_" + room.RoomId + "_" + ServiceName,
                            Number = room.Version
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<MembershipCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var membership = context.Message.Packet.Membership;
                var user = (User) dbContext.BaseUsers.Find(context.Message.Packet.Membership.User.BaseUserId);
                var complex = dbContext.Complexes.Find(context.Message.Packet.Membership.Complex.ComplexId);

                membership.User = user;
                membership.Complex = complex;

                dbContext.AddRange(membership);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Membership_" + membership.MembershipId + "_" + ServiceName,
                            Number = membership.Version
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<SessionCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = context.Message.Packet.Session;
                var user = context.Message.Packet.BaseUser;

                session.BaseUser = dbContext.BaseUsers.Find(user.BaseUserId);

                dbContext.AddRange(session);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Session_" + session.SessionId + "_" + ServiceName,
                            Number = session.Version
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<UserProfileUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var globalUser = context.Message.Packet.BaseUser;

                var localUser = dbContext.BaseUsers.Find(globalUser.BaseUserId);

                localUser.Title = globalUser.Title;
                localUser.Avatar = globalUser.Avatar;
                localUser.Version = globalUser.Version;

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "BaseUser_" + localUser.BaseUserId + "_" + ServiceName,
                            Number = localUser.Version
                        }
                    }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ComplexProfileUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var globalComplex = context.Message.Packet.Complex;

                var localComplex = dbContext.Complexes.Find(globalComplex.ComplexId);

                localComplex.Title = globalComplex.Title;
                localComplex.Avatar = globalComplex.Avatar;
                localComplex.Version = globalComplex.Version;

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Complex_" + localComplex.ComplexId + "_" + ServiceName,
                            Number = localComplex.Version
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoomProfileUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var globalRoom = context.Message.Packet.BaseRoom;

                var localRoom = dbContext.BaseRooms.Find(globalRoom.RoomId);

                localRoom.Title = globalRoom.Title;
                localRoom.Avatar = globalRoom.Avatar;
                localRoom.Version = globalRoom.Version;

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Room_" + localRoom.RoomId + "_" + ServiceName,
                            Number = localRoom.Version
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoomDeletionNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var globalRoom = context.Message.Packet.BaseRoom;

                var localRoom = dbContext.BaseRooms.Find(globalRoom.RoomId);

                dbContext.Entry(localRoom).Reference(r => r.Complex).Load();
                var complex = localRoom.Complex;
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();

                if (localRoom is Room ro)
                    complex.Rooms.Remove(ro);
                else if (localRoom is SingleRoom sr)
                    complex.SingleRooms.Remove(sr);
                
                dbContext.BaseRooms.Remove(localRoom);

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ContactCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var me = (User) dbContext.BaseUsers.Find(context.Message.Packet.Users[0].BaseUserId);
                var peer = (User) dbContext.BaseUsers.Find(context.Message.Packet.Users[1].BaseUserId);

                var complex = context.Message.Packet.Complex;
                var complexSecret = context.Message.Packet.ComplexSecret;
                var room = context.Message.Packet.BaseRoom;
                var m1 = context.Message.Packet.Memberships[0];
                var m2 = context.Message.Packet.Memberships[1];

                var lComplex = new Complex()
                {
                    ComplexId = complex.ComplexId,
                    Title = complex.Title,
                    Avatar = complex.Avatar,
                    Mode = complex.Mode,
                    Version = complex.Version,
                    ComplexSecret = new ComplexSecret()
                    {
                        ComplexSecretId = complexSecret.ComplexSecretId,
                        Admin = null,
                        Version = complexSecret.Version
                    },
                    Rooms = new List<Room>()
                    {
                        new Room()
                        {
                            RoomId = room.RoomId,
                            Title = room.Title,
                            Avatar = room.Avatar,
                            Version = room.Version
                        }
                    },
                    Members = new List<Membership>()
                    {
                        new Membership()
                        {
                            MembershipId = m1.MembershipId,
                            User = me,
                            Version = m1.Version,
                            MemberAccess = new MemberAccess()
                            {
                                MemberAccessId = m1.MemberAccess.MemberAccessId,
                                CanCreateMessage = m1.MemberAccess.CanCreateMessage,
                                CanModifyAccess = m1.MemberAccess.CanModifyAccess,
                                CanModifyWorkers = m1.MemberAccess.CanModifyWorkers,
                                CanSendInvite = m1.MemberAccess.CanSendInvite,
                                CanUpdateProfiles = m1.MemberAccess.CanUpdateProfiles,
                                Version = m1.MemberAccess.Version
                            }
                        },
                        new Membership()
                        {
                            MembershipId = m2.MembershipId,
                            User = peer,
                            Version = m2.Version,
                            MemberAccess = new MemberAccess()
                            {
                                MemberAccessId = m2.MemberAccess.MemberAccessId,
                                CanCreateMessage = m2.MemberAccess.CanCreateMessage,
                                CanModifyAccess = m2.MemberAccess.CanModifyAccess,
                                CanModifyWorkers = m2.MemberAccess.CanModifyWorkers,
                                CanSendInvite = m2.MemberAccess.CanSendInvite,
                                CanUpdateProfiles = m2.MemberAccess.CanUpdateProfiles,
                                Version = m2.MemberAccess.Version
                            }
                        }
                    }
                };

                lComplex.Members[0].MemberAccess.Membership = lComplex.Members[0];
                lComplex.Members[1].MemberAccess.Membership = lComplex.Members[1];

                dbContext.AddRange(lComplex);
                dbContext.SaveChanges();

                var myContact = context.Message.Packet.Contacts[0];
                myContact.Complex = lComplex;
                myContact.User = me;
                myContact.Peer = peer;
                dbContext.Contacts.Add(myContact);

                var peerContact = context.Message.Packet.Contacts[1];
                peerContact.Complex = lComplex;
                peerContact.User = peer;
                peerContact.Peer = me;
                dbContext.Contacts.Add(peerContact);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                        {
                            new Version()
                            {
                                VersionId = "Complex_" + complex.ComplexId + "_" + ServiceName,
                                Number = complex.Version
                            },
                            new Version()
                            {
                                VersionId = "ComplexSecret_" + complexSecret.ComplexSecretId + "_" + ServiceName,
                                Number = complexSecret.Version
                            },
                            new Version()
                            {
                                VersionId = "Room_" + room.RoomId + "_" + ServiceName,
                                Number = room.Version
                            },
                            new Version()
                            {
                                VersionId = "Membership_" + m1.MembershipId + "_" + ServiceName,
                                Number = m1.Version
                            },
                            new Version()
                            {
                                VersionId = "Membership_" + m2.MembershipId + "_" + ServiceName,
                                Number = m2.Version
                            },
                            new Version()
                            {
                                VersionId = "MemberAccess_" + m1.MemberAccess.MemberAccessId + "_" + ServiceName,
                                Number = m1.MemberAccess.Version
                            },
                            new Version()
                            {
                                VersionId = "MemberAccess_" + m2.MemberAccess.MemberAccessId + "_" + ServiceName,
                                Number = m2.MemberAccess.Version
                            },
                            new Version()
                            {
                                VersionId = "Contact_" + myContact.ContactId + "_" + ServiceName,
                                Number = myContact.Version
                            },
                            new Version()
                            {
                                VersionId = "Contact_" + peerContact.ContactId + "_" + ServiceName,
                                Number = peerContact.Version
                            }
                        }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<InviteCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var invite = context.Message.Packet.Invite;
                var complex = dbContext.Complexes.Find(context.Message.Packet.Complex.ComplexId);
                var user = (User) dbContext.BaseUsers.Find(context.Message.Packet.User.BaseUserId);

                invite.Complex = complex;
                invite.User = user;

                dbContext.AddRange(invite);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Invite_" + invite.InviteId + "_" + ServiceName,
                            Number = invite.Version
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<InviteCancelledNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var invite = dbContext.Invites.Find(context.Message.Packet.Invite.InviteId);
                var user = (User) dbContext.BaseUsers.Find(context.Message.Packet.User.BaseUserId);

                user.Invites.Remove(invite);
                dbContext.Invites.Remove(invite);

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<InviteAcceptedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var invite = dbContext.Invites.Find(context.Message.Packet.Invite.InviteId);
                var membership = context.Message.Packet.Membership;
                var human = (User) dbContext.BaseUsers.Find(invite.UserId);
                var complex = dbContext.Complexes.Find(invite.ComplexId);

                dbContext.Entry(human).Collection(h => h.Invites).Load();
                human.Invites.Remove(invite);
                dbContext.Invites.Remove(invite);
                membership.Complex = complex;
                membership.User = human;
                dbContext.Entry(complex).Collection(c => c.Members).Load();
                complex.Members.Add(membership);
                dbContext.Memberships.Add(membership);
                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Membership_" + membership.MembershipId + "_" + ServiceName,
                            Number = membership.Version
                        },
                        new Version()
                        {
                            VersionId = "MemberAccess_" + membership.MemberAccess.MemberAccessId + "_" + ServiceName,
                            Number = membership.MemberAccess.Version
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<InviteIgnoredNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var invite = dbContext.Invites.Find(context.Message.Packet.Invite.InviteId);
                var human = (User) dbContext.BaseUsers.Find(context.Message.Packet.User.BaseUserId);

                dbContext.Entry(invite).Reference(i => i.Complex).Load();
                human.Invites.Remove(invite);
                dbContext.Invites.Remove(invite);

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<SessionUpdatedNotif> context)
        {
            var globalSession = context.Message.Packet.Session;

            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(globalSession.SessionId);

                session.Online = globalSession.Online;
                session.ConnectionId = globalSession.ConnectionId;
                session.Token = globalSession.Token;
                session.Version = globalSession.Version;

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                    {
                        new Version()
                        {
                            VersionId = "Session_" + session.SessionId + "_" + ServiceName,
                            Number = session.Version
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<AccountCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var user = context.Message.Packet.User;
                var userSecret = context.Message.Packet.UserSecret;
                var complexSecret = context.Message.Packet.ComplexSecret;

                user.UserSecret = userSecret;
                userSecret.User = user;
                user.Memberships[0].User = user;
                user.Memberships[0].Complex.ComplexSecret = complexSecret;
                user.Memberships[0].Complex.ComplexSecret.Complex = user.Memberships[0].Complex;
                user.Memberships[0].Complex.ComplexSecret.Admin = user;
                user.Memberships[0].Complex.Rooms[0].Complex = user.Memberships[0].Complex;
                user.UserSecret.Home = user.Memberships[0].Complex;
                user.Memberships[0].User = user;
                user.Memberships[0].MemberAccess.Membership = user.Memberships[0];
                
                dbContext.AddRange(user);

                dbContext.SaveChanges();
                
                var kt = new KafkaTransport();
                kt.NotifyAllClusters(new EntitiesVersionUpdatedNotif()
                {
                    Versions = new List<Version>()
                        {
                            new Version()
                            {
                                VersionId = "BaseUser_" + user.BaseUserId + "_" + ServiceName,
                                Number = user.Version
                            },
                            new Version()
                            {
                                VersionId = "UserSecret_" + user.UserSecret.UserSecretId + "_" + ServiceName,
                                Number = user.UserSecret.Version
                            },
                            new Version()
                            {
                                VersionId = "Membership_" + user.Memberships[0].MembershipId + "_" + ServiceName,
                                Number = user.Memberships[0].Version
                            },
                            new Version()
                            {
                                VersionId = "MemberAccess_" + user.Memberships[0].MemberAccess.MemberAccessId + "_" +
                                            ServiceName,
                                Number = user.Memberships[0].MemberAccess.Version
                            },
                            new Version()
                            {
                                VersionId = "Complex_" + user.Memberships[0].Complex.ComplexId + "_" + ServiceName,
                                Number = user.Memberships[0].Complex.Version
                            },
                            new Version()
                            {
                                VersionId = "ComplexSecret_" + user.Memberships[0].Complex.ComplexSecret.ComplexId + "_" +
                                            ServiceName,
                                Number = user.Memberships[0].Complex.ComplexSecret.Version
                            },
                            new Version()
                            {
                                VersionId = "Room_" + user.Memberships[0].Complex.Rooms[0].RoomId + "_" + ServiceName,
                                Number = user.Memberships[0].Complex.Rooms[0].Version
                            }
                        }
                });
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<LogoutNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = dbContext.Sessions.Find(context.Message.Packet.Session.SessionId);

                if (session != null)
                {
                    dbContext.Sessions.Remove(session);
                    dbContext.SaveChanges();
                }
            }
            
            return Task.CompletedTask;
        }
        
        public async Task Consume(ConsumeContext<ComplexDeletionNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var complex = dbContext.Complexes.Find(context.Message.Packet.Complex.ComplexId);

                dbContext.Entry(complex).Collection(c => c.Members).Query().Include(mem => mem.MemberAccess).Load();
                dbContext.Entry(complex).Collection(c => c.Rooms).Query().Include(r => r.Workers).Load();
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

                var sessionIds =
                    (from r in complex.Rooms
                        from w in r.Workers
                        select dbContext.BaseUsers.Where(b => b.BaseUserId == w.BotId)
                            .Include(b => b.Sessions).First()
                            .Sessions.First().SessionId).ToList();

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

                foreach (var room in complex.Rooms)
                {
                    await dbContext.MessageSeens.Where(ms => ms.Message.RoomId == room.RoomId).DeleteFromQueryAsync();
                    await dbContext.Messages.Where(m => m.RoomId == room.RoomId).DeleteFromQueryAsync();
                    await dbContext.Workerships.Where(w => w.RoomId == room.RoomId).DeleteFromQueryAsync();
                }

                dbContext.SaveChanges();
            }

            using (var dbContext = new DatabaseContext())
            {
                var complex = dbContext.Complexes.Find(context.Message.Packet.Complex.ComplexId);
                dbContext.Entry(complex).Collection(c => c.Rooms).Load();

                foreach (var room in complex.Rooms)
                {
                    dbContext.BaseRooms.Remove(room);
                }

                dbContext.Entry(complex).Reference(c => c.ComplexSecret).Load();
                dbContext.ComplexSecrets.Remove(complex.ComplexSecret);
                dbContext.Complexes.Remove(complex);

                dbContext.SaveChanges();
            }
        }

        public Task Consume(ConsumeContext<PendingCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                dbContext.Add(context.Message.Packet.Pending);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ModuleProfileUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var mod = dbContext.Modules.Find(context.Message.Packet.Module.BaseUserId);
                mod.Title = context.Message.Packet.Module.Title;
                mod.Description = context.Message.Packet.Module.Description;
                mod.Avatar = context.Message.Packet.Module.Avatar;
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ModulePermissionCreated> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var modPermis = context.Message.Packet.ModulePermission;
                modPermis.Bot = (Bot)dbContext.BaseUsers.Find(modPermis.BotId);
                modPermis.Module = (Module) dbContext.BaseUsers.Find(modPermis.ModuleId);
                dbContext.ModulePermissions.Add(modPermis);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<MessageSeenNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var messageSeen = context.Message.Packet.MessageSeen;
                messageSeen.Message = dbContext.Messages.Find(messageSeen.MessageId);
                messageSeen.User = (User)dbContext.BaseUsers.Find(messageSeen.UserId);
                dbContext.MessageSeens.Add(messageSeen);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<WorkershipCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var workership = context.Message.Packet.Workership;
                workership.Room = dbContext.BaseRooms.Find(workership.RoomId);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<WorkershipDeletedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var workership = dbContext.Workerships.Find(context.Message.Packet.Workership.WorkershipId);
                dbContext.Workerships.Remove(workership);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<WorkershipUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var newWorkership = context.Message.Packet.Workership;
                var workership = dbContext.Workerships.Find(newWorkership.WorkershipId);
                workership.PosX = newWorkership.PosX;
                workership.PosY = newWorkership.PosY;
                workership.Width = newWorkership.Width;
                workership.Height = newWorkership.Height;
                workership.Angle = newWorkership.Angle;
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<BotCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var bot = context.Message.Packet.Bot;
                var botCreation = context.Message.Packet.BotCreation;
                var botSubscription = context.Message.Packet.BotSubscription;
                bot.Subscriptions.Clear();
                bot.Subscriptions.Add(botSubscription);
                botSubscription.Bot = bot;
                botCreation.Bot = bot;
                dbContext.AddRange(botCreation);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<BotProfileUpdatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var newBot = context.Message.Packet.Bot;
                var bot = (Bot)dbContext.BaseUsers.Find(newBot.BaseUserId);
                bot.Title = newBot.Title;
                bot.Avatar = newBot.Avatar;
                bot.Description = newBot.Description;
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ModuleCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var mod = context.Message.Packet.Module;
                var modCreation = context.Message.Packet.ModuleCreation;
                modCreation.Module = mod;
                dbContext.AddRange(modCreation);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<BotScreenShotCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var bss = context.Message.Packet.BotScreenShot;
                var bot = (Bot) dbContext.BaseUsers.Find(bss.Bot.BaseUserId);
                var photo = (Photo) dbContext.Files.Find(bss.Photo.FileId);
                bss.Bot = bot;
                bss.Photo = photo;
                dbContext.BotScreenShots.Add(bss);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<BotScreenShotRemovedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var bss = dbContext.BotScreenShots.Find(context.Message.Packet.BotScreenShot.BotScreenShotId);
                dbContext.BotScreenShots.Remove(bss);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}