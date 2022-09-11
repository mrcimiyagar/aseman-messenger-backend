using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using HostVersion.Commands.Pushes;
using HostVersion.DbContexts;
using HostVersion.Entities;
using HostVersion.Hubs;
using HostVersion.Models;
using HostVersion.Notifications;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace HostVersion.Utils
{
    public class Pusher
    {
        enum NotifSenderState { WaitingForAck, EmptyQueue }
        
        private readonly IHubContext<NotificationsHub> _notifsHub;
        private readonly ConcurrentDictionary<long, CancellationTokenSource> _cancellationTokens;
        private readonly ConcurrentDictionary<long, NotifSenderState> _senderStates;
        private readonly ConcurrentDictionary<long, object> _stateLocks;
        private readonly BlockingCollection<PusherPacket> _packetPipe;
        
        public Pusher(IHubContext<NotificationsHub> notifsHub)
        {
            _notifsHub = notifsHub;
            _cancellationTokens = new ConcurrentDictionary<long, CancellationTokenSource>();
            _senderStates = new ConcurrentDictionary<long, NotifSenderState>();
            _stateLocks = new ConcurrentDictionary<long, object>();
            _packetPipe = new BlockingCollection<PusherPacket>();
            
            new Thread(() =>
            {
                while (true)
                {
                    PusherPacket packet = _packetPipe.Take();

                    var sessionId = packet.SessionId;
                    
                    using (var mongo = new MongoLayer())
                    {
                        var coll = mongo.GetNotifsColl();

                        var n = coll.Find(notif => notif["SessionId"] == sessionId).FirstOrDefault();
                        
                        if (n == null)
                        {
                            _senderStates[sessionId] = NotifSenderState.EmptyQueue;
                            if (_cancellationTokens.TryGetValue(sessionId, out var cts))
                            {
                                try
                                {
                                    cts.Cancel();
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            continue;
                        }
                        using (var dbContext = new DatabaseContext())
                        {
                            var session = dbContext.Sessions.Find(sessionId);
                            PushNotification(session, n);
                        }
                    }
                }
            }).Start();
        }

        public void NotifyNotificationReceived(long sessionId)
        {
            lock (_stateLocks[sessionId])
            {
                _senderStates[sessionId] = NotifSenderState.EmptyQueue;   
            }
        }

        public void NextPush(long sessionId)
        {
            if (_senderStates.ContainsKey(sessionId))
            {
                lock (_stateLocks[sessionId])
                {
                    if (_senderStates[sessionId] == NotifSenderState.WaitingForAck) return;
                }
            }
            else
            {
                _stateLocks[sessionId] = new object();
            }
            
            _senderStates[sessionId] = NotifSenderState.WaitingForAck;
            
            try
            {
                _cancellationTokens[sessionId]?.Cancel();
            }
            catch (Exception)
            {
                // ignored
            }
            
            var cts = new CancellationTokenSource();
            _cancellationTokens[sessionId] = cts;

            Task.Run(async () =>
            {
                using (var dbContext = new DatabaseContext())
                {
                    var session = dbContext.Sessions.Find(sessionId);
                    if (!session.Online)
                    {
                        _senderStates[sessionId] = NotifSenderState.EmptyQueue;
                        return;
                    }
                    _packetPipe.TryAdd(new PusherPacket() {SessionId = sessionId});
                    await Task.Delay(20000, cts.Token);
                    _senderStates[sessionId] = NotifSenderState.EmptyQueue;
                    NextPush(sessionId);
                }
            }, cts.Token);
        }

        private async void PushNotification(Session session, BsonDocument n)
        {
            var type = n["Type"].ToString();

            if (type == typeof(BotViewResizedNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotViewResizedNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotViewResized", notif);
            }
            else if (type == typeof(BotLoadedNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotLoadedNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotLoaded", notif);
            }
            else if (type == typeof(UserRequestedBotPreviewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<UserRequestedBotPreviewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyUserRequestedBotPreview", notif);
            }
            else if (type == typeof(AudioMessageNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<AudioMessageNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyAudioMessageReceived", notif);
            }
            else if (type == typeof(BotAdditionToRoomNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotAdditionToRoomNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotAddedToRoom", notif);
            }
            else if (type == typeof(BotAnimatedBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotAnimatedBotViewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotAnimatedBotView", notif);
            }
            else if (type == typeof(BotRanCommandsOnBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotRanCommandsOnBotViewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotRanCommandsOnBotView", notif);
            }
            else if (type == typeof(BotRemovationFromRoomNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotRemovationFromRoomNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotRemovedFromRoom", notif);
            }
            else if (type == typeof(BotSentBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotSentBotViewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotSentBotView", notif);
            }
            else if (type == typeof(BotUpdatedBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotUpdatedBotViewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotUpdatedBotView", notif);
            }
            else if (type == typeof(ComplexDeletionNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<ComplexDeletionNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyComplexDeleted", notif);
            }
            else if (type == typeof(ContactCreationNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<ContactCreationNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyContactCreated", notif);
            }
            else if (type == typeof(InviteAcceptanceNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<InviteAcceptanceNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyInviteAccepted", notif);
            }
            else if (type == typeof(InviteCancellationNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<InviteCancellationNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyInviteCancelled", notif);
            }
            else if (type == typeof(InviteCreationNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<InviteCreationNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyInviteCreated", notif);
            }
            else if (type == typeof(InviteIgnoranceNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<InviteIgnoranceNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyInviteIgnored", notif);
            }
            else if (type == typeof(PhotoMessageNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<PhotoMessageNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyPhotoMessageReceived", notif);
            }
            else if (type == typeof(RoomDeletionNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<RoomDeletionNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyRoomDeleted", notif);
            }
            else if (type == typeof(ServiceMessageNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<ServiceMessageNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyServiceMessageReceived", notif);
            }
            else if (type == typeof(TextMessageNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<TextMessageNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyTextMessageReceived", notif);
            }
            else if (type == typeof(UserJointComplexNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<UserJointComplexNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyUserJointComplex", notif);
            }
            else if (type == typeof(UserRequestedBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<UserRequestedBotViewNotification>(n.ToJson());
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyUserRequestedBotView", notif);
            }
            else if (type == typeof(VideoMessageNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<VideoMessageNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyVideoMessageReceived", notif);
            }
            else if (type == typeof(MessageSeenNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<MessageSeenNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyMessageSeen", notif);
            }
            else if (type == typeof(MemberAccessUpdatedNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<MemberAccessUpdatedNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyMemberAccessUpdated", notif);
            }
            else if (type == typeof(UserClickedBotViewNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<UserClickedBotViewNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyUserClickedBotView", notif);
            }
            else if (type == typeof(BotPropertiesChangedNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<BotPropertiesChangedNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyBotPropertiesChanged", notif);
            }
            else if (type == typeof(ModulePermissionGrantedNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<ModulePermissionGrantedNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyModulePermissionGranted", notif);
            }
            else if (type == typeof(RoomCreationNotification).Name)
            {
                var notif = BsonSerializer.Deserialize<RoomCreationNotification>(n);
                notif.Type = notif.GetType().Name;
                await _notifsHub.Clients.Client(session.ConnectionId)
                    .SendAsync("NotifyRoomCreated", notif);
            }
        }
    }
}