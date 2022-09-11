using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class BotPropertiesChangedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("workership")]
        public Workership Workership { get; set; }
    }
}