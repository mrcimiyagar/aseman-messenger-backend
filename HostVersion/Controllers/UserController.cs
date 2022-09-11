using System.Linq;
using System.Threading.Tasks;
using HostVersion.Commands.Requests.User;
using HostVersion.DbContexts;
using HostVersion.Middles;
using HostVersion.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [Route("~/api/user/update_user_profile")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateProfile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<UpdateUserProfileRequest, UpdateUserProfileResponse>(
                    new UpdateUserProfileRequest()
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

        [Route("~/api/user/get_me")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetMe()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};

                var result = await new KafkaTransport().AskPairedPeer<GetMeRequest, GetMeResponse>(
                    new GetMeRequest()
                    {
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/user/get_user_by_id")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetUserById([FromBody] Packet packet)
        {
            VersionHandler.HandleVersionsFetchings(packet);

            var result = await new KafkaTransport().AskPairedPeer<GetUserByIdRequest, GetUserByIdResponse>(
                new GetUserByIdRequest()
                {
                    Packet = packet,
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });

            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

            return result.Packet;
        }

        [Route("~/api/user/search_users")]
        [HttpPost]
        public async Task<ActionResult<Packet>> SearchUsers([FromBody] Packet packet)
        {
            VersionHandler.HandleVersionsFetchings(packet);

            var result = await new KafkaTransport().AskPairedPeer<SearchUsersRequest, SearchUsersResponse>(
                new SearchUsersRequest()
                {
                    Packet = packet,
                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                });

            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

            return result.Packet;
        }
    }
}