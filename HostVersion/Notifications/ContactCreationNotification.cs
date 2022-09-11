using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class ContactCreationNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("contactId")]
        public long? ContactId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("contact")]
        public virtual Contact Contact { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("complexSecret")]
        public virtual ComplexSecret ComplexSecret { get; set; }
        [ProtoMember(4)]
        [BsonElement, JsonProperty("message")]
        public virtual ServiceMessage Message { get; set; }
    }
}