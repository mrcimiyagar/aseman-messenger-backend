using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class UserJointComplexNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("membershipId")]
        public long? MembershipId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("membership")]
        public Membership Membership { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("message")]
        public ServiceMessage Message { get; set; }
    }
}