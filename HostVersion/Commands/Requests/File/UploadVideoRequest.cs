using HostVersion.Forms;
using ProtoBuf;

namespace HostVersion.Commands.Requests.File
{
    [ProtoContract]
    public class UploadVideoRequest : Request
    {
        [ProtoMember(1)]
        public VideoUF Form { get; set; }
    }
}