using Microsoft.EntityFrameworkCore;
using SharedArea.Entities;
using SharedArea.Utils;

namespace DataKeeperPeer
{
    public class DatabaseContext : SharedArea.DbContexts.DatabaseContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<BaseUser> BaseUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSecret> UserSecrets { get; set; }
        public DbSet<Bot> Bots { get; set; }
        public DbSet<Complex> Complexes { get; set; }
        public DbSet<ComplexSecret> ComplexSecrets { get; set; }
        public DbSet<BaseRoom> BaseRooms { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MemberAccess> MemberAccesses { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Pending> Pendings { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageSeen> MessageSeens { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileUsage> FileUsages { get; set; }
        public DbSet<Workership> Workerships { get; set; }
        public DbSet<BotSecret> BotSecrets { get; set; }
        public DbSet<BotCreation> BotCreations { get; set; }
        public DbSet<BotSubscription> BotSubscriptions { get; set; }
        public DbSet<BotStoreHeader> BotStoreHeader { get; set; }
        public DbSet<BotStoreSection> BotStoreSections { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleSecret> ModuleSecrets { get; set; }
        public DbSet<ModuleCreation> ModuleCreations { get; set; }
        public DbSet<ModulePermission> ModulePermissions { get; set; }
        public DbSet<BotScreenShot> BotScreenShots { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseNpgsql(ConnStringGenerator.GenerateDefaultConnectionString("CityServiceDb"));
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>().HasBaseType<BaseUser>();
            modelBuilder.Entity<Bot>().HasBaseType<BaseUser>();
            modelBuilder.Entity<Module>().HasBaseType<BaseUser>();

            modelBuilder.Entity<SingleRoom>().HasBaseType<BaseRoom>();
            modelBuilder.Entity<Room>().HasBaseType<BaseRoom>();
            
            modelBuilder.Entity<Photo>().HasBaseType<File>();
            modelBuilder.Entity<Audio>().HasBaseType<File>();
            modelBuilder.Entity<Video>().HasBaseType<File>();
            modelBuilder.Entity<Document>().HasBaseType<File>();
            
            modelBuilder.Entity<TextMessage>().HasBaseType<Message>();
            modelBuilder.Entity<PhotoMessage>().HasBaseType<Message>();
            modelBuilder.Entity<AudioMessage>().HasBaseType<Message>();
            modelBuilder.Entity<VideoMessage>().HasBaseType<Message>();
            modelBuilder.Entity<ServiceMessage>().HasBaseType<Message>();
            
            modelBuilder.Entity<App>()
                .Property(s => s.AppId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Audio>()
                .Property(s => s.FileId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<AudioMessage>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BaseRoom>()
                .Property(s => s.RoomId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BaseUser>()
                .Property(s => s.BaseUserId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Bot>()
                .Property(s => s.BaseUserId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotCreation>()
                .Property(s => s.BotCreationId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotSecret>()
                .Property(s => s.BotSecretId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotStoreBanner>()
                .Property(s => s.BotStoreBannerId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotStoreBot>()
                .Property(s => s.BotStoreBotId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotStoreHeader>()
                .Property(s => s.BotStoreHeaderId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotStoreSection>()
                .Property(s => s.BotStoreSectionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<BotSubscription>()
                .Property(s => s.BotSubscriptionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Complex>()
                .Property(s => s.ComplexId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<ComplexSecret>()
                .Property(s => s.ComplexSecretId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Contact>()
                .Property(s => s.ContactId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Document>()
                .Property(s => s.FileId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<File>()
                .Property(s => s.FileId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<FileTag>()
                .Property(s => s.FileTagId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<FileUsage>()
                .Property(s => s.FileUsageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Invite>()
                .Property(s => s.InviteId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<MemberAccess>()
                .Property(s => s.MemberAccessId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Membership>()
                .Property(s => s.MembershipId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Message>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<MessageSeen>()
                .Property(s => s.MessageSeenId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Module>()
                .Property(s => s.BaseUserId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<ModuleCreation>()
                .Property(s => s.ModuleCreationId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<ModulePermission>()
                .Property(s => s.ModulePermissionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<ModuleSecret>()
                .Property(s => s.ModuleSecretId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Pending>()
                .Property(s => s.PendingId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Photo>()
                .Property(s => s.FileId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<PhotoMessage>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Room>()
                .Property(s => s.RoomId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<ServiceMessage>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Session>()
                .Property(s => s.SessionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<SingleRoom>()
                .Property(s => s.RoomId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Tag>()
                .Property(s => s.TagId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<TextMessage>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<User>()
                .Property(s => s.BaseUserId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<UserSecret>()
                .Property(s => s.UserSecretId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Version>()
                .Property(s => s.VersionId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Video>()
                .Property(s => s.FileId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<VideoMessage>()
                .Property(s => s.MessageId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Workership>()
                .Property(s => s.WorkershipId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<Workership>()
                .HasIndex(m => new {m.RoomId, m.BotId})
                .IsUnique();

            modelBuilder.Entity<Membership>()
                .HasIndex(m => new {m.ComplexId, m.UserId})
                .IsUnique();
            
            modelBuilder.Entity<Contact>()
                .HasIndex(c => new {c.UserId, c.PeerId})
                .IsUnique();

            modelBuilder.Entity<Invite>()
                .HasIndex(i => new {i.ComplexId, i.UserId})
                .IsUnique();
            
            modelBuilder.Entity<Membership>()
                .HasOne(a => a.MemberAccess)
                .WithOne(a => a.Membership)
                .HasForeignKey<MemberAccess>(c => c.MembershipId);
            
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.UserId);
            
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Peer)
                .WithMany(u => u.Peereds)
                .HasForeignKey(c => c.PeerId);
        }
    }
}