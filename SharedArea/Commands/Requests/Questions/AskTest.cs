using ProtoBuf;

namespace SharedArea.Commands.Requests.Questions
{
    [ProtoContract]
    public class AskTest
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}