using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class ModulePermissionGrantedPush : Push
    {
        [ProtoMember(1)]
        public ModulePermissionGrantedNotification Notif { get; set; }
    }
}