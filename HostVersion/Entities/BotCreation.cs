using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class BotCreation
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botCreationId")]
        public long BotCreationId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("creatorId")]
        public long? CreatorId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("creator")]
        public virtual User Creator { get; set; }
    }
}