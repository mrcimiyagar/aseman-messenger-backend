using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class ModuleSecret
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("moduleSecretId")]
        public long ModuleSecretId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("moduleId")]
        public long? ModuleId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("module")]
        public virtual Module Module { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("creatorId")]
        public long? CreatorId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("creator")]
        public virtual BaseUser Creator { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("token")]
        public string Token { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("serverAddress")]
        public string ServerAddress { get; set; }
    }
}