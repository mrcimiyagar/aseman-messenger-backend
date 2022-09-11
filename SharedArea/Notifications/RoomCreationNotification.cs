using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class RoomCreationNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("room")]
        public Room Room { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("singleRoom")]
        public SingleRoom SingleRoom { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("message")]
        public ServiceMessage Message { get; set; }
    }
}