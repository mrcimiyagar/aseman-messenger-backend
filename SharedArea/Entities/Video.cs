using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Video : File
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("duration")]
        public long Duration { get; set; }

        public Video()
        {
            this.Type = "Video";
        }
    }
}