
using ProtoBuf;

namespace SharedArea.Forms
{
    [ProtoContract]
    public class AudioUF
    {
        [ProtoMember(1)]
        public long ComplexId { get; set; }
        [ProtoMember(2)]
        public long RoomId { get; set; }
        [ProtoMember(3)]
        public string Title { get; set; }
        [ProtoMember(4)]
        public long Duration { get; set; }
        [ProtoMember(5)]
        public bool AsRawFile { get; set; }
    }
}