using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class FileTag
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("fileTagId")]
        public long FileTagId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("fileId")] 
        public long FileId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("file")] 
        public virtual File File { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("tagId")]
        public long TagId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("tag")]
        public virtual Tag Tag { get; set; }
    }
}