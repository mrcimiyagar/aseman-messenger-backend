using System.Collections.Generic;
using System.IO;
using Confluent.Kafka;
using ProtoBuf;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace SharedArea.Utils
{
    public class ProtoSerializer<T> : ISerializer<T>
    {
        
        public IEnumerable<KeyValuePair<string, object>> 
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
            => config;

        public void Dispose() {}

        public byte[] Serialize(T data, SerializationContext context)
        {
            var stream = new MemoryStream();
            Serializer.Serialize<T>(stream, data);
            return stream.ToArray();
        }
    }
}