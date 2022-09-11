using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HostVersion.Commands.Pushes;
using HostVersion.Commands.Requests.Answers;
using HostVersion.Commands.Requests.Bot;
using HostVersion.Commands.Requests.Questions;
using HostVersion.DbContexts;
using HostVersion.Entities;
using HostVersion.Middles;
using HostVersion.Notifications;
using HostVersion.Utils;
using HostVersion.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : Controller
    {
        [Route("~/api/robot/notify_bot_loaded")]
        [HttpPost]
        public async Task<ActionResult<Packet>> NotifyBotLoaded([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                context.Entry(session).Reference(s => s.BaseUser).Query()
                    .Include(u => ((User)u).Memberships)
                    .ThenInclude(m => m.Complex)
                    .ThenInclude(c => c.Rooms)
                    .ThenInclude(r => r.Workers)
                    .Include(u => ((User)u).Memberships)
                    .ThenInclude(m => m.Complex)
                    .ThenInclude(c => c.SingleRooms)
                    .ThenInclude(r => r.Workers)
                    .Load();
                var user = (User) session.BaseUser;

                var w = from m in user.Memberships
                    where m.ComplexId == packet.Complex.ComplexId
                    select m.Complex
                    into c
                    from r in c.Rooms.Select(r => (BaseRoom) r).Concat(c.SingleRooms.Select(r => (BaseRoom) r))
                    where r.RoomId == packet.BaseRoom.RoomId
                    select r.Workers
                    into workers
                    from worker in workers
                    where worker.BotId == packet.Bot.BaseUserId
                    select worker;

                if (!w.Any())
                    return new Packet()
                    {
                        Status = "error_1"
                    };

                var bot = (Bot) context.BaseUsers.Find(w.FirstOrDefault().BotId);

                var notif = new BotLoadedNotification()
                {
                    ComplexId = packet.Complex.ComplexId,
                    RoomId = packet.BaseRoom.RoomId,
                    UserId = user.BaseUserId,
                    BotWindowMode = packet.BotWindowMode
                };
                context.Entry(bot).Collection(b => b.Sessions).Load();
                var sessionIds = (from s in bot.Sessions select s.SessionId).ToList();
                var push = new BotLoadedPush()
                {
                    Notif = notif,
                    SessionIds = sessionIds
                };
                
                new KafkaTransport().PushNotifToApiGateway(push);

                return new Packet()
                {
                    Status = "success"
                };
            }
        }
        
        [Route("~/api/robot/get_bots")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetBots()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<GetBotsRequest, GetBotsResponse>(
                    new GetBotsRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/add_bot_to_room")]
        [HttpPost]
        public async Task<ActionResult<Packet>> AddBotToRoom([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_3"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<AddBotToRoomRequest, AddBotToRoomResponse>(
                    new AddBotToRoomRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/get_bot_store_content")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetBotStore()
        {
            var result = await new KafkaTransport().AskPairedPeer<GetBotStoreContentRequest, GetBotStoreContentResponse>(
                new GetBotStoreContentRequest()
                {
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });

            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

            return result.Packet;
        }

        [Route("~/api/robot/update_workership")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateWorkership([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_1"};
                
                VersionHandler.HandleVersionsFetchings(packet);
                
                var result = await new KafkaTransport().AskPairedPeer<UpdateWorkershipRequest, UpdateWorkershipResponse>(
                    new UpdateWorkershipRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/get_created_bots")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetCreatedBots()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<GetCreatedBotsRequest, GetCreatedBotsResponse>(
                    new GetCreatedBotsRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/get_subscribed_bots")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetSubscribedBots()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<GetSubscribedBotsRequest, GetSubscribedBotsResponse>(
                    new GetSubscribedBotsRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/subscribe_bot")]
        [HttpPost]
        public async Task<ActionResult<Packet>> SubscribeBot([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_2"};
                
                VersionHandler.HandleVersionsFetchings(packet);
                
                var result = await new KafkaTransport().AskPairedPeer<SubscribeBotRequest, SubscribeBotResponse>(
                    new SubscribeBotRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/create_bot")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateBot([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<CreateBotRequest, CreateBotResponse>(
                    new CreateBotRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/get_robot")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetRobot([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_081"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetBotRequest, GetBotResponse>(
                    new GetBotRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/update_bot_profile")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateBotProfile([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_1"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<UpdateBotProfileRequest, UpdateBotProfileResponse>(
                    new UpdateBotProfileRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/get_workerships")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetWorkerships([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetWorkershipsRequest, GetWorkershipsResponse>(
                    new GetWorkershipsRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
                
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/robot/search_bots")]
        [HttpPost]
        public async Task<ActionResult<Packet>> SearchBots([FromBody] Packet packet)
        {
            VersionHandler.HandleVersionsFetchings(packet);

            var result = await new KafkaTransport().AskPairedPeer<SearchBotsRequest, SearchBotsResponse>(
                new SearchBotsRequest()
                {
                    Packet = packet,
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });

            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

            return result.Packet;
        }

        [Route("~/api/robot/remove_bot_from_room")]
        [HttpPost]
        public async Task<ActionResult<Packet>> RemoveBotFromRoom([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_2"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<RemoveBotFromRoomRequest, RemoveBotFromRoomResponse>(
                    new RemoveBotFromRoomRequest()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }
        
        [Route("~/api/robot/add_bot_screen_shot")]
        [HttpPost]
        public async Task<ActionResult<Packet>> AddBotScreenShot([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_01"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<AskAddBotScreenShot, AnswerAddBotScreenShot>(
                    new AskAddBotScreenShot()
                    {
                        Packet = packet,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }
    }
}