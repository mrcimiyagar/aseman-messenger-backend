using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class BotAdditionToRoomNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("workership")]
        public Workership Workership { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("bot")]
        public Bot Bot { get; set; }
    }
}