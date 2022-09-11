using MySql.Data.MySqlClient;

namespace DataIndexerPeer
{
    public class RawDbContext
    {
        public static RawDbContext Instance { get; set; } = new RawDbContext();

        private readonly MySqlConnection _connection;

        public RawDbContext()
        { 
            var dbName = "TicketServerDb"; 
            var username = "root"; 
            var password = "3g5h165tsK65j1s564L69ka5R168kk37sut5ls3Sk2t";
            var connstring = $"Server=localhost; database={dbName}; UID={username}; Password=\"{password}\";";
            _connection = new MySqlConnection(connstring);
            _connection.Open();
        }

        public MySqlConnection Connection()
        {
            return _connection;
        }
    }
}