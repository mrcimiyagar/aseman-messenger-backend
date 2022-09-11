using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class UserClickedBotViewPush : Push
    {
        [ProtoMember(1)]
        public UserClickedBotViewNotification Notif { get; set; }
    }
}