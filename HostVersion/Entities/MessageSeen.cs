using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class MessageSeen
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("messageSeenId")]
        public string MessageSeenId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("userId")]
        public long? UserId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("User")]
        public virtual User User { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("messageId")]
        public long? MessageId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("Message")]
        public virtual Message Message { get; set; }
    }
}