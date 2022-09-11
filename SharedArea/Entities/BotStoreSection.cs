using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class BotStoreSection
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botStoreSectionId")]
        public long BotStoreSectionId { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("botStoreBots")]
        public virtual List<BotStoreBot> BotStoreBots { get; set; }
    }
}