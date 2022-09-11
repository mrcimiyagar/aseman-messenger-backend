using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HostVersion.Commands.Requests.File;
using HostVersion.DbContexts;
using HostVersion.Forms;
using HostVersion.Middles;
using HostVersion.Models.Forms;
using HostVersion.Utils;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using File = HostVersion.Entities.File;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        [Route("~/api/file/write_to_file")]
        [RequestSizeLimit(bytes: 4294967296)]
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<ActionResult<Packet>> WriteToFile()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return new Packet {Status = "error_0"};
            }

            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_1"};

                var formParts = new Dictionary<string, string>();

                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(Request.ContentType),
                    DefaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    var hasContentDispositionHeader =
                        ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                            out var contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var guid = Guid.NewGuid().ToString();

                            lock (StreamRepo.GlobalLock)
                            {
                                StreamRepo.FileStreams.Add(guid, section.Body);
                            }
                            
                            var kt = new KafkaTransport();
                            var result = await kt.AskPairedPeer<WriteToFileRequest, WriteToFileResponse>(
                                new WriteToFileRequest()
                                {
                                    SessionId = session.SessionId,
                                    SessionVersion = session.Version,
                                    Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString()),
                                    Packet = new Packet()
                                    {
                                        File = new File() {FileId = Convert.ToInt64(formParts["FileId"])},
                                        StreamCode = guid
                                    }
                                });

                            lock (StreamRepo.GlobalLock)
                            {
                                StreamRepo.FileStreams.Remove(guid);
                            }

                            VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                            return result.Packet;
                        }

                        if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                        {
                            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                            var encoding = GetEncoding(section);
                            using (var streamReader = new StreamReader(
                                section.Body,
                                encoding,
                                true,
                                1024,
                                true))
                            {
                                var value = await streamReader.ReadToEndAsync();

                                formParts[key.ToString()] = value;

                                if (formParts.Count > DefaultFormOptions.ValueCountLimit)
                                {
                                    throw new InvalidDataException(
                                        $"Form key count limit {DefaultFormOptions.ValueCountLimit} exceeded.");
                                }
                            }
                        }
                    }

                    section = await reader.ReadNextSectionAsync();
                }
            }

            return new Packet() {Status = "error_2"};
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }

        [Route("~/api/file/get_file_size")]
        [HttpPost]
        public async Task<ActionResult<Packet>> GetFileSize([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.Authenticate(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<GetFileSizeRequest, GetFileSizeResponse>(
                    new GetFileSizeRequest()
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

        [Route("~/api/file/get_file_upload_stream")]
        [HttpPost]
        public ActionResult GetFileUploadStream([FromBody] Packet packet)
        {
            if (packet.Username == SharedArea.GlobalVariables.FileTransferUsername
                && packet.Password == SharedArea.GlobalVariables.FileTransferPassword)
            {
                VersionHandler.HandleVersionsFetchings(packet);

                return File(StreamRepo.FileStreams[packet.StreamCode], "application/octet-stream");
            }
            else
            {
                return NotFound();
            }
        }

        [Route("~/api/file/upload_photo")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UploadPhoto([FromForm] PhotoUploadForm form)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var puf = new PhotoUF()
                {
                    ComplexId = form.ComplexId,
                    RoomId = form.RoomId,
                    Width = form.Width,
                    Height = form.Height,
                    IsAvatar = form.IsAvatar
                };

                var result = await new KafkaTransport().AskPairedPeer<UploadPhotoRequest, UploadPhotoResponse>(
                    new UploadPhotoRequest()
                    {
                        Form = puf,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/file/upload_audio")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UploadAudio([FromForm] AudioUploadForm form)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var auf = new AudioUF()
                {
                    ComplexId = form.ComplexId,
                    RoomId = form.RoomId,
                    Title = form.Title,
                    Duration = form.Duration
                };
                
                var result = await new KafkaTransport().AskPairedPeer<UploadAudioRequest, UploadAudioResponse>(
                    new UploadAudioRequest()
                    {
                        Form = auf,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/file/upload_video")]
        [HttpPost]
        public async Task<ActionResult<Packet>> UploadVideo([FromForm] VideoUploadForm form)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet {Status = "error_0"};
                
                var vuf = new VideoUF()
                {
                    ComplexId = form.ComplexId,
                    RoomId = form.RoomId,
                    Title = form.Title,
                    Duration = form.Duration
                };
                
                var result = await new KafkaTransport().AskPairedPeer<UploadVideoRequest, UploadVideoResponse>(
                    new UploadVideoRequest()
                    {
                        Form = vuf,
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                VersionHandler.HandleVersionsUpdates(result.Packet.Versions);

                return result.Packet;
            }
        }

        [Route("~/api/file/take_file_download_stream")]
        [RequestSizeLimit(bytes: 4294967296)]
        [HttpPost]
        public ActionResult TakeFileDownloadStream([FromForm] TakeFileDSF form)
        {
            if (form.Username != SharedArea.GlobalVariables.FileTransferUsername ||
                form.Password != SharedArea.GlobalVariables.FileTransferPassword) return Forbid();
            
            var file = form.File;
            var streamCode = form.StreamCode;

            lock (StreamRepo.GlobalLock)
            {
                StreamRepo.FileStreams.Add(streamCode, file.OpenReadStream());
            }

            var lockObj = StreamRepo.FileStreamLocks[streamCode];

            lock (lockObj)
            {
                Monitor.Pulse(lockObj);
            }

            lock (lockObj)
            {
                Monitor.Wait(lockObj);
            }

            return Ok();
        }

        [Route("~/api/file/download_file")]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(long fileId, long offset)
        {
            using (var context = new DatabaseContext())
            {
                var session = Security.Authenticate(context, Request.Headers[AuthExtracter.AK]);
                if (session == null) return NotFound();

                var guid = Guid.NewGuid().ToString();

                var result = await new KafkaTransport().AskPairedPeer<DownloadFileRequest, DownloadFileResponse>(
                    new DownloadFileRequest()
                    {
                        Packet = new Packet()
                        {
                            StreamCode = guid,
                            Offset = offset,
                            File = new File() {FileId = fileId}
                        },
                        SessionId = session.SessionId,
                        SessionVersion = session.Version,
                        Headers = Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString())
                    });

                if (result.Packet.Status == "success")
                {
                    var s = StreamRepo.FileStreams[guid];
                    StreamRepo.FileStreams.Remove(guid);
                    return File(s, "application/octet-stream");
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [Route("~/api/file/bot_create_document_file")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotCreateDocumentFile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBotOrModule(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);
                
                var result = await new KafkaTransport().AskPairedPeer<BotCreateDocumentFileRequest, BotCreateDocumentFileResponse>(
                    new BotCreateDocumentFileRequest()
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
        
        [Route("~/api/file/bot_append_text_to_txt_file")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotAppendTextToTxtFile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBotOrModule(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);
                
                var result = await new KafkaTransport().AskPairedPeer<BotAppendTextToTxtFileRequest, BotAppendTextToTxtFileResponse>(
                    new BotAppendTextToTxtFileRequest()
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
        
        [Route("~/api/file/bot_execute_sql_on_sql_file")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotExecuteSqlOnSqlFile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBotOrModule(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);
                
                var result = await new KafkaTransport().AskPairedPeer<BotExecuteSqlOnSqlFileRequest, BotExecuteSqlOnSqlFileResponse>(
                    new BotExecuteSqlOnSqlFileRequest()
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
        
        [Route("~/api/file/bot_execute_mongo_com_on_mongo_file")]
        [HttpPost]
        public async Task<ActionResult<Packet>> BotExecuteMongoComOnMongoFile([FromBody] Packet packet)
        {
            using (var dbContext = new DatabaseContext())
            {
                var session = Security.AuthenticateBotOrModule(dbContext, Request.Headers[AuthExtracter.AK]);
                if (session == null) return new Packet() {Status = "error_0"};

                VersionHandler.HandleVersionsFetchings(packet);

                var result = await new KafkaTransport().AskPairedPeer<BotExecuteMongoComOnMongoFileRequest
                    , BotExecuteMongoComOnMongoFileResponse>(
                    new BotExecuteMongoComOnMongoFileRequest()
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