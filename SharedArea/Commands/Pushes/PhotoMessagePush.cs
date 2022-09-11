using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class PhotoMessagePush : Push
    {
        [ProtoMember(1)]
        public PhotoMessageNotification Notif { get; set; }
    }
}