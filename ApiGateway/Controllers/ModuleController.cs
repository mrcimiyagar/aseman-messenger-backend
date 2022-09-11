
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using ApiGateway.Utils;
using Microsoft.AspNetCore.Mvc;
using SharedArea.Commands.Requests.Internal;
using SharedArea.Commands.Requests.Module;
using SharedArea.Entities;
using SharedArea.Middles;
using SharedArea.Utils;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : Controller
    {
        [Route("~/api/module/request_module")]
        [HttpPost]
        public async Task<ActionResult> RequestModule([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.AuthenticateBot(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return Forbid();
                
                var result = await new KafkaTransport().AskPairedPeer<GetModuleServerAddressRequest, GetModuleServerAddressResponse>(
                    new GetModuleServerAddressRequest()
                    {
                        Packet = new Packet()
                        {
                            Module = new Module()
                            {
                                BaseUserId = packet.Module.BaseUserId
                            }
                        },
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                var client = new HttpClient();
                var content = new FormUrlEncodedContent(packet.ModuleRequest.Parameters);
                var serverPath = result.Packet.ModuleSecret.ServerAddress;
                if (serverPath.EndsWith("/")) serverPath = serverPath.Substring(0, serverPath.Length - 1);
                var response = await client.PostAsync(serverPath + "/" + packet.ModuleRequest.ActionName, content);
                var responseString = await response.Content.ReadAsStringAsync();
                
                return Ok(responseString);
            }
        }

        [Route("~/api/module/create_module")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateModule([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<CreateModuleRequest, CreateModuleResponse>(
                    new CreateModuleRequest()
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
        
        [Route("~/api/module/update_module_profile")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateModuleProfile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<UpdateModuleProfileRequest, UpdateModuleProfileResponse>(
                    new UpdateModuleProfileRequest()
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
        
        [Route("~/api/module/search_modules")]
        [HttpPost]
        public async Task<ActionResult<Packet>> SearchModules([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<SearchModulesRequest, SearchModulesResponse>(
                    new SearchModulesRequest()
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
        
        [Route("~/api/module/bot_permit_module")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotPermitModule([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBot(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<BotPermitModuleRequest, BotPermitModuleResponse>(
                    new BotPermitModuleRequest()
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