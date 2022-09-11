using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Module : BaseUser
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("description")]
        public string Description { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("modulePermissions")]
        public virtual List<ModulePermission> ModulePermissions { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("moduleSecret")]
        public virtual ModuleSecret ModuleSecret { get; set; }

        public Module() : base()
        {
            this.Type = "Module";
        }
    }
}