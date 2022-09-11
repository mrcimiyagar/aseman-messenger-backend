using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class BotStoreBot
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botStoreBotId")]
        public long BotStoreBotId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("botStoreSectionId")]
        public long? BotStoreSectionId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("botStoreSection")]
        public virtual BotStoreSection BotStoreSection { get; set; }
    }
}