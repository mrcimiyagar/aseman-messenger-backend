using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class VideoMessagePush : Push
    {
        [ProtoMember(1)]
        public VideoMessageNotification Notif { get; set; }
    }
}