using ProtoBuf;

namespace HostVersion.Wrappers
{
    [ProtoContract]
    public enum WrapperType
    {
        [ProtoEnum]
        Question,
        [ProtoEnum]
        Answer,
        [ProtoEnum]
        Notification,
        [ProtoEnum]
        OutNetNotif
    }
}