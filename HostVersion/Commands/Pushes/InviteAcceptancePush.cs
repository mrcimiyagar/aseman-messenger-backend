using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class InviteAcceptancePush : Push
    {
        [ProtoMember(1)]
        public InviteAcceptanceNotification Notif { get; set; }
    }
}