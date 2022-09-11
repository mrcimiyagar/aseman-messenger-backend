using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class ModulePermission
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("modulePermissionId")]
        public long ModulePermissionId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("moduleId")]
        public long? ModuleId { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("module")]
        public virtual Module Module { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("botId")]
        public long? BotId { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("bot")]
        public virtual Bot Bot { get; set; }
    }
}