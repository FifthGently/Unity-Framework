namespace Frameworks
{
    using NetWork;

    public interface PacketProtocolBase
    {
        PacketInfo ResolvePacketHead(StreamBuffer buff);
    }
}

