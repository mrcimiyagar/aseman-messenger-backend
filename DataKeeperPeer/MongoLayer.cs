using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedArea.Notifications;

namespace DataKeeperPeer
{
    public class MongoLayer : IDisposable
    {
        private static IMongoDatabase _db;
        private static IMongoCollection<BsonDocument> _notifColl;
        private static IMongoCollection<Notification> _notifColl2;
        
        public static void Setup()
        {
            var client = new MongoClient("mongodb://aseman:3x2fG1b65sg4hN68sr4yj8j6k5Bstul4yi56l453tsK5346u5s4R648j@localhost:27017");
            _db = client.GetDatabase("DataKeeperVersionsDb1");
            if (!CollectionExistsAsync("Versions").Result)
                _db.CreateCollection("Versions");
            _notifColl = _db.GetCollection<BsonDocument>("Versions");
            _notifColl2 = _db.GetCollection<Notification>("Versions");
            Console.WriteLine("Connected to MongoDb");
        }
        
        public IMongoCollection<BsonDocument> GetNotifsColl()
        {
            return _notifColl;
        }

        public IMongoCollection<Notification> GetNotifsColl2()
        {
            return _notifColl2;
        }
        
        private static async Task<bool> CollectionExistsAsync(string collectionName)
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