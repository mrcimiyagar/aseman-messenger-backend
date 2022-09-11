using System.Threading.Tasks;
using HostVersion.DbContexts;
using MySql.Data.MySqlClient;

namespace HostVersion
{
    public class RawDbContext
    {
        public static RawDbContext Instance { get; set; } = new RawDbContext();

        private readonly MySqlConnection _connection;

        public RawDbContext()
        { 
            using (var dbContext = new MySqlDbContext())
            {
                dbContext.Database.EnsureCreated();
            }

            Task.Delay(1);
            
            var dbName = "TicketServerDb"; 
            var username = "root"; 
            var password = "3g5h165tsK65j1s564L69ka5R168kk37sut5ls3Sk2t";
            var connstring = $"Server=localhost; database={dbName}; UID={username}; Password=\"{password}\";";
            _connection = new MySqlConnection(connstring);
            _connection.Open();

            var query = @"CREATE TABLE IF NOT EXISTS Tickets (
              id bigint(20) NOT NULL auto_increment,
              stub char(50) NOT NULL default '',
              PRIMARY KEY  (id),
              UNIQUE KEY stub (stub)) ENGINE=MyISAM";
            var cmd = new MySqlCommand(query, _connection);
            cmd.ExecuteNonQuery();
        }

        public MySqlConnection Connection()
        {
            return _connection;
        }
    }
}