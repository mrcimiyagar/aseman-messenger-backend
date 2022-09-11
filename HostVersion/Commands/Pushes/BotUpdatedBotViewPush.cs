using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotUpdatedBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotUpdatedBotViewNotification Notif { get; set; }
    }
}