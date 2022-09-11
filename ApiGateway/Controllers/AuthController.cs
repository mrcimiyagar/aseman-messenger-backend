using System.Linq;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using ApiGateway.Utils;
using Bugsnag.Payload;
using SharedArea.Middles;
using Microsoft.AspNetCore.Mvc;
using SharedArea.Commands.Requests.Auth;
using SharedArea.Utils;
using Session = SharedArea.Entities.Session;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [Route("~/api/auth/register")]
        [HttpPost]
        public async Task<ActionResult<Packet>> Register([FromBody] Packet packet)
        {
            VersionHandler.HandleVersionsFetchings(packet);

            var result = await new KafkaTransport().AskPairedPeer<RegisterRequest, RegisterResponse>(
                new RegisterRequest()
                {
                    Packet = packet,
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });
            
            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);
            
            return result.Packet;
        }

        [Route("~/api/auth/verify")]
        [HttpPost]
        public async Task<ActionResult<Packet>> Verify([FromBody] Packet packet)
        {
            VersionHandler.HandleVersionsFetchings(packet);
            
            var result = await new KafkaTransport().AskPairedPeer<VerifyRequest, VerifyResponse>(
                new VerifyRequest()
                {
                    Packet = packet,
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });

            using (var dbContext = new DatabaseContext())
            {
                var session = result.Packet.Session;
                var clonedSess = new Session
                {
                    SessionId = session.SessionId,
                    Online = session.Online,
                    Token = session.Token,
                    Version = session.Version,
                    ConnectionId = session.ConnectionId,
                    BaseUser = null,
                    BaseUserId = null
                };
                dbContext.Sessions.Add(clonedSess);
                dbContext.SaveChanges();
            }
            
            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);
            
            return result.Packet;
        }

        [Route("~/api/auth/logout")]
        [HttpPost]
        public async Task<ActionResult<Packet>> Logout()
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_040"};
                
                var result = await new KafkaTransport().AskPairedPeer<LogoutRequest, LogoutResponse>(
                    new LogoutRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });
            
                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);
                
                return result.Packet;
            }
        }

        [Route("~/api/auth/delete_account")]
        [HttpPost]
        public async Task<ActionResult<Packet>> DeleteAccount()
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                var result = await new KafkaTransport().AskPairedPeer<DeleteAccountRequest, DeleteAccountResponse>(
                    new DeleteAccountRequest()
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