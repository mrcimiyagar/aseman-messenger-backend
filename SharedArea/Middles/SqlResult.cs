using ProtoBuf;

namespace SharedArea.Middles
{
    [ProtoContract]
    public class SqlResult
    {
        [ProtoMember(1)]
        public string QueryResultJson { get; set; }
        [ProtoMember(2)]
        public int? NonQueryResultNumber { get; set; }
        [ProtoMember(3)]
        public long? ScalarResultNumber { get; set; }
        [ProtoMember(4)]
        public string ErrorMessage { get; set; }
    }
}