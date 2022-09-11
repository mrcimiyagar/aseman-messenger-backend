using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotAdditionToRoomPush : Push
    {
        [ProtoMember(1)]
        public BotAdditionToRoomNotification Notif { get; set; }
    }
}