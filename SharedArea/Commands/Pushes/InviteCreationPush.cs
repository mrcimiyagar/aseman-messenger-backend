using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class InviteCreationPush : Push
    {
        [ProtoMember(1)]
        public InviteCreationNotification Notif { get; set; }
    }
}