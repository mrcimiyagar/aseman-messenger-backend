using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class User : BaseUser
    {
        [ProtoMember(1, AsReference = true)]
        [BsonElement, JsonProperty("memberships")]
        public virtual List<Membership> Memberships { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("contacts")]
        public virtual List<Contact> Contacts { get; set; }
        [ProtoMember(3, AsReference = true)]
        [BsonElement, JsonProperty("peereds")]
        public virtual List<Contact> Peereds { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("invites")]
        public virtual List<Invite> Invites { get; set; }
        [ProtoMember(5, AsReference = true)]
        [BsonElement, JsonProperty("createdBots")]
        public virtual List<BotCreation> CreatedBots { get; set; }
        [ProtoMember(6, AsReference = true)]
        [BsonElement, JsonProperty("createdModules")]
        public virtual List<ModuleCreation> CreatedModules { get; set; }
        [ProtoMember(7, AsReference = true)]
        [BsonElement, JsonProperty("subscribedBots")]
        public virtual List<BotSubscription> SubscribedBots { get; set; }
        [ProtoMember(8, AsReference = true)]
        [BsonElement, JsonProperty("messageSeens")]
        public virtual List<MessageSeen> MessageSeens { get; set; }
        [ProtoMember(9, AsReference = true)]
        [BsonElement, JsonProperty("apps")]
        public virtual List<App> Apps { get; set; }
        [ProtoMember(10, AsReference = true)]
        [JsonIgnore]
        public virtual UserSecret UserSecret { get; set; }

        public User()
        {
            this.Type = "User";
        }
    }
}