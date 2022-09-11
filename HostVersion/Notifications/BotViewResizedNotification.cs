using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace HostVersion.Notifications
{
    public class BotViewResizedNotification : Notification
    {
        [BsonElement, JsonProperty("userId")]
        public long UserId { get; set; }
        [BsonElement, JsonProperty("complexId")]
        public long ComplexId { get; set; }
        [BsonElement, JsonProperty("roomId")]
        public long RoomId { get; set; }
        [BsonElement, JsonProperty("newWidth")]
        public int NewWidth { get; set; }
        [BsonElement, JsonProperty("newHeight")]
        public int NewHeight { get; set; }
    }
}