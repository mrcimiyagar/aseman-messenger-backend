using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace HostVersion.Entities
{
    [ProtoContract]
    public class Workership
    {
        [ProtoMember(1)]
        [Key]
        [BsonElement, JsonProperty("workershipId")]
        public long WorkershipId { get; set; }
        [ProtoMember(2)]
        [BsonElement, JsonProperty("botId")]
        public long BotId { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("roomId")]
        public long? RoomId { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("room")]
        public virtual BaseRoom Room { get; set; }
        [ProtoMember(5)]
        [BsonElement, JsonProperty("posX")]
        public int PosX { get; set; }
        [ProtoMember(6)]
        [BsonElement, JsonProperty("posY")]
        public int PosY { get; set; }
        [ProtoMember(7)]
        [BsonElement, JsonProperty("width")]
        public int Width { get; set; }
        [ProtoMember(8)]
        [BsonElement, JsonProperty("height")]
        public int Height { get; set; }
        [ProtoMember(9)]
        [BsonElement, JsonProperty("angle")]
        public int Angle { get; set; }
    }
}