using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class Document : File
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        public Document()
        {
            this.Type = "Document";
        }
    }
}