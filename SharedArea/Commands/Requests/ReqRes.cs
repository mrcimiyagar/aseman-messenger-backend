using ProtoBuf;

namespace SharedArea.Commands.Requests
{
    [ProtoContract]
    [ProtoInclude(1, typeof(Request))]
    [ProtoInclude(2, typeof(Response))]
    public class ReqRes : BasePack
    {
        
    }
}