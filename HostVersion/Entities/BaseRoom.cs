using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Room))]
    [ProtoInclude(2, typeof(SingleRoom))]
    [BsonKnownTypes(typeof(Room), typeof(SingleRoom))]
    public class BaseRoom
    {
        [ProtoMember(101)]
        [Key]
        [BsonElement, JsonProperty("roomId")]
        public long RoomId { get; set; }
        [ProtoMember(102)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(103)]
        [BsonElement, JsonProperty("avatar")]
        public long Avatar { get; set; }
        [ProtoMember(104)]
        [BsonElement, JsonProperty("complexId")]
        public long? ComplexId { get; set; }
        [ProtoMember(105, AsReference = true)]
        [BsonElement, JsonProperty("complex")]
        public virtual Complex Complex { get; set; }
        [ProtoMember(106, AsReference = true)]
        [BsonElement, JsonProperty("workers")]
        public virtual List<Workership> Workers { get; set; }
        [ProtoMember(107, AsReference = true)]
        [BsonElement, JsonProperty("messages")]
        public virtual List<Message> Messages { get; set; }
        [ProtoMember(108, AsReference = true)]
        [BsonElement, JsonProperty("files")]
        public virtual List<FileUsage> Files { get; set; }
        [ProtoMember(109)]
        [BsonElement, JsonProperty("type")]
        public string Type { get; set; }
        [ProtoMember(110, AsReference = true)]
        [NotMapped]
        [BsonElement, JsonProperty("lastAction")]
        public virtual Message LastAction { get; set; }
        [ProtoMember(111)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
        [ProtoMember(112)]
        [BsonElement, JsonProperty("backgroundUrl")]
        public string BackgroundUrl { get; set; }

        public BaseRoom()
        {
            this.Type = "BaseRoom";
        }
    }
}