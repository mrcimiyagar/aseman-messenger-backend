using ProtoBuf;
using SharedArea.Forms;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class UploadPhotoRequest : Request
    {
        [ProtoMember(1)]
        public PhotoUF Form { get; set; }
    }
}