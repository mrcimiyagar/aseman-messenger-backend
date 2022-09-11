using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class InviteCancellationPush : Push
    {
        [ProtoMember(1)]
        public InviteCancellationNotification Notif { get; set; }
    }
}