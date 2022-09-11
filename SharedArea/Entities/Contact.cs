using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Contact
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("contactId")]
        public long ContactId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("complexId")]
        public long? ComplexId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("complex")]
        public virtual Complex Complex { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("userId")]
        public long? UserId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("user")]
        public virtual User User { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("peerId")]
        public long? PeerId { get; set; }
        [ProtoMember(7, AsReference = true)]
        [BsonElement, JsonProperty("peer")]
        public virtual User Peer { get; set; }
        [ProtoMember(8)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}