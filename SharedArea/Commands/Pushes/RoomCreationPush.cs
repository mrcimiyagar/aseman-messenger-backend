using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class RoomCreationPush : Push
    {
        [ProtoMember(1)]
        public RoomCreationNotification Notif { get; set; }
    }
}