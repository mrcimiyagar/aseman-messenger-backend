using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class UserRequestedBotPreviewPush : Push
    {
        [ProtoMember(1)]
        public UserRequestedBotPreviewNotification Notif { get; set; }
    }
}