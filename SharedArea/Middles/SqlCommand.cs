using ProtoBuf;

namespace SharedArea.Middles
{
    [ProtoContract]
    public class SqlCommand
    {
        [ProtoMember(1)]
        public string SqlScript { get; set; }
        [ProtoMember(2)]
        public bool IsQuery { get; set; }
        [ProtoMember(3)]
        public bool MustReturnId { get; set; }
    }
}