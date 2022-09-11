using ProtoBuf;

namespace SharedArea.Commands.Requests.Answers
{
    [ProtoContract]
    public class AnswerIndexEntity : Response
    {
        [ProtoMember(1)]
        public long Index { get; set; }
    }
}