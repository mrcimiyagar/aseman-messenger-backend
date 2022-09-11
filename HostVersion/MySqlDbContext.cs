using Microsoft.EntityFrameworkCore;

namespace HostVersion
{
    public class MySqlDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbName = "TicketServerDb"; 
            var username = "root"; 
            var password = "3g5h165tsK65j1s564L69ka5R168kk37sut5ls3Sk2t";
            var connstring = $"Server=localhost; Database={dbName}; UID={username}; Password=\"{password}\";";
            optionsBuilder.UseMySQL(connstring);
        }
    }
}