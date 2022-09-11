using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class ServiceMessagePush : Push
    {
        [ProtoMember(1)]
        public ServiceMessageNotification Notif { get; set; }
    }
}