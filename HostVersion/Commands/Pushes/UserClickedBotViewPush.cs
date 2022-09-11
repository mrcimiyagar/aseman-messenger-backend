using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class UserClickedBotViewPush : Push
    {
        [ProtoMember(1)]
        public UserClickedBotViewNotification Notif { get; set; }
    }
}