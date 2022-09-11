using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Photo))]
    [ProtoInclude(2, typeof(Audio))]
    [ProtoInclude(3, typeof(Video))]
    [ProtoInclude(4, typeof(Document))]
    [BsonKnownTypes(typeof(Photo), typeof(Audio), typeof(Video), typeof(Document))]
    public class File
    {
        [ProtoMember(101)]
        [Key]
        [BsonElement, JsonProperty("fileId")]
        public long FileId { get; set; }
        [ProtoMember(102)]
        [BsonElement, JsonProperty("size")]
        public long Size { get; set; }
        [ProtoMember(103)]
        [BsonElement, JsonProperty("FileTransferFinished")]
        public bool FileTransferFinished { get; set; }
        [ProtoMember(104)]
        [BsonElement, JsonProperty("isPublic")]
        public bool IsPublic { get; set; }
        [ProtoMember(105)]
        [BsonElement, JsonProperty("asRawFile")]
        public bool AsRawFile { get; set; }
        [ProtoMember(106)]
        [BsonElement, JsonProperty("type")]
        public string Type { get; set; }
        [ProtoMember(107, AsReference = true)]
        [BsonElement, JsonProperty("fileTags")]
        public virtual List<FileTag> FileTags { get; set; }
        [ProtoMember(108, AsReference = true)]
        [BsonElement, JsonProperty("fileUsages")]
        public virtual List<FileUsage> FileUsages { get; set; }
        [ProtoMember(109)]
        [JsonIgnore]
        public long? UploaderId { get; set; }
        [ProtoMember(110, AsReference = true)]
        [JsonIgnore]
        public virtual BaseUser Uploader { get; set; }
    }
}