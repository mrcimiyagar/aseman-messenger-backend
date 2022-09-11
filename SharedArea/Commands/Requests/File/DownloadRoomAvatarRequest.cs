using ProtoBuf;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class DownloadRoomAvatarRequest : Request
    {
        [ProtoMember(1)]
        public string StreamCode { get; set; }
        [ProtoMember(2)]
        public long ComplexId { get; set; }
        [ProtoMember(3)]
        public long RoomId { get; set; }
    }
}