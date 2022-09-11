using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class PhotoMessagePush : Push
    {
        [ProtoMember(1)]
        public PhotoMessageNotification Notif { get; set; }
    }
}