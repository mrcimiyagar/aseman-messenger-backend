﻿using System.Linq;
using System.Threading.Tasks;
using HostVersion.Commands.Requests.Contact;
using HostVersion.DbContexts;
using HostVersion.Middles;
using HostVersion.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : Controller
    {   
        [Route("~/api/contact/create_contact")]
        [HttpPost]
        public async Task<ActionResult<Packet>> CreateContact([FromBody] Packet packet)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_051"};

                VersionHandler.HandleVersionsFetchings(packet);
            
                var result = await new KafkaTransport().AskPairedPeer<CreateContactRequest, CreateContactResponse>(
                    new CreateContactRequest()
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

        [Route("~/api/contact/get_contacts")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetContacts()
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_060"};
                
                var result = await new KafkaTransport().AskPairedPeer<GetContactsRequest, GetContactsResponse>(
                    new GetContactsRequest()
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