using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotRemovationFromRoomPush : Push
    {
        [ProtoMember(1)]
        public BotRemovationFromRoomNotification Notif { get; set; }
    }
}