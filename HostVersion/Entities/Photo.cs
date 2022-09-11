using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Photo : File
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("width")]
        public int Width { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("height")]
        public int Height { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("isAvatar")]
        public bool IsAvatar { get; set; }

        public Photo()
        {
            this.Type = "Photo";
        }
    }
}