using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class TextMessageNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("message")]
        public TextMessage Message { get; set; }
    }
}