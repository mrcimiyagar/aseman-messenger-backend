using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class VideoMessagePush : Push
    {
        [ProtoMember(1)]
        public VideoMessageNotification Notif { get; set; }
    }
}