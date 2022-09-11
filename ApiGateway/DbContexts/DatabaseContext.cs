using Microsoft.EntityFrameworkCore;
using SharedArea.Entities;
using SharedArea.Notifications;
using SharedArea.Utils;

namespace ApiGateway.DbContexts
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Version> Versions { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql(ConnStringGenerator.GenerateDefaultConnectionString("ApiGatewayDb"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Session>()
                .Property(s => s.SessionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<App>()
                .Property(a => a.AppId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Version>()
                .Property(s => s.VersionId)
                .ValueGeneratedNever();

            modelBuilder.Entity<BaseUser>()
                .Property(bu => bu.BaseUserId)
                .ValueGeneratedNever();
        }
    }
}