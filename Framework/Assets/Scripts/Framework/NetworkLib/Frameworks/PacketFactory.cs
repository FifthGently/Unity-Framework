namespace Frameworks
{
    using System;

    public abstract class PacketFactory
    {
        protected PacketFactory()
        {
        }

        public abstract PacketBase Create();
        public abstract ushort GetPacketId();
        public abstract ushort GetPacketSize();
    }
}

