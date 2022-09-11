using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedArea.Notifications;

namespace ApiGateway.DbContexts
{
    public class MongoLayer : IDisposable
    {
        private static MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<BsonDocument> _notifColl;
        private readonly IMongoCollection<Notification> _notifColl2;
        
        public static void Setup()
        {
            _client = new MongoClient("mongodb://aseman:3x2fG1b65sg4hN68sr4yj8j6k5Bstul4yi56l453tsK5346u5s4R648j@localhost:27017");
            Console.WriteLine("Connected to MongoDb");
        }

        public MongoLayer()
        {
            _db = _client.GetDatabase("ApiGatewayMongoDb");
            if (!CollectionExistsAsync("Notifications").Result)
                _db.CreateCollection("Notifications");
            _notifColl = _db.GetCollection<BsonDocument>("Notifications");
            _notifColl2 = _db.GetCollection<Notification>("Notifications");
        }
        
        public IMongoCollection<BsonDocument> GetNotifsColl()
        {
            return _notifColl;
        }

        public IMongoCollection<Notification> GetNotifsColl2()
        {
            return _notifColl2;
        }
        
        private async Task<bool> CollectionExistsAsync(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await _db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            return await collections.AnyAsync();
        }

        public void Dispose()
        {
            
        }
    }
}