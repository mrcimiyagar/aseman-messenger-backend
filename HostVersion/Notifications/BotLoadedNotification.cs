using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class BotLoadedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("userId")]
        public long UserId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("roomId")]
        public long RoomId { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("complexId")]
        public long ComplexId { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("botWindowMode")]
        public bool BotWindowMode { get; set; }
    }
}