using ProtoBuf;
using SharedArea.Forms;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class UploadVideoRequest : Request
    {
        [ProtoMember(1)]
        public VideoUF Form { get; set; }
    }
}