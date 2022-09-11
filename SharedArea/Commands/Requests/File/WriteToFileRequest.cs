using ProtoBuf;

namespace SharedArea.Commands.Requests.File
{
    [ProtoContract]
    public class WriteToFileRequest : Request
    {
        [ProtoMember(1)]
        public string StreamCode { get; set; }
    }
}