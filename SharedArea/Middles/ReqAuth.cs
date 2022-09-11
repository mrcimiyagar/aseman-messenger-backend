using ProtoBuf;

namespace SharedArea.Middles
{
    [ProtoContract]
    public class ReqAuth
    {
        [ProtoMember(1)]
        public long SessionId { get; set; }
        [ProtoMember(2)]
        public string Token { get; set; }
    }
}