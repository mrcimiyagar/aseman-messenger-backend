using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HostVersion.Utils
{
    public static class DatabaseConfig
    {
        public static void ConfigDatabase(DbContext context)
        {
            context.Database.SetCommandTimeout(60000);
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void ConfigMongoNotifDb(IMongoCollection<BsonDocument> notifColl)
        {
            //notifColl.DeleteMany(notification => true);
        }
    }
}