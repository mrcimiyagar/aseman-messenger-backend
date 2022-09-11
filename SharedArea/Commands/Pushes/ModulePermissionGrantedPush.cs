using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class ModulePermissionGrantedPush : Push
    {
        [ProtoMember(1)]
        public ModulePermissionGrantedNotification Notif { get; set; }
    }
}