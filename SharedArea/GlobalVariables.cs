using System.Collections.Generic;

namespace SharedArea
{
    public class GlobalVariables
    {
        public static readonly Dictionary<string, string> SuperPeerAddresses = new Dictionary<string, string>()
        {
            { "guilan", "localhost:9096" }
        };
        public static string KafkaUsername = "admin", KafkaPassword = "admin-secret";
        public static string FileTransferUsername = "admin", FileTransferPassword = "admin-secret";
        public static string FileTransferUploadStreamAction = "/api/file/get_file_upload_stream";
        public static string FileTransferDownloadStreamAction = "/api/file/take_file_download_stream";
    }
}