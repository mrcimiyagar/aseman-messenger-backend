using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class BotPropertiesChangedPush : Push
    {
        [ProtoMember(1)]
        public BotPropertiesChangedNotification Notif { get; set; }
    }
}