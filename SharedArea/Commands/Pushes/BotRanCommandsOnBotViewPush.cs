using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotRanCommandsOnBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotRanCommandsOnBotViewNotification Notif { get; set; }
    }
}