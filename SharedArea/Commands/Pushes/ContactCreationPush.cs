using ProtoBuf;
using SharedArea.Notifications;

namespace SharedArea.Commands.Pushes
{
    [ProtoContract]
    public class ContactCreationPush : Push
    {
        [ProtoMember(1)]
        public ContactCreationNotification Notif { get; set; }
    }
}