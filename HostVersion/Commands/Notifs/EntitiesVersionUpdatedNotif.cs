using System.Collections.Generic;
using HostVersion.Entities;
using ProtoBuf;

namespace HostVersion.Commands.Notifs
{
    [ProtoContract]
    public class EntitiesVersionUpdatedNotif : Notification
    {
        [ProtoMember(1)]
        public List<Version> Versions { get; set; }
    }
}