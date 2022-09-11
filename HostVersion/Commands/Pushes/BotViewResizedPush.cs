using HostVersion.Notifications;

namespace HostVersion.Commands.Pushes
{
    public class BotViewResizedPush : Push
    {
        public BotViewResizedNotification Notif { get; set; }
    }
}