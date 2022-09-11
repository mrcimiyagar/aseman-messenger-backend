using System.Linq;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using ApiGateway.Utils;
using SharedArea.Middles;
using Microsoft.AspNetCore.Mvc;
using SharedArea.Commands.Requests.Complex;
using SharedArea.Utils;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplexController : Controller
    {
        [Route("~/api/complex/update_complex_profile")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateComplexProfile([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_1" };

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<UpdateComplexProfileRequest, UpdateComplexProfileResponse>(
                    new UpdateComplexProfileRequest()
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

        [Route("~/api/complex/create_complex")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateComplex([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_1" };

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<CreateComplexRequest, CreateComplexResponse>(
                    new CreateComplexRequest()
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

        [Route("~/api/complex/delete_complex")]
        [HttpPost]
        public async Task<ActionResult<Packet>> DeleteComplex([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_2" };
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<DeleteComplexRequest, DeleteComplexResponse>(
                    new DeleteComplexRequest()
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

        [Route("~/api/complex/get_complexes")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetComplexes()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<GetComplexesRequest, GetComplexesResponse>(
                    new GetComplexesRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/complex/get_complex_by_id")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetComplexById([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetComplexByIdRequest, GetComplexByIdResponse>(
                    new GetComplexByIdRequest()
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

        [Route("~/api/complex/search_complexes")]
        [HttpPost]
        public async Task<ActionResult<Packet>> SearchComplexes([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<SearchComplexesRequest, SearchComplexesResponse>(
                    new SearchComplexesRequest()
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

        [Route("~/api/complex/update_member_access")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateMemberAccess([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<UpdateMemberAccessRequest, UpdateMemberAccessResponse>(
                    new UpdateMemberAccessRequest()
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

        [Route("~/api/complex/get_complex_accesses")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetComplexAccesses([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetComplexAccessesRequest, GetComplexAccessesResponse>(
                    new GetComplexAccessesRequest()
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

        [Route("~/api/complex/bot_get_workerships")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotGetWorkerships()
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBot(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<BotGetWorkershipsRequest, BotGetWorkershipsResponse>(
                    new BotGetWorkershipsRequest()
                    {
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