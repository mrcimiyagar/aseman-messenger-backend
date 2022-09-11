using ProtoBuf;

namespace HostVersion.Commands.Requests.File
{
    [ProtoContract] 
    public class DownloadFileRequest : Request
    {
        [ProtoMember(1)]
        public long FileId { get; set; }
        [ProtoMember(2)]
        public long Offset { get; set; }
        [ProtoMember(3)]
        public string StreamCode { get; set; }
    }
}