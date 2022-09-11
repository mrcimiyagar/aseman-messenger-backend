using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Session
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("sessionId")]
        public long SessionId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("token")]
        public string Token { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("baseUserId")]
        public long? BaseUserId { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("baseUser")]
        public virtual BaseUser BaseUser { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("online")]
        public bool Online { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("connectionId")]
        public string ConnectionId { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}