using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class VideoMessage : Message
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("videoId")]
        public long? VideoId { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("video")]
        public virtual Video Video { get; set; }

        public VideoMessage()
        {
            this.Type = "VideoMessage";
        }
    }
}