using System;
using System.Collections.Generic;
using ProtoBuf;

namespace SharedArea.Middles
{
    [ProtoContract]
    public class ModuleResponse
    {
        [ProtoMember(1)]
        public Dictionary<string, string> Parameters { get; set; }
    }
}