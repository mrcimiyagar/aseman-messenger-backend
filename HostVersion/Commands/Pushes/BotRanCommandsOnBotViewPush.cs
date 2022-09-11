using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotRanCommandsOnBotViewPush : Push
    {
        [ProtoMember(1)]
        public BotRanCommandsOnBotViewNotification Notif { get; set; }
    }
}