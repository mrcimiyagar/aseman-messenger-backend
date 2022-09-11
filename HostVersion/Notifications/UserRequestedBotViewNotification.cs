using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class UserRequestedBotViewNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("complexId")]
        public long ComplexId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("roomId")]
        public long RoomId { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("botId")]
        public long BotId { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("user")]
        public User User { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("workerWidth")]
        public int WorkerWidth { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("workerHeight")]
        public int WorkerHeight { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("botWindowMode")]
        public bool BotWindowMode { get; set; }
    }
}