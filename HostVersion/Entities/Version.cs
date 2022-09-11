using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Version
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("versionId")]
        public string VersionId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("number")]
        public long Number { get; set; }
    }
}