using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class UserRequestedBotPreviewPush : Push
    {
        [ProtoMember(1)]
        public UserRequestedBotPreviewNotification Notif { get; set; }
    }
}