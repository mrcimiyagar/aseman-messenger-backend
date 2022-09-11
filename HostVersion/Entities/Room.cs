using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Room : BaseRoom
    {
        public Room()
        {
            this.Type = "Room";
        }
    }
}