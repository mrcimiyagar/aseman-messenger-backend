using HostVersion.Forms;
using ProtoBuf;

namespace HostVersion.Commands.Requests.File
{
    [ProtoContract]
    public class UploadPhotoRequest : Request
    {
        [ProtoMember(1)]
        public PhotoUF Form { get; set; }
    }
}