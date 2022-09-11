using ProtoBuf;

namespace HostVersion.Commands.Requests.Answers
{
    [ProtoContract]
    public class AnswerTest
    {
        [ProtoMember(1)]
        public string MsgText { get; set; }
    }
}