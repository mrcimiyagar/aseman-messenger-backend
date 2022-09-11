using ProtoBuf;
using SharedArea.Middles;

namespace SharedArea.Commands.Notifs
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AccountCreatedNotif))]
    [ProtoInclude(2, typeof(AccountDeletedNotif))]
    [ProtoInclude(3, typeof(AppCreatedNotif))]
    [ProtoInclude(4, typeof(AudioCreatedNotif))]
    [ProtoInclude(5, typeof(BotCreatedNotif))]
    [ProtoInclude(6, typeof(BotProfileUpdatedNotif))]
    [ProtoInclude(7, typeof(BotSubscribedNotif))]
    [ProtoInclude(8, typeof(ComplexCreatedNotif))]
    [ProtoInclude(9, typeof(ComplexDeletionNotif))]
    [ProtoInclude(10, typeof(ComplexProfileUpdatedNotif))]
    [ProtoInclude(11, typeof(ContactCreatedNotif))]
    [ProtoInclude(12, typeof(DeleteSessionsNotif))]
    [ProtoInclude(13, typeof(DocumentCreatedNotif))]
    [ProtoInclude(14, typeof(EntitiesVersionUpdatedNotif))]
    [ProtoInclude(15, typeof(InviteAcceptedNotif))]
    [ProtoInclude(16, typeof(InviteCancelledNotif))]
    [ProtoInclude(17, typeof(InviteCreatedNotif))]
    [ProtoInclude(18, typeof(InviteIgnoredNotif))]
    [ProtoInclude(19, typeof(LogoutNotif))]
    [ProtoInclude(20, typeof(MemberAccessUpdatedNotif))]
    [ProtoInclude(21, typeof(MembershipCreatedNotif))]
    [ProtoInclude(22, typeof(MessageCreatedNotif))]
    [ProtoInclude(23, typeof(MessageSeenNotif))]
    [ProtoInclude(24, typeof(ModuleCreatedNotif))]
    [ProtoInclude(25, typeof(ModulePermissionCreated))]
    [ProtoInclude(26, typeof(ModuleProfileUpdatedNotif))]
    [ProtoInclude(27, typeof(PendingCreatedNotif))]
    [ProtoInclude(28, typeof(PhotoCreatedNotif))]
    [ProtoInclude(29, typeof(RoomCreatedNotif))]
    [ProtoInclude(30, typeof(RoomCreatedSpecificNotif))]
    [ProtoInclude(31, typeof(RoomDeletionNotif))]
    [ProtoInclude(32, typeof(RoomProfileUpdatedNotif))]
    [ProtoInclude(33, typeof(RunPusherNotif))]
    [ProtoInclude(34, typeof(SessionCreatedNotif))]
    [ProtoInclude(35, typeof(SessionUpdatedNotif))]
    [ProtoInclude(36, typeof(TextAppendedToDocumentNotif))]
    [ProtoInclude(37, typeof(UserCreatedNotif))]
    [ProtoInclude(38, typeof(UserProfileUpdatedNotif))]
    [ProtoInclude(39, typeof(VideoCreatedNotif))]
    [ProtoInclude(40, typeof(WorkershipCreatedNotif))]
    [ProtoInclude(41, typeof(WorkershipDeletedNotif))]
    [ProtoInclude(42, typeof(WorkershipUpdatedNotif))]
    public class Notification : BasePack
    {
        [ProtoMember(101)]
        public Packet Packet { get; set; }
        [ProtoMember(102)]
        public string[] Destinations { get; set; }
    }
}