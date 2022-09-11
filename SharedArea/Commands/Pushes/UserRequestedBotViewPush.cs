using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class UserRequestedBotViewPush : Push
    {
        [ProtoMember(1)]
        public UserRequestedBotViewNotification Notif { get; set; }
    }
}