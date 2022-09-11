using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class BotSentBotViewNotification : Notification
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
        [BsonElement, JsonProperty("viewData")]
        public string ViewData { get; set; }
    }
}