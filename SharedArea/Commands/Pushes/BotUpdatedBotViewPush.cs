using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotUpdatedBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotUpdatedBotViewNotification Notif { get; set; }
    }
}