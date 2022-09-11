namespace ApiGateway
{
    public class Variables
    {
        public static string PairedPeerAddress { get; set; } = "localhost:9095";
        public static string SelfPeerAddress { get; set; } = "localhost:9093";
        public static string BugSnagToken { get; set; }
        public static string SelfClusterCode { get; set; } = "guilan";
        public static string SelfPeerCode { get; set; } = "rasht";
        
    }
}