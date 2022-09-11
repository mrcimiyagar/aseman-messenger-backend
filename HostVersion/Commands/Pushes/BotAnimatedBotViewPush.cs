using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotAnimatedBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotAnimatedBotViewNotification Notif { get; set; }
    }
}