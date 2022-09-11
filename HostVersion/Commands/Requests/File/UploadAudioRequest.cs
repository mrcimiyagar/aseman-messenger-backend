using HostVersion.Forms;
using ProtoBuf;

namespace HostVersion.Commands.Requests.File
{
    [ProtoContract]
    public class UploadAudioRequest : Request
    {
        [ProtoMember(1)]
        public AudioUF Form { get; set; }
    }
}