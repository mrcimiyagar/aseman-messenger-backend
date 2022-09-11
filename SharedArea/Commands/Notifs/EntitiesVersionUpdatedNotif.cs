using System.Collections.Generic;
using ProtoBuf;
using SharedArea.Entities;

namespace SharedArea.Commands.Notifs
{
    [ProtoContract]
    public class EntitiesVersionUpdatedNotif : Notification
    {
        [ProtoMember(1)]
        public List<Version> Versions { get; set; }
    }
}