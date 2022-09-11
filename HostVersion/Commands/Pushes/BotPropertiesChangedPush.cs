using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class BotPropertiesChangedPush : Push
    {
        [ProtoMember(1)]
        public BotPropertiesChangedNotification Notif { get; set; }
    }
}