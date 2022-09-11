
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using ApiGateway.Utils;
using SharedArea.Middles;
using Microsoft.AspNetCore.Mvc;
using SharedArea.Commands.Requests.Answers;
using SharedArea.Commands.Requests.Bot;
using SharedArea.Commands.Requests.Questions;
using SharedArea.Utils;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : Controller
    {
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