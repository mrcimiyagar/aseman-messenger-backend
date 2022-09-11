using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class InviteIgnoredPush : Push
    {
        [ProtoMember(1)]
        public InviteIgnoranceNotification Notif { get; set; }
    }
}