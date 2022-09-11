using ProtoBuf;

namespace HostVersion.Commands.Notifs
{
    [ProtoContract]
    public class RunPusherNotif : Notification
    {
        public long SessionId { get; set; }
    }
}