using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotSentBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotSentBotViewNotification Notif { get; set; }
    }
}