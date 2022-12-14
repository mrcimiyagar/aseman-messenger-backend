using HostVersion.Commands.Requests.Answers;
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
using HostVersion.Commands.Requests.Room;
using HostVersion.Commands.Requests.User;
using HostVersion.Middles;
using ProtoBuf;

namespace HostVersion.Commands.Requests
{
    [ProtoContract]
    [ProtoInclude(1, typeof(AnswerIndexEntity))]
    [ProtoInclude(2, typeof(AnswerTest))]
    [ProtoInclude(3, typeof(CreateAppResponse))]
    [ProtoInclude(4, typeof(DeleteAccountResponse))]
    [ProtoInclude(5, typeof(LoginResponse))]
    [ProtoInclude(6, typeof(LogoutResponse))]
    [ProtoInclude(7, typeof(RegisterResponse))]
    [ProtoInclude(8, typeof(VerifyResponse))]
    [ProtoInclude(9, typeof(AddBotToRoomResponse))]
    [ProtoInclude(10, typeof(CreateBotResponse))]
    [ProtoInclude(11, typeof(GetBotResponse))]
    [ProtoInclude(12, typeof(GetBotsResponse))]
    [ProtoInclude(13, typeof(GetBotStoreContentResponse))]
    [ProtoInclude(14, typeof(GetCreatedBotsResponse))]
    [ProtoInclude(15, typeof(GetSubscribedBotsResponse))]
    [ProtoInclude(16, typeof(GetWorkershipsResponse))]
    [ProtoInclude(17, typeof(RemoveBotFromRoomResponse))]
    [ProtoInclude(18, typeof(SearchBotsResponse))]
    [ProtoInclude(19, typeof(SubscribeBotResponse))]
    [ProtoInclude(20, typeof(UpdateBotProfileResponse))]
    [ProtoInclude(21, typeof(UpdateWorkershipResponse))]
    [ProtoInclude(22, typeof(BotGetWorkershipsResponse))]
    [ProtoInclude(23, typeof(CreateComplexResponse))]
    [ProtoInclude(24, typeof(DeleteComplexResponse))]
    [ProtoInclude(25, typeof(GetComplexAccessesResponse))]
    [ProtoInclude(26, typeof(GetComplexByIdResponse))]
    [ProtoInclude(27, typeof(GetComplexesResponse))]
    [ProtoInclude(28, typeof(SearchComplexesResponse))]
    [ProtoInclude(29, typeof(UpdateComplexProfileResponse))]
    [ProtoInclude(30, typeof(UpdateMemberAccessResponse))]
    [ProtoInclude(31, typeof(CreateContactResponse))]
    [ProtoInclude(32, typeof(GetContactsResponse))]
    [ProtoInclude(33, typeof(BotAppendTextToTxtFileResponse))]
    [ProtoInclude(34, typeof(BotExecuteMongoComOnMongoFileResponse))]
    [ProtoInclude(35, typeof(BotExecuteSqlOnSqlFileResponse))]
    [ProtoInclude(36, typeof(CreateDocumentResponse))]
    [ProtoInclude(37, typeof(DownloadBotAvatarResponse))]
    [ProtoInclude(38, typeof(DownloadComplexAvatarResponse))]
    [ProtoInclude(39, typeof(DownloadFileResponse))]
    [ProtoInclude(40, typeof(DownloadRoomAvatarResponse))]
    [ProtoInclude(41, typeof(DownloadUserAvatarResponse))]
    [ProtoInclude(42, typeof(GetFileSizeResponse))]
    [ProtoInclude(43, typeof(UploadAudioResponse))]
    [ProtoInclude(44, typeof(UploadPhotoResponse))]
    [ProtoInclude(45, typeof(UploadVideoResponse))]
    [ProtoInclude(46, typeof(WriteToFileResponse))]
    [ProtoInclude(47, typeof(BotCreateDocumentFileResponse))]
    [ProtoInclude(48, typeof(ConsolidateSessionResponse))]
    [ProtoInclude(49, typeof(GetComplexWorkersResponse))]
    [ProtoInclude(50, typeof(GetModuleServerAddressResponse))]
    [ProtoInclude(51, typeof(PutServiceMessageResponse))]
    [ProtoInclude(52, typeof(AcceptInviteResponse))]
    [ProtoInclude(53, typeof(CancelInviteResponse))]
    [ProtoInclude(54, typeof(CreateInviteResponse))]
    [ProtoInclude(55, typeof(GetMyInvitesResponse))]
    [ProtoInclude(56, typeof(IgnoreInviteResponse))]
    [ProtoInclude(57, typeof(BotCreateFileMessageResponse))]
    [ProtoInclude(58, typeof(BotCreateTextMessageResponse))]
    [ProtoInclude(59, typeof(CreateFileMessageResponse))]
    [ProtoInclude(60, typeof(CreateTextMessageResponse))]
    [ProtoInclude(61, typeof(GetLastActionsResponse))]
    [ProtoInclude(62, typeof(GetMessageSeenCountResponse))]
    [ProtoInclude(63, typeof(GetMessagesResponse))]
    [ProtoInclude(64, typeof(NotifyMessageSeenResponse))]
    [ProtoInclude(65, typeof(BotPermitModuleResponse))]
    [ProtoInclude(66, typeof(CreateModuleResponse))]
    [ProtoInclude(67, typeof(RequestModuleResponse))]
    [ProtoInclude(68, typeof(SearchModulesResponse))]
    [ProtoInclude(69, typeof(UpdateModuleProfileResponse))]
    [ProtoInclude(70, typeof(AnimateBotViewResponse))]
    [ProtoInclude(71, typeof(ClickBotViewResponse))]
    [ProtoInclude(72, typeof(RequestBotViewResponse))]
    [ProtoInclude(73, typeof(RunCommandsOnBotViewResponse))]
    [ProtoInclude(74, typeof(SendBotViewResponse))]
    [ProtoInclude(75, typeof(UpdateBotViewResponse))]
    [ProtoInclude(76, typeof(CreateRoomResponse))]
    [ProtoInclude(77, typeof(DeleteRoomResponse))]
    [ProtoInclude(78, typeof(GetRoomByIdResponse))]
    [ProtoInclude(79, typeof(GetRoomsResponse))]
    [ProtoInclude(80, typeof(UpdateRoomProfileResponse))]
    [ProtoInclude(81, typeof(GetMeResponse))]
    [ProtoInclude(82, typeof(GetUserByIdResponse))]
    [ProtoInclude(83, typeof(SearchUsersResponse))]
    [ProtoInclude(84, typeof(UpdateUserProfileResponse))]
    [ProtoInclude(85, typeof(AnswerAddBotScreenShot))]
    [ProtoInclude(86, typeof(AnswerRemoveBotScreenShot))]
    [ProtoInclude(87, typeof(AnswerRequestBotPreview))]
    public class Response : ReqRes
    {
        [ProtoMember(101)]
        public Packet Packet { get; set; }
    }
}