namespace Frameworks
{
    using System;

    public class PacketBase
    {
        protected ushort m_usPacketId;
        protected ushort m_usPacketSize;

        public virtual ushort GetResCmd()
        {
            return 0;
        }

        public virtual void Load(StreamBuffer stream, ushort cmd, ushort packetSize)
        {
            this.m_usPacketSize = packetSize;
            this.m_usPacketId = cmd;
        }

        public virtual void Write(StreamBuffer stream)
        {
            stream.WriteData(this.m_usPacketId);
        }

        public ushort PacketCmd
        {
            get
            {
                return this.m_usPacketId;
            }
            set
            {
                this.m_usPacketId = value;
            }
        }

        public ushort PacketSize
        {
            get
            {
                return this.m_usPacketSize;
            }
            set
            {
                this.m_usPacketSize = value;
            }
        }
    }
}

