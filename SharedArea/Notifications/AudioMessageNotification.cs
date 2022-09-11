using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class AudioMessageNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("message")]
        public AudioMessage Message { get; set; }
    }
}