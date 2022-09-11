using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class ComplexDeletionNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("complexId")]
        public long ComplexId { get; set; }
    }
}