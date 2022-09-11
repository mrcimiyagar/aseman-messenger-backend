using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class BotScreenShot
    {
        [Key]
        [ProtoMember(1)]
        [BsonElement, JsonProperty("botScreenShotId")]
        public long BotScreenShotId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("photoId")]
        public long? PhotoId { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("photo")]
        public virtual Photo Photo { get; set; }
    }
}