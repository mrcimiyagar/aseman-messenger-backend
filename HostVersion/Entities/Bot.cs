using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Bot : BaseUser
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("description")]
        public string Description { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("viewURL")]
        public string ViewURL { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("botSecret")]
        public virtual BotSecret BotSecret { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("workerships")]
        public virtual List<Workership> Workerships { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("subscriptions")]
        public virtual List<BotSubscription> Subscriptions { get; set; }
        [ProtoMember(6, AsReference = true)]
        [BsonElement, JsonProperty("modulePermissions")]
        public virtual List<ModulePermission> ModulePermissions { get; set; }
        [ProtoMember(7, AsReference = true)]
        [BsonElement, JsonProperty("screenshots")]
        public virtual List<BotScreenShot> ScreenShots { get; set; }

        public Bot()
        {
            this.Type = "Bot";
        }
    }
}