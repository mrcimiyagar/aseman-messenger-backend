using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class InviteAcceptancePush : Push
    {
        [ProtoMember(1)]
        public InviteAcceptanceNotification Notif { get; set; }
    }
}