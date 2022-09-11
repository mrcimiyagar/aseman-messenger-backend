using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class MessageSeenPush : Push
    {
        [ProtoMember(1)]
        public MessageSeenNotification Notif { get; set; }
    }
}