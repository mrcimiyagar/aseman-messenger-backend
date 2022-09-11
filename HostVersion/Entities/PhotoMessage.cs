using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class PhotoMessage : Message
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("photoId")]
        public long? PhotoId { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("photo")]
        public virtual Photo Photo { get; set; }

        public PhotoMessage()
        {
            this.Type = "PhotoMessage";
        }
    }
}