using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;

namespace SharedArea.Entities
{
    [ProtoContract]
    public class SingleRoom : BaseRoom
    {
        [ProtoMember(1)]
        [BsonElement, JsonProperty("user1Id")]
        public long? User1Id { get; set; }
        [ProtoMember(2, AsReference = true)]
        [BsonElement, JsonProperty("user1")]
        public virtual BaseUser User1 { get; set; }
        [ProtoMember(3)]
        [BsonElement, JsonProperty("user2Id")]
        public long? User2Id { get; set; }
        [ProtoMember(4, AsReference = true)]
        [BsonElement, JsonProperty("user2")]
        public virtual BaseUser User2 { get; set; }

        public SingleRoom()
        {
            this.Type = "SingleRoom";
        }
    }
}