using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class UserJointComplexPush : Push
    {
        [ProtoMember(1)]
        public UserJointComplexNotification Notif { get; set; }
    }
}