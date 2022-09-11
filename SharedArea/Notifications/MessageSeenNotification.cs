using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class MessageSeenNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("messageId")]
        public long MessageId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("messageSeenCount")]
        public long MessageSeenCount { get; set; }
    }
}