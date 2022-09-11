using System.Collections.Generic;
using ProtoBuf;

namespace HostVersion.Middles
{
    [ProtoContract]
    public class ModuleResponse
    {
        [ProtoMember(1)]
        public Dictionary<string, string> Parameters { get; set; }
    }
}