using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class FileUsage
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("fileUsageId")]
        public long FileUsageId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("fileId")]
        public long? FileId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("file")]
        public virtual File File { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("roomId")]
        public long? RoomId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("room")]
        public virtual BaseRoom Room { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("fileUsageType")]
        public short FileUsageType { get; set; }
    }
}