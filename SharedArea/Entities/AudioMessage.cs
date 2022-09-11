using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class AudioMessage : Message
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("audioId")]
        public long? AudioId { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("audio")]
        public virtual Audio Audio { get; set; }

        public AudioMessage()
        {
            this.Type = "AudioMessage";
        }
    }
}