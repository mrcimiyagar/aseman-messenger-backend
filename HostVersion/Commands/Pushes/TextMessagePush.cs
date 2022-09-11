using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class TextMessagePush : Push
    {
        [ProtoMember(1)]
        public TextMessageNotification Notif { get; set; }
    }
}