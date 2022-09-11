using System.Collections.Generic;

namespace SuperPeer
{
    public class Variables
    {
        public static string SelfPeerAddress = "localhost:9096";
        public static string SelfClusterCode { get; set; } = "guilan";
        public static string SelfPeerCode { get; set; } = "rasht";
        public static string BugSnagToken { get; set; }
        
        public static readonly Dictionary<string, string> PeerAddresses = new Dictionary<string, string>()
        {
            { "rasht", "localhost:9095" }
        };
    }
}