using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class ServiceMessagePush : Push
    {
        [ProtoMember(1)]
        public ServiceMessageNotification Notif { get; set; }
    }
}