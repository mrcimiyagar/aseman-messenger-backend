using HostVersion.Commands.Notifs;
using HostVersion.Commands.Pushes;
using HostVersion.Commands.Requests;
using ProtoBuf;

namespace HostVersion.Commands
{
    [ProtoContract]
    [ProtoInclude(1, typeof(ReqRes))]
    [ProtoInclude(2, typeof(Push))]
    [ProtoInclude(3, typeof(Notification))]
    public class BasePack
    {
        
    }
}