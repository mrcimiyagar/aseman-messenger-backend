using System.Linq;
using System.Threading.Tasks;
using ApiGateway.DbContexts;
using ApiGateway.Utils;
using SharedArea.Middles;
using Microsoft.AspNetCore.Mvc;
using SharedArea.Commands.Requests.Room;
using SharedArea.Utils;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        [Route("~/api/room/create_room")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateRoom([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_2" };
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<CreateRoomRequest, CreateRoomResponse>(
                    new CreateRoomRequest()
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
                
        [Route("~/api/room/update_room_profile")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UpdateRoomProfile([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_2" };

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<UpdateRoomProfileRequest, UpdateRoomProfileResponse>(
                    new UpdateRoomProfileRequest()
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

        [Route("~/api/room/delete_room")]
        [HttpPost]
        public async Task<ActionResult<Packet>> DeleteRoom([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet { Status = "error_2" };

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<DeleteRoomRequest, DeleteRoomResponse>(
                    new DeleteRoomRequest()
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

        [Route("~/api/room/get_rooms")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetRooms([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0B1"};
                
                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetRoomsRequest, GetRoomsResponse>(
                    new GetRoomsRequest()
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

        [Route("~/api/room/get_room_by_id")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetRoomById([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<GetRoomByIdRequest, GetRoomByIdResponse>(
                    new GetRoomByIdRequest()
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