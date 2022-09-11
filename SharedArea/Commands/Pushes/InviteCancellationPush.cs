using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class InviteCancellationPush : Push
    {
        [ProtoMember(1)]
        public InviteCancellationNotification Notif { get; set; }
    }
}