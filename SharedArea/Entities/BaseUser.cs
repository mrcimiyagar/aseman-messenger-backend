using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    [ProtoInclude(1, typeof(User))]
    [ProtoInclude(2, typeof(Bot))]
    [ProtoInclude(3, typeof(Module))]
    [BsonDiscriminator(RootClass = true, Required = true)]
    [BsonKnownTypes(typeof(User), typeof(Bot), typeof(Module))]
    public class BaseUser
    {
        [ProtoMember(101)]
        [Key]
        [BsonElement, JsonProperty("baseUserId")]
        public long BaseUserId { get; set; }
        [ProtoMember(102)]
        [BsonElement, JsonProperty("title")]
        public string Title { get; set; }
        [ProtoMember(103)]
        [BsonElement, JsonProperty("avatar")]
        public long Avatar { get; set; }
        [ProtoMember(104, AsReference = true)]
        [BsonElement, JsonProperty("sessions")]
        public virtual List<Session> Sessions { get; set; }
        [ProtoMember(105)]
        [BsonElement, JsonProperty("type")]
        public string Type { get; set; }
        [ProtoMember(106)]
        [BsonElement, JsonProperty("version")]
        public long Version { get; set; }
    }
}