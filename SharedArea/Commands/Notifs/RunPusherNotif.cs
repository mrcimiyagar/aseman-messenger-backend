using ProtoBuf;

namespace SharedArea.Commands.Notifs
{
    [ProtoContract]
    public class RunPusherNotif : Notification
    {
        public long SessionId { get; set; }
    }
}