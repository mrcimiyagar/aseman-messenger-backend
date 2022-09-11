using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class AudioMessagePush : Push
    {
        [ProtoMember(1)]
        public AudioMessageNotification Notif { get; set; }
    }
}