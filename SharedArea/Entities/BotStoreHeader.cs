using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class BotStoreHeader
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botStoreHeaderId")]
        public long BotStoreHeaderId { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("banners")]
        public virtual List<BotStoreBanner> Banners { get; set; }
    }
}