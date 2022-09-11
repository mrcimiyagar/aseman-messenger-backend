using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class MemberAccessUpdatedPush : Push
    {
        [ProtoMember(1)]
        public MemberAccessUpdatedNotification Notif { get; set; }
    }
}