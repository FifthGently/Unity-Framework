namespace Frameworks
{
    using System;

    public abstract class HandleBase
    {
        protected HandleBase()
        {
        }

        public abstract PacketBase Create(StreamBuffer data, ushort cmd, ushort packetSize);
    }
}

