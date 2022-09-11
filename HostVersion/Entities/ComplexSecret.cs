using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class ComplexSecret
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("complexSecretId")]
        public long ComplexSecretId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("complexId")]
        public long? ComplexId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("complex")]
        public virtual Complex Complex { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("adminId")]
        public long? AdminId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("admin")]
        public virtual User Admin { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}