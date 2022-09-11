using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class BotAnimatedBotViewNotification : Notification
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
        [BsonElement, JsonProperty("animData")]
        public string AnimData { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("batchData")]
        public bool BatchData { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("botWindowMode")]
        public bool BotWindowMode { get; set; }
    }
}