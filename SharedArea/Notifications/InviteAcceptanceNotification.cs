using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class InviteAcceptanceNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("inviteId")]
        public long? InviteId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("invite")]
        public virtual Invite Invite { get; set; }
    }
}