using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Notifications
{
    [ProtoContract]
    public class ModulePermissionGrantedNotification : Notification
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("modulePermission")]
        public ModulePermission ModulePermission { get; set; }
    }
}