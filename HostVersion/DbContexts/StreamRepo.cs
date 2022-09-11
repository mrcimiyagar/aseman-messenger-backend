using System.Collections.Generic;
using System.IO;

namespace HostVersion.DbContexts
{
    public static class StreamRepo
    {
        public static object GlobalLock = new object();
        public static Dictionary<string, Stream> FileStreams { get; set; } = new Dictionary<string, Stream>();
        public static Dictionary<string, object> FileStreamLocks { get; set; } = new Dictionary<string, object>();
        public static Dictionary<string, object> FileTransferDoneLocks { get; set; } = new Dictionary<string, object>();
    }
}