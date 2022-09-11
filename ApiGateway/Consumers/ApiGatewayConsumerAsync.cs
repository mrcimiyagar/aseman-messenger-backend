
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using MongoDB.Bson;
using Newtonsoft.Json;
using SharedArea;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Pushes;
using SharedArea.Commands.Requests.Internal;
using SharedArea.Entities;
using SharedArea.Notifications;
using Notification = SharedArea.Notifications.Notification;

namespace ApiGateway.Consumers
{
    public class ApiGatewayConsumerAsync : IConsumerAsync<ComplexDeletionPush>, IConsumerAsync<RoomDeletionPush>, IConsumerAsync<ContactCreationPush>
        , IConsumerAsync<ServiceMessagePush>, IConsumerAsync<InviteCreationPush>, IConsumerAsync<InviteCancellationPush>
        , IConsumerAsync<UserJointComplexPush>, IConsumerAsync<InviteAcceptancePush>, IConsumerAsync<InviteIgnoredPush>
        , IConsumerAsync<BotAdditionToRoomPush>, IConsumerAsync<BotRemovationFromRoomPush>, IConsumerAsync<TextMessagePush>
        , IConsumerAsync<PhotoMessagePush>, IConsumerAsync<AudioMessagePush>, IConsumerAsync<VideoMessagePush>
        , IConsumerAsync<UserRequestedBotViewPush>, IConsumerAsync<BotSentBotViewPush>, IConsumerAsync<BotUpdatedBotViewPush>
        , IConsumerAsync<BotAnimatedBotViewPush>, IConsumerAsync<BotRanCommandsOnBotViewPush>, IConsumerAsync<MessageSeenPush>
        , IConsumerAsync<MemberAccessUpdatedPush>, IConsumerAsync<UserClickedBotViewPush>, IConsumerAsync<BotPropertiesChangedPush>
        , IConsumerAsync<ModulePermissionGrantedPush>, IConsumerAsync<RoomCreationPush>
        , IConsumerAsync<SessionCreatedNotif>, IConsumerAsync<DeleteSessionsNotif>, IConsumerAsync<LogoutNotif>, IConsumerAsync<ConsolidateSessionRequest>
        , IConsumerAsync<AppCreatedNotif>, IConsumerAsync<UserRequestedBotPreviewPush>
    {
        public Task Consume(ConsumeContext<AppCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var app = context.Message.Packet.App;
                app.CreatorId = null;
                app.Creator = null;

                dbContext.Apps.Add(app);

                dbContext.SaveChanges();
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
        
        public async Task Consume(ConsumeContext<UserClickedBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new UserClickedBotViewNotification()
                    {
                        Complex = context.Message.Notif.Complex,
                        Room = context.Message.Notif.Room,
                        User = context.Message.Notif.User,
                        ControlId = context.Message.Notif.ControlId,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<UserClickedBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotPropertiesChangedPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotPropertiesChangedNotification()
                    {
                        Workership = context.Message.Notif.Workership,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotPropertiesChangedNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }
        
        public async Task Consume(ConsumeContext<MemberAccessUpdatedPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new MemberAccessUpdatedNotification()
                    {
                        MemberAccess = context.Message.Notif.MemberAccess,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<MemberAccessUpdatedNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }
        
        public  async Task Consume(ConsumeContext<ComplexDeletionPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new ComplexDeletionNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<ComplexDeletionNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<RoomDeletionPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new RoomDeletionNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<RoomDeletionNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<ContactCreationPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new ContactCreationNotification()
                    {
                        Contact = context.Message.Notif.Contact,
                        ComplexSecret = context.Message.Notif.ComplexSecret,
                        Message = context.Message.Notif.Message,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<ContactCreationNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<ServiceMessagePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new ServiceMessageNotification()
                    {
                        Message = context.Message.Notif.Message,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<ServiceMessageNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<InviteCreationPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new InviteCreationNotification()
                    {
                        Invite = context.Message.Notif.Invite,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<InviteCreationNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<InviteCancellationPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new InviteCancellationNotification()
                    {
                        Invite = context.Message.Notif.Invite,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<InviteCancellationNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<UserJointComplexPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new UserJointComplexNotification()
                    {
                        Membership = context.Message.Notif.Membership,
                        Message =  context.Message.Notif.Message,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<UserJointComplexNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<InviteAcceptancePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new InviteAcceptanceNotification()
                    {
                        Invite = context.Message.Notif.Invite,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<InviteAcceptanceNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<InviteIgnoredPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var s = dbContext.Sessions.Find(sessionId);

                    var notification = new InviteIgnoranceNotification()
                    {
                        Invite = context.Message.Notif.Invite,
                        SessionId = s.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<InviteIgnoranceNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotAdditionToRoomPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotAdditionToRoomNotification()
                    {
                        Workership = context.Message.Notif.Workership,
                        Bot = context.Message.Notif.Bot,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotAdditionToRoomNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotRemovationFromRoomPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotRemovationFromRoomNotification()
                    {
                        Workership = context.Message.Notif.Workership,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotRemovationFromRoomNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<TextMessagePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);
                    var notification = new TextMessageNotification()
                    {
                        Message = context.Message.Notif.Message,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<TextMessageNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<PhotoMessagePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new PhotoMessageNotification()
                    {
                        Message = context.Message.Notif.Message,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<PhotoMessageNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<AudioMessagePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new AudioMessageNotification()
                    {
                        Message = context.Message.Notif.Message,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<AudioMessageNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<VideoMessagePush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new VideoMessageNotification()
                    {
                        Message = context.Message.Notif.Message,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<VideoMessageNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<UserRequestedBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new UserRequestedBotViewNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        BotId = context.Message.Notif.BotId,
                        User = context.Message.Notif.User,
                        WorkerWidth = context.Message.Notif.WorkerWidth,
                        WorkerHeight = context.Message.Notif.WorkerHeight,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<UserRequestedBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }
        
        public async Task Consume(ConsumeContext<UserRequestedBotPreviewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new UserRequestedBotPreviewNotification()
                    {
                        BotId = context.Message.Notif.BotId,
                        User = context.Message.Notif.User,
                        WorkerWidth = context.Message.Notif.WorkerWidth,
                        WorkerHeight = context.Message.Notif.WorkerHeight,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<UserRequestedBotPreviewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotSentBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotSentBotViewNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        BotId = context.Message.Notif.BotId,
                        ViewData = context.Message.Notif.ViewData,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotSentBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotUpdatedBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotUpdatedBotViewNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        BotId = context.Message.Notif.BotId,
                        UpdateData = context.Message.Notif.UpdateData,
                        BatchData = context.Message.Notif.BatchData,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotUpdatedBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotAnimatedBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotAnimatedBotViewNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        BotId = context.Message.Notif.BotId,
                        AnimData = context.Message.Notif.AnimData,
                        BatchData = context.Message.Notif.BatchData,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotAnimatedBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<BotRanCommandsOnBotViewPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new BotRanCommandsOnBotViewNotification()
                    {
                        ComplexId = context.Message.Notif.ComplexId,
                        RoomId = context.Message.Notif.RoomId,
                        BotId = context.Message.Notif.BotId,
                        CommandsData = context.Message.Notif.CommandsData,
                        BatchData = context.Message.Notif.BatchData,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<BotRanCommandsOnBotViewNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public async Task Consume(ConsumeContext<MessageSeenPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new MessageSeenNotification()
                    {
                        MessageId = context.Message.Notif.MessageId,
                        MessageSeenCount = context.Message.Notif.MessageSeenCount,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<MessageSeenNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }
        
        public async Task Consume(ConsumeContext<ModulePermissionGrantedPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new ModulePermissionGrantedNotification()
                    {
                        ModulePermission = context.Message.Notif.ModulePermission,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<ModulePermissionGrantedNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }
        
        public async Task Consume(ConsumeContext<RoomCreationPush> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                foreach (var sessionId in context.Message.SessionIds)
                {
                    var session = dbContext.Sessions.Find(sessionId);

                    var notification = new RoomCreationNotification()
                    {
                        Room = context.Message.Notif.Room,
                        SingleRoom = context.Message.Notif.SingleRoom,
                        Message = context.Message.Notif.Message,
                        SessionId = session.SessionId
                    };

                    using (var mongo = new MongoLayer())
                    {
                        notification.Type = notification.GetType().Name;
                        var json = JsonConvert.SerializeObject(notification);
                        var obj = JsonConvert.DeserializeObject<RoomCreationNotification>(json);
                        await mongo.GetNotifsColl2().InsertOneAsync(obj);
                    }
                }

                foreach (var sessionId in context.Message.SessionIds)
                {
                    Startup.Pusher.NextPush(sessionId);
                }
            }
        }

        public Task Consume(ConsumeContext<SessionCreatedNotif> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = context.Message.Packet.Session;
                session.BaseUser = null;
                session.BaseUserId = null;
                
                dbContext.Sessions.Add(session);

                dbContext.SaveChanges();
            }
            
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<DeleteSessionsNotif> context)
        {
            var gSessions = context.Message.Packet.Sessions;

            using (var dbContext = new DatabaseContext())
            {
                var lSessions = new List<Session>();
                foreach (var gSession in gSessions)
                    lSessions.Add(dbContext.Sessions.Find(gSession.SessionId));

                dbContext.Sessions.RemoveRange(lSessions);

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ConsolidateSessionRequest> context)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = context.Message.Packet.Session;
                session.BaseUser = null;
                session.BaseUserId = null;
                
                dbContext.Sessions.Add(session);

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}