using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class PhotoMessageNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("message")]
        public PhotoMessage Message { get; set; }
    }
}