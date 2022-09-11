using HostVersion.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Notifications
{
    [ProtoContract]
    public class ModulePermissionGrantedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("modulePermission")]
        public ModulePermission ModulePermission { get; set; }
    }
}