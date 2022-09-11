using ProtoBuf;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class DownloadUserAvatarRequest : Request
    {
        [ProtoMember(1)]
        public string StreamCode { get; set; }
        [ProtoMember(2)]
        public long UserId { get; set; }
    }
}