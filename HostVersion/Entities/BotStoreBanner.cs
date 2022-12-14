using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class BotStoreBanner
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botStoreBannerId")]
        public long BotStoreBannerId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("imagePath")]
        public string ImagePath { get; set; }
    }
}