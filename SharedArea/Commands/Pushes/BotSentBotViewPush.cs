using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotSentBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotSentBotViewNotification Notif { get; set; }
    }
}