using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedArea.Entities;

namespace SharedArea.Utils
{
    public class RoomConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BaseRoom));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            if (jo["type"]?.Value<string>() == null)
            {
                return jo.ToObject<BaseRoom>(Newtonsoft.Json.JsonSerializer.CreateDefault());
            }

            switch (jo["type"].Value<string>())
            {
                case "Room":
                    return jo.ToObject<Room>(serializer);
                case "SingleRoom":
                    return jo.ToObject<SingleRoom>(serializer);
                default:
                {
                    return jo.ToObject<BaseRoom>(Newtonsoft.Json.JsonSerializer.CreateDefault());
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