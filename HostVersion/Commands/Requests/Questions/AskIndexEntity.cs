using ProtoBuf;

namespace HostVersion.Commands.Requests.Questions
{
    [ProtoContract]
    public class AskIndexEntity : Request
    {
        [ProtoMember(1)]
        public string EntityType { get; set; }
    }
}