using ProtoBuf;

namespace SharedArea.Commands.Notifs
{
    [ProtoContract]
    public class AppCreatedNotif : Notification
    {
        [ProtoMember(1)]
        public Entities.App App { get; set; }
    }
}