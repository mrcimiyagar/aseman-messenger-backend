using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Audio : File
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("duration")]
        public long Duration { get; set; }

        public Audio()
        {
            this.Type = "Audio";
        }
    }
}