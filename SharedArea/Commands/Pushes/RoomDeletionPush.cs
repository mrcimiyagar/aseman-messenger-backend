using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class RoomDeletionPush : Push
    {
        [ProtoMember(1)]
        public RoomDeletionNotification Notif { get; set; }
    }
}