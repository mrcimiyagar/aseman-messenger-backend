using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Pending
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("pendingId")]
        public long PendingId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("email")]
        public string Email { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("verifyCode")]
        public string VerifyCode { get; set; }
    }
}