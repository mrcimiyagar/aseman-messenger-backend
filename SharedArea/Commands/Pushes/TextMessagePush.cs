using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class TextMessagePush : Push
    {
        [ProtoMember(1)]
        public TextMessageNotification Notif { get; set; }
    }
}