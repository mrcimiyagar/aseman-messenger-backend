using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class AudioMessagePush : Push
    {
        [ProtoMember(1)]
        public AudioMessageNotification Notif { get; set; }
    }
}