using ProtoBuf;
using SharedArea.Forms;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class UploadAudioRequest : Request
    {
        [ProtoMember(1)]
        public AudioUF Form { get; set; }
    }
}