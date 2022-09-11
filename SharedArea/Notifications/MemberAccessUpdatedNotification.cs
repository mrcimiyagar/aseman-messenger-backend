using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class MemberAccessUpdatedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("memberAccess")]
        public MemberAccess MemberAccess { get; set; }
    }
}