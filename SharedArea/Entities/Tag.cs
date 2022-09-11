using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Tag
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("tagId")]
        public long TagId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("fileTags")]
        public List<FileTag> FileTags { get; set; }
    }
}