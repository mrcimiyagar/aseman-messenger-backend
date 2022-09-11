using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class ComplexDeletionPush : Push
    {
        [ProtoMember(1)]
        public ComplexDeletionNotification Notif { get; set; }
    }
}