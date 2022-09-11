using System.Collections.Generic;
using ProtoBuf;

namespace HostVersion.Middles
{
    [ProtoContract]
    public class ModuleRequest
    {
        [ProtoMember(1)]
        public string ActionName { get; set; }
        [ProtoMember(2)]
        public Dictionary<string, string> Parameters { get; set; }
    }
}