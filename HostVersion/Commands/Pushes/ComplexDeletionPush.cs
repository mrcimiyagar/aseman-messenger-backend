using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class ComplexDeletionPush : Push
    {
        [ProtoMember(1)]
        public ComplexDeletionNotification Notif { get; set; }
    }
}