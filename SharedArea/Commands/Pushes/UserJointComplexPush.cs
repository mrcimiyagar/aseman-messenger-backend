using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class UserJointComplexPush : Push
    {
        [ProtoMember(1)]
        public UserJointComplexNotification Notif { get; set; }
    }
}