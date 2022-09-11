using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotRemovationFromRoomPush : Push
    {
        [ProtoMember(1)]
        public BotRemovationFromRoomNotification Notif { get; set; }
    }
}