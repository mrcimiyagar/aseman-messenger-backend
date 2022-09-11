using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class App
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("appId")]
        public long AppId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("token")]
        public string Token { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("creatorId")]
        public long? CreatorId { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("creator")]
        public virtual BaseUser Creator { get; set; }
    }
}