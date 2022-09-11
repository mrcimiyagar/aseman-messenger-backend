using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class InviteCreationNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("inviteId")]
        public long? InviteId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("invite")]
        public virtual Invite Invite { get; set; }
    }
}