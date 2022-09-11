using System.Linq;
using System.Threading.Tasks;
using HostVersion.Commands.Requests.App;
using HostVersion.DbContexts;
using HostVersion.Middles;
using HostVersion.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : Controller
    {
        [Route("~/api/app/create_app")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateApp([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};
                
                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<CreateAppRequest, CreateAppResponse>(
                    new CreateAppRequest()
                    {
                        Packet = packet,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }
    }
}