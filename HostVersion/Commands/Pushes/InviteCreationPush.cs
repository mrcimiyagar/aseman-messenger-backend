using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class InviteCreationPush : Push
    {
        [ProtoMember(1)]
        public InviteCreationNotification Notif { get; set; }
    }
}