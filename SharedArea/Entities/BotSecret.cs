using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class BotSecret
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botSecretId")]
        public long BotSecretId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("token")]
        public string Token { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("creatorId")]
        public long? CreatorId { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("creator")]
        public virtual User Creator { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(6, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
    }
}