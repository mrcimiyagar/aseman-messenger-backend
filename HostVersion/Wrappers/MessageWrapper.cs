using System.ComponentModel;
using ProtoBuf;

namespace HostVersion.Wrappers
{
    [ProtoContract]
    public class MessageWrapper<T>
    {
        [ProtoMember(1)]
        public string DestClusterCode { get; set; }
        [ProtoMember(2)]
        public string DestPeerCode { get; set; }
        [ProtoMember(3)]
        public string SrcClusterCode { get; set; }
        [ProtoMember(4)]
        public string SrcPeerCode { get; set; }
        [ProtoMember(5)]
        public T Message { get; set; }
        [ProtoMember(6)]
        public string QuestionId { get; set; }
        [ProtoMember(7), DefaultValue(WrapperType.Question)]
        public WrapperType MessageType { get; set; }
    }
}