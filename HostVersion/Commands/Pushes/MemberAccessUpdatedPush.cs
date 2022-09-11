using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class MemberAccessUpdatedPush : Push
    {
        [ProtoMember(1)]
        public MemberAccessUpdatedNotification Notif { get; set; }
    }
}