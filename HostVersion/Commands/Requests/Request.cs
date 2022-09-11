using System.Collections.Generic;
using HostVersion.Commands.Requests.App;
using HostVersion.Commands.Requests.Auth;
using HostVersion.Commands.Requests.Bot;
using HostVersion.Commands.Requests.Complex;
using HostVersion.Commands.Requests.Contact;
using HostVersion.Commands.Requests.File;
using HostVersion.Commands.Requests.Internal;
using HostVersion.Commands.Requests.Invite;
using HostVersion.Commands.Requests.Message;
using HostVersion.Commands.Requests.Module;
using HostVersion.Commands.Requests.Pulse;
using HostVersion.Commands.Requests.Questions;
using HostVersion.Commands.Requests.Room;
using HostVersion.Commands.Requests.User;
using HostVersion.Middles;
using ProtoBuf;

namespace HostVersion.Commands.Requests
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AskIndexEntity))]
    [ProtoInclude(2, typeof(AskTest))]
    [ProtoInclude(3, typeof(CreateAppRequest))]
    [ProtoInclude(4, typeof(DeleteAccountRequest))]
    [ProtoInclude(5, typeof(LoginRequest))]
    [ProtoInclude(6, typeof(LogoutRequest))]
    [ProtoInclude(7, typeof(RegisterRequest))]
    [ProtoInclude(8, typeof(VerifyRequest))]
    [ProtoInclude(9, typeof(AddBotToRoomRequest))]
    [ProtoInclude(10, typeof(CreateBotRequest))]
    [ProtoInclude(11, typeof(GetBotRequest))]
    [ProtoInclude(12, typeof(GetBotsRequest))]
    [ProtoInclude(13, typeof(GetBotStoreContentRequest))]
    [ProtoInclude(14, typeof(GetCreatedBotsRequest))]
    [ProtoInclude(15, typeof(GetSubscribedBotsRequest))]
    [ProtoInclude(16, typeof(GetWorkershipsRequest))]
    [ProtoInclude(17, typeof(RemoveBotFromRoomRequest))]
    [ProtoInclude(18, typeof(SearchBotsRequest))]
    [ProtoInclude(19, typeof(SubscribeBotRequest))]
    [ProtoInclude(20, typeof(UpdateBotProfileRequest))]
    [ProtoInclude(21, typeof(UpdateWorkershipRequest))]
    [ProtoInclude(22, typeof(BotGetWorkershipsRequest))]
    [ProtoInclude(23, typeof(CreateComplexRequest))]
    [ProtoInclude(24, typeof(DeleteComplexRequest))]
    [ProtoInclude(25, typeof(GetComplexAccessesRequest))]
    [ProtoInclude(26, typeof(GetComplexByIdRequest))]
    [ProtoInclude(27, typeof(GetComplexesRequest))]
    [ProtoInclude(28, typeof(SearchComplexesRequest))]
    [ProtoInclude(29, typeof(UpdateComplexProfileRequest))]
    [ProtoInclude(30, typeof(UpdateMemberAccessRequest))]
    [ProtoInclude(31, typeof(CreateContactRequest))]
    [ProtoInclude(32, typeof(GetContactsRequest))]
    [ProtoInclude(33, typeof(BotAppendTextToTxtFileRequest))]
    [ProtoInclude(34, typeof(BotExecuteMongoComOnMongoFileRequest))]
    [ProtoInclude(35, typeof(BotExecuteSqlOnSqlFileRequest))]
    [ProtoInclude(36, typeof(CreateDocumentRequest))]
    [ProtoInclude(37, typeof(DownloadBotAvatarRequest))]
    [ProtoInclude(38, typeof(DownloadComplexAvatarRequest))]
    [ProtoInclude(39, typeof(DownloadFileRequest))]
    [ProtoInclude(40, typeof(DownloadRoomAvatarRequest))]
    [ProtoInclude(41, typeof(DownloadUserAvatarRequest))]
    [ProtoInclude(42, typeof(GetFileSizeRequest))]
    [ProtoInclude(43, typeof(UploadAudioRequest))]
    [ProtoInclude(44, typeof(UploadPhotoRequest))]
    [ProtoInclude(45, typeof(UploadVideoRequest))]
    [ProtoInclude(46, typeof(WriteToFileRequest))]
    [ProtoInclude(47, typeof(BotCreateDocumentFileRequest))]
    [ProtoInclude(48, typeof(ConsolidateSessionRequest))]
    [ProtoInclude(49, typeof(GetComplexWorkersRequest))]
    [ProtoInclude(50, typeof(GetModuleServerAddressRequest))]
    [ProtoInclude(51, typeof(PutServiceMessageRequest))]
    [ProtoInclude(52, typeof(AcceptInviteRequest))]
    [ProtoInclude(53, typeof(CancelInviteRequest))]
    [ProtoInclude(54, typeof(CreateInviteRequest))]
    [ProtoInclude(55, typeof(GetMyInvitesRequest))]
    [ProtoInclude(56, typeof(IgnoreInviteRequest))]
    [ProtoInclude(57, typeof(BotCreateFileMessageRequest))]
    [ProtoInclude(58, typeof(BotCreateTextMessageRequest))]
    [ProtoInclude(59, typeof(CreateFileMessageRequest))]
    [ProtoInclude(60, typeof(CreateTextMessageRequest))]
    [ProtoInclude(61, typeof(GetLastActionsRequest))]
    [ProtoInclude(62, typeof(GetMessageSeenCountRequest))]
    [ProtoInclude(63, typeof(GetMessagesRequest))]
    [ProtoInclude(64, typeof(NotifyMessageSeenRequest))]
    [ProtoInclude(65, typeof(BotPermitModuleRequest))]
    [ProtoInclude(66, typeof(CreateModuleRequest))]
    [ProtoInclude(67, typeof(RequestModuleRequest))]
    [ProtoInclude(68, typeof(SearchModulesRequest))]
    [ProtoInclude(69, typeof(UpdateModuleProfileRequest))]
    [ProtoInclude(70, typeof(AnimateBotViewRequest))]
    [ProtoInclude(71, typeof(ClickBotViewRequest))]
    [ProtoInclude(72, typeof(RequestBotViewRequest))]
    [ProtoInclude(73, typeof(RunCommandsOnBotViewRequest))]
    [ProtoInclude(74, typeof(SendBotViewRequest))]
    [ProtoInclude(75, typeof(UpdateBotViewRequest))]
    [ProtoInclude(76, typeof(CreateRoomRequest))]
    [ProtoInclude(77, typeof(DeleteRoomRequest))]
    [ProtoInclude(78, typeof(GetRoomByIdRequest))]
    [ProtoInclude(79, typeof(GetRoomsRequest))]
    [ProtoInclude(80, typeof(UpdateRoomProfileRequest))]
    [ProtoInclude(81, typeof(GetMeRequest))]
    [ProtoInclude(82, typeof(GetUserByIdRequest))]
    [ProtoInclude(83, typeof(SearchUsersRequest))]
    [ProtoInclude(84, typeof(UpdateUserProfileRequest))]
    [ProtoInclude(85, typeof(AskAddBotScreenShot))]
    [ProtoInclude(86, typeof(AskRemoveBotScreenShot))]
    [ProtoInclude(87, typeof(AskRequestBotPreview))]
    public class Request : ReqRes
    {
        [ProtoMember(101)]
        public long SessionId { get; set; }
        [ProtoMember(102)]
        public long SessionVersion { get; set; }
        [ProtoMember(103)]
        public Dictionary<string, string> Headers { get; set; }
        [ProtoMember(104)]
        public Packet Packet { get; set; }
        [ProtoMember(105)]
        public string Destination { get; set; }
    }
}