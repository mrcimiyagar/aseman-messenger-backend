using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    [ProtoInclude(1, typeof(TextMessage))]
    [ProtoInclude(2, typeof(PhotoMessage))]
    [ProtoInclude(3, typeof(AudioMessage))]
    [ProtoInclude(4, typeof(VideoMessage))]
    [ProtoInclude(5, typeof(ServiceMessage))]
    [BsonKnownTypes(typeof(TextMessage), typeof(PhotoMessage), typeof(AudioMessage), typeof(VideoMessage), typeof(ServiceMessage))]
    public class Message
    {
        [ProtoMember(101)]
        [Key]
        [BsonElement, JsonProperty("messageId")]
        public long MessageId { get; set; }
        [ProtoMember(102)]
        [BsonElement, JsonProperty("time")]
        public long Time { get; set; }
        [ProtoMember(103)]
        [BsonElement, JsonProperty("type")]
        public string Type { get; set; }
        [ProtoMember(104)]
        [BsonElement, JsonProperty("authorId")]
        public long? AuthorId { get; set; }
        [ProtoMember(105, AsReference = true)]
        [BsonElement, JsonProperty("author")]
        public virtual BaseUser Author { get; set; }
        [ProtoMember(106)]
        [BsonElement, JsonProperty("roomId")]
        public long? RoomId { get; set; }
        [ProtoMember(107, AsReference = true)]
        [BsonElement, JsonProperty("room")]
        public virtual BaseRoom Room { get; set; }
        [ProtoMember(108, AsReference = true)]
        [BsonElement, JsonProperty("messageSeens")]
        public virtual List<MessageSeen> MessageSeens { get; set; }
        [ProtoMember(109)]
        [NotMapped]
        [BsonElement, JsonProperty("seenByMe")]
        public bool SeenByMe { get; set; }
        [ProtoMember(110)]
        [NotMapped]
        [BsonElement, JsonProperty("seenCount")]
        public long SeenCount { get; set; }
    }
}