using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class RoomDeletionPush : Push
    {
        [ProtoMember(1)]
        public RoomDeletionNotification Notif { get; set; }
    }
}