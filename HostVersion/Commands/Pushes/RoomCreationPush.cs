using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class RoomCreationPush : Push
    {
        [ProtoMember(1)]
        public RoomCreationNotification Notif { get; set; }
    }
}