using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class TextMessageNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("message")]
        public TextMessage Message { get; set; }
    }
}