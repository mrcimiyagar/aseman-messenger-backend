using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class ContactCreationPush : Push
    {
        [ProtoMember(1)]
        public ContactCreationNotification Notif { get; set; }
    }
}