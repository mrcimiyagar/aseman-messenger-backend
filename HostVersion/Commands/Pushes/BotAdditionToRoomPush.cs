using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotAdditionToRoomPush : Push
    {
        [ProtoMember(1)]
        public BotAdditionToRoomNotification Notif { get; set; }
    }
}