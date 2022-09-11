using ProtoBuf;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Pushes;
using SharedArea.Commands.Requests;

namespace SharedArea.Commands
{
    [ProtoContract]
    [ProtoInclude(1, typeof(ReqRes))]
    [ProtoInclude(2, typeof(Push))]
    [ProtoInclude(3, typeof(Notification))]
    public class BasePack
    {
        
    }
}