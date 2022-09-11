using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class MessageSeenPush : Push
    {
        [ProtoMember(1)]
        public MessageSeenNotification Notif { get; set; }
    }
}