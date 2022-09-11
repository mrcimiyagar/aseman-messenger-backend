using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class MemberAccess
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("memberAccessId")]
        public long MemberAccessId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("canCreateMessage")]
        public bool CanCreateMessage { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("canSendInvite")]
        public bool CanSendInvite { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("canModifyWorkers")]
        public bool CanModifyWorkers { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("canUpdateProfiles")]
        public bool CanUpdateProfiles { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("canModifyAccess")]
        public bool CanModifyAccess { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("membershipId")]
        public long? MembershipId { get; set; }
        [ProtoMember(8, AsReference = true)]
        [BsonElement, JsonProperty("membership")]
        public virtual Membership Membership { get; set; }
        [ProtoMember(9)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}