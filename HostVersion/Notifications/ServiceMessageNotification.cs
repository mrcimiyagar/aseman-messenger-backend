using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class ServiceMessageNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("message")]
        public ServiceMessage Message { get; set; }
    }
}