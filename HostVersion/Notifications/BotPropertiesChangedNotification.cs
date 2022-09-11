using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class BotPropertiesChangedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("workership")]
        public Workership Workership { get; set; }
    }
}