using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class UserSecret
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("userSecretId")]
        public long UserSecretId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("homeId")]
        public long? HomeId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("home")]
        public virtual Complex Home { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("email")]
        public string Email { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("userId")]
        public long? UserId { get; set; }
        [ProtoMember(6, AsReference = true)]
        [BsonElement, JsonProperty("user")]
        public virtual User User { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}