using System;
using System.Collections.Generic;
using System.IO;
using Confluent.Kafka;
using ProtoBuf;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace SharedArea.Utils
{
    public class ProtoDeserializer<T> : IDeserializer<T>
    {
        public IEnumerable<KeyValuePair<string, object>> 
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
            => config;

        public void Dispose() {}

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var stream = new MemoryStream();
            stream.Write(data);
            stream.Position = 0;
            return Serializer.Deserialize<T>(stream);
        }
    }
}