using ProtoBuf;

namespace HostVersion.Commands.Notifs
{
    [ProtoContract]
    public class AppCreatedNotif : Notification
    {
        [ProtoMember(1)]
        public Entities.App App { get; set; }
    }
}