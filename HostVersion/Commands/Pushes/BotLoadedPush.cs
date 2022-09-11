using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotLoadedPush : Push
    {
        [ProtoMember(1)]
        public BotLoadedNotification Notif { get; set; }
    }
}