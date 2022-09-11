using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class BotSubscription
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("botSubscriptionId")]
        public long BotSubscriptionId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("subscriberId")]
        public long? SubscriberId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("subscriber")]
        public virtual User Subscriber { get; set; }
    }
}