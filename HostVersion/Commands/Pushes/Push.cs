using System.Collections.Generic;
using ProtoBuf;

namespace HostVersion.Commands.Pushes
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AudioMessagePush))]
    [ProtoInclude(2, typeof(BotAdditionToRoomPush))]
    [ProtoInclude(3, typeof(BotAnimatedBotViewPush))]
    [ProtoInclude(4, typeof(BotPropertiesChangedPush))]
    [ProtoInclude(5, typeof(BotRanCommandsOnBotViewPush))]
    [ProtoInclude(6, typeof(BotRemovationFromRoomPush))]
    [ProtoInclude(7, typeof(BotSentBotViewPush))]
    [ProtoInclude(8, typeof(BotUpdatedBotViewPush))]
    [ProtoInclude(9, typeof(ComplexDeletionPush))]
    [ProtoInclude(10, typeof(ContactCreationPush))]
    [ProtoInclude(11, typeof(InviteAcceptancePush))]
    [ProtoInclude(12, typeof(InviteCancellationPush))]
    [ProtoInclude(13, typeof(InviteCreationPush))]
    [ProtoInclude(14, typeof(InviteIgnoredPush))]
    [ProtoInclude(15, typeof(MemberAccessUpdatedPush))]
    [ProtoInclude(16, typeof(MessageSeenPush))]
    [ProtoInclude(17, typeof(ModulePermissionGrantedPush))]
    [ProtoInclude(18, typeof(PhotoMessagePush))]
    [ProtoInclude(19, typeof(RoomCreationPush))]
    [ProtoInclude(20, typeof(RoomDeletionPush))]
    [ProtoInclude(21, typeof(ServiceMessagePush))]
    [ProtoInclude(22, typeof(TextMessagePush))]
    [ProtoInclude(23, typeof(UserClickedBotViewPush))]
    [ProtoInclude(24, typeof(UserJointComplexPush))]
    [ProtoInclude(25, typeof(UserRequestedBotViewPush))]
    [ProtoInclude(26, typeof(VideoMessagePush))]
    [ProtoInclude(27, typeof(UserRequestedBotPreviewPush))]
    public class Push : BasePack
    {
        [ProtoMember(101)]
        public List<long> SessionIds { get; set; }
    }
}