using System;
using HostVersion.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HostVersion.Utils
{
    public class FileConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(File));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            if (jo["type"]?.Value<string>() == null)
            {
                return jo.ToObject<File>(Newtonsoft.Json.JsonSerializer.CreateDefault());
            }

            switch (jo["type"].Value<string>())
            {
                case "Photo":
                    return jo.ToObject<Photo>(serializer);
                case "Audio":
                    return jo.ToObject<Audio>(serializer);
                case "Video":
                    return jo.ToObject<Video>(serializer);
                case "Document":
                    return jo.ToObject<Document>(serializer);
                default:
                {
                    return jo.ToObject<File>(Newtonsoft.Json.JsonSerializer.CreateDefault());
                }
            }
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}