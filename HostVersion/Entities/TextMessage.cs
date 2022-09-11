using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class TextMessage : Message
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("text")]
        public string Text { get; set; }

        public TextMessage()
        {
            this.Type = "TextMessage";
        }
    }
}