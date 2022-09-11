using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotAnimatedBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotAnimatedBotViewNotification Notif { get; set; }
    }
}