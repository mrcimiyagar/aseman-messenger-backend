﻿using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    public class UserRequestedBotViewPush : Push
    {
        [ProtoMember(1)]
        public UserRequestedBotViewNotification Notif { get; set; }
    }
}