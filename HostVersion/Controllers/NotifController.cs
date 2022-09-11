using System.Linq;
using HostVersion.DbContexts;
using HostVersion.Middles;
using HostVersion.Utils;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotifController : Controller
    {
        [Route("~/api/notif/notify_notif_received")]
        [HttpPost]
        public ActionResult<Packet> NotifyNotifReceived([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                using (var mongo = new MongoLayer())
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(packet.Notif.NotificationId));
                    
                    var notif = mongo.GetNotifsColl().Find(filter).FirstOrDefault();
                    if (notif == null) return new Packet() {Status = "error_1"};
                    if (notif["SessionId"] == session.SessionId)
                    {
                        var n = mongo.GetNotifsColl().FindOneAndDelete(filter).FirstOrDefault();
                        if (n != null)
                        {
                            Startup.Pusher.NotifyNotificationReceived(session.SessionId);
                            Startup.Pusher.NextPush(session.SessionId);
                        }
                    }
                }
            }

            return new Packet() {Status = "success"};
        }
        
        [Route("~/api/notif/notify_bot_notif_received")]
        [HttpPost]
        public ActionResult<Packet> NotifyBotNotifReceived([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBot(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                using (var mongo = new MongoLayer())
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(packet.Notif.NotificationId));
                    
                    var notif = mongo.GetNotifsColl().Find(filter).FirstOrDefault();
                    if (notif == null) return new Packet() {Status = "error_1"};
                    if (notif["SessionId"] == session.SessionId)
                        mongo.GetNotifsColl().DeleteOne(filter);
                }
                
                Startup.Pusher.NotifyNotificationReceived(session.SessionId);
                
                Startup.Pusher.NextPush(session.SessionId);
            }

            return new Packet() {Status = "success"};
        }
    }
}