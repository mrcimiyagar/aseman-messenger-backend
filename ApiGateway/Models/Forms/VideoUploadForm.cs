using Microsoft.AspNetCore.Http;

namespace ApiGateway.Models.Forms
{
    public class VideoUploadForm
    {
        public long ComplexId { get; set; }
        public long RoomId { get; set; }
        public string Title { get; set; }
        public long Duration { get; set; }
    }
}