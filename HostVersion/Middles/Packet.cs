using System.Collections.Generic;
using HostVersion.Entities;
using HostVersion.Notifications;
using ProtoBuf;

namespace HostVersion.Middles
{
    [ProtoContract]
    public class Packet
    {
        [ProtoMember(1)]
        public string Status { get; set; }
        [ProtoMember(2)]
        public string Email { get; set; }
        [ProtoMember(3)]
        public string VerifyCode { get; set; }
        [ProtoMember(4)]
        public Session Session { get; set; }
        [ProtoMember(5)]
        public User User { get; set; }
        [ProtoMember(6)]
        public UserSecret UserSecret { get; set; }
        [ProtoMember(7)]
        public Contact Contact { get; set; }
        [ProtoMember(8)]
        public ServiceMessage ServiceMessage { get; set; }
        [ProtoMember(9)]
        public List<Contact> Contacts { get; set; }
        [ProtoMember(10)]
        public BaseRoom BaseRoom { get; set; }
        [ProtoMember(11)]
        public Complex Complex { get; set; }
        [ProtoMember(12)]
        public ComplexSecret ComplexSecret { get; set; }
        [ProtoMember(13)]
        public List<ComplexSecret> ComplexSecrets { get; set; }
        [ProtoMember(14)]
        public List<Workership> Workerships { get; set; }
        [ProtoMember(15)]
        public Workership Workership { get; set; }
        [ProtoMember(16)]
        public Bot Bot { get; set; }
        [ProtoMember(17)]
        public List<Bot> Bots { get; set; }
        [ProtoMember(18)]
        public List<Complex> Complexes { get; set; }
        [ProtoMember(19)]
        public List<BaseRoom> BaseRooms { get; set; }
        [ProtoMember(20)]
        public string SearchQuery { get; set; }
        [ProtoMember(21)]
        public List<User> Users { get; set; }
        [ProtoMember(22)]
        public List<Session> Sessions { get; set; }
        [ProtoMember(23)]
        public List<Membership> Memberships { get; set; }
        [ProtoMember(24)]
        public Membership Membership { get; set; }
        [ProtoMember(25)]
        public MemberAccess MemberAccess { get; set; }
        [ProtoMember(26)]
        public List<MemberAccess> MemberAccesses { get; set; }
        [ProtoMember(27)]
        public List<File> Files { get; set; }
        [ProtoMember(28)]
        public List<Message> Messages { get; set; }
        [ProtoMember(29)]
        public File File { get; set; }
        [ProtoMember(30)]
        public Message Message { get; set; }
        [ProtoMember(31)]
        public TextMessage TextMessage { get; set; }
        [ProtoMember(32)]
        public PhotoMessage PhotoMessage { get; set; }
        [ProtoMember(33)]
        public AudioMessage AudioMessage { get; set; }
        [ProtoMember(34)]
        public VideoMessage VideoMessage { get; set; }
        [ProtoMember(35)]
        public BotStoreHeader BotStoreHeader { get; set; }
        [ProtoMember(36)]
        public List<BotStoreSection> BotStoreSections { get; set; }
        [ProtoMember(37)]
        public BotSubscription BotSubscription { get; set; }
        [ProtoMember(38)]
        public List<BotSubscription> BotSubscriptions { get; set; }
        [ProtoMember(39)]
        public BotCreation BotCreation { get; set; }
        [ProtoMember(40)]
        public List<BotCreation> BotCreations { get; set; }
        [ProtoMember(41)]
        public Invite Invite { get; set; }
        [ProtoMember(42)]
        public List<Invite> Invites { get; set; }
        [ProtoMember(43)]
        public FileUsage FileUsage { get; set; }
        [ProtoMember(44)]
        public BaseUser BaseUser { get; set; }
        [ProtoMember(45)]
        public Photo Photo { get; set; }
        [ProtoMember(46)]
        public Audio Audio { get; set; }
        [ProtoMember(47)]
        public Video Video { get; set; }
        [ProtoMember(48)]
        public Document Document { get; set; }
        [ProtoMember(49)]
        public Notification Notif { get; set; }
        [ProtoMember(50)]
        public List<Version> Versions { get; set; }
        [ProtoMember(51)]
        public string Username { get; set; }
        [ProtoMember(52)]
        public string Password { get; set; }
        [ProtoMember(53)]
        public string StreamCode { get; set; }
        [ProtoMember(54)]
        public long? Offset { get; set; }
        [ProtoMember(55)]
        public string RawJson { get; set; }
        [ProtoMember(56)]
        public long? MessageSeenCount { get; set; }
        [ProtoMember(57)]
        public bool? BatchData { get; set; }
        [ProtoMember(58)]
        public bool? FetchNext { get; set; }
        [ProtoMember(59)]
        public string ControlId { get; set; }
        [ProtoMember(60)]
        public Module Module { get; set; }
        [ProtoMember(61)]
        public List<Module> Modules { get; set; }
        [ProtoMember(62)]
        public ModuleSecret ModuleSecret { get; set; }
        [ProtoMember(63)]
        public ModulePermission ModulePermission { get; set; }
        [ProtoMember(64)]
        public ModuleCreation ModuleCreation { get; set; }
        [ProtoMember(65)]
        public ModuleRequest ModuleRequest { get; set; }
        [ProtoMember(66)]
        public ModuleResponse ModuleResponse { get; set; }
        [ProtoMember(67)]
        public string Text { get; set; }
        [ProtoMember(68)]
        public string ErrorMessage { get; set; }
        [ProtoMember(69)]
        public SqlCommand SqlCommand { get; set; }
        [ProtoMember(70)]
        public SqlResult SqlResult { get; set; }
        [ProtoMember(71)]
        public App App { get; set; }
        [ProtoMember(72)]
        public bool? SingleRoomMode { get; set; }
        [ProtoMember(73)]
        public bool? FinishFileTransfer { get; set; }
        [ProtoMember(74)]
        public Pending Pending { get; set; }
        [ProtoMember(75)]
        public MessageSeen MessageSeen { get; set; }
        [ProtoMember(76)]
        public BotScreenShot BotScreenShot { get; set; }
        [ProtoMember(77)]
        public bool PreviewBotView { get; set; }
        [ProtoMember(78)]
        public bool BotWindowMode { get; set; }
    }
}