using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class MemberAccessUpdatedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("memberAccess")]
        public MemberAccess MemberAccess { get; set; }
    }
}