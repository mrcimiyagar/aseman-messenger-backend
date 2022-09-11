using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class UserClickedBotViewNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("complex")]
        public Complex Complex { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("room")]
        public BaseRoom Room { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("user")]
        public User User { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("controlId")]
        public string ControlId { get; set; }
    }
}