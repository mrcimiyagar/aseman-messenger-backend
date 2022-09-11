using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class InviteIgnoredPush : Push
    {
        [ProtoMember(1)]
        public InviteIgnoranceNotification Notif { get; set; }
    }
}