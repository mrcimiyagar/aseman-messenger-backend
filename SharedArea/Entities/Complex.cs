using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Complex
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("complexId")]
        public long ComplexId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("avatar")]
        public long Avatar { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("mode")]
        public short Mode { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("members")]
        public virtual List<Membership> Members { get; set; }
        [ProtoMember(6, AsReference = true)]
        [BsonElement, JsonProperty("rooms")]
        public virtual List<Room> Rooms { get; set; }
        [ProtoMember(7, AsReference = true)]
        [BsonElement, JsonProperty("singleRooms")]
        public virtual List<SingleRoom> SingleRooms { get; set; }
        [ProtoMember(8, AsReference = true)]
        [BsonElement, JsonProperty("invites")]
        public virtual List<Invite> Invites { get; set; }
        [ProtoMember(9, AsReference = true)]
        [JsonIgnore]
        public virtual ComplexSecret ComplexSecret { get; set; }
        [ProtoMember(10)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}