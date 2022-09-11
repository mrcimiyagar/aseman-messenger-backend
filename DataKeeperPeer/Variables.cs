namespace DataKeeperPeer
{
    public class Variables
    {
        public static string ApiGatewayAddress = "localhost:9093";
        public static string ApiGatewayHttpAddress = "http://localhost:8080";
        public static string IndexerPeerAddress = "localhost:9094";
        public static string SuperPeerAddress = "localhost:9096";
        public static string SelfPeerAddress = "localhost:9095";
        public static string SelfClusterCode { get; set; } = "guilan";
        public static string SelfPeerCode { get; set; } = "rasht";
        public static string BugSnagToken { get; set; }
        public static bool HasSuperPeer { get; set; } = false;
    }
}