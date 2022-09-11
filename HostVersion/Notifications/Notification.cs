using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AudioMessageNotification))]
    [ProtoInclude(2, typeof(BotAdditionToRoomNotification))]
    [ProtoInclude(3, typeof(BotAnimatedBotViewNotification))]
    [ProtoInclude(4, typeof(BotPropertiesChangedNotification))]
    [ProtoInclude(5, typeof(BotRanCommandsOnBotViewNotification))]
    [ProtoInclude(6, typeof(BotRemovationFromRoomNotification))]
    [ProtoInclude(7, typeof(BotSentBotViewNotification))]
    [ProtoInclude(8, typeof(BotUpdatedBotViewNotification))]
    [ProtoInclude(9, typeof(ComplexDeletionNotification))]
    [ProtoInclude(10, typeof(ContactCreationNotification))]
    [ProtoInclude(11, typeof(InviteAcceptanceNotification))]
    [ProtoInclude(12, typeof(InviteCancellationNotification))]
    [ProtoInclude(13, typeof(InviteCreationNotification))]
    [ProtoInclude(14, typeof(InviteIgnoranceNotification))]
    [ProtoInclude(15, typeof(MemberAccessUpdatedNotification))]
    [ProtoInclude(16, typeof(MessageSeenNotification))]
    [ProtoInclude(17, typeof(ModulePermissionGrantedNotification))]
    [ProtoInclude(18, typeof(PhotoMessageNotification))]
    [ProtoInclude(19, typeof(RoomCreationNotification))]
    [ProtoInclude(20, typeof(RoomDeletionNotification))]
    [ProtoInclude(21, typeof(ServiceMessageNotification))]
    [ProtoInclude(22, typeof(TextMessageNotification))]
    [ProtoInclude(23, typeof(UserClickedBotViewNotification))]
    [ProtoInclude(24, typeof(UserJointComplexNotification))]
    [ProtoInclude(25, typeof(UserRequestedBotViewNotification))]
    [ProtoInclude(26, typeof(UserRequestedBotPreviewNotification))]
    
    public class Notification
    {
        [ProtoMember(101)]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement, JsonProperty("notificationId")]
        public string NotificationId { get; set; }
        [ProtoMember(102)]
        [BsonElement, JsonProperty("type")]
        public string Type { get; set; }
        [ProtoMember(103)]
        [BsonElement, JsonProperty("sessionId")]
        public long SessionId { get; set; }
    }
}