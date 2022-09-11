
using ProtoBuf;

namespace SharedArea.Forms
{
    [ProtoContract]
    public class PhotoUF
    {
        [ProtoMember(1)]
        public long ComplexId { get; set; }
        [ProtoMember(2)]
        public long RoomId { get; set; }
        [ProtoMember(3)]
        public int Width { get; set; }
        [ProtoMember(4)]
        public int Height { get; set; }
        [ProtoMember(5)]
        public bool IsAvatar { get; set; }
        [ProtoMember(6)]
        public bool AsRawFile { get; set; }
    }
}