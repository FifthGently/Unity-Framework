namespace NetWork
{
    using Frameworks;
    using System;
    using System.Collections.Generic;

    public class SocketDispatch
    {
        private NetQueue<PacketBase> m_cAckQueue = new NetQueue<PacketBase>(0x4000);
        private NetQueue<ConnectEvent> m_cConnectEventQueue = new NetQueue<ConnectEvent>(0x200);
        private Dictionary<ushort, HandleBase> m_mapHandles = new Dictionary<ushort, HandleBase>();
        private PacketProtocolBase protocol;

        public SocketDispatch()
        {
            this.m_cAckQueue.Clear();
            this.m_cConnectEventQueue.Clear();
        }

        public virtual void AckPacket(PacketBase packet)
        {
            this.m_cAckQueue.Enqueue(packet);
        }

        public PacketBase CreatePacket(ushort cmd, StreamBuffer data, ushort packetSize)
        {
            if (this.m_mapHandles.ContainsKey(cmd))
            {
                return this.m_mapHandles[cmd].Create(data, cmd, packetSize);
            }
            return null;
        }

        public void Destory()
        {
            this.m_cAckQueue = null;
            this.m_mapHandles = null;
            this.m_cConnectEventQueue = null;
            this.protocol = null;
        }

        internal void OnConnect()
        {
            SocketManager.Instance.OnConnect();
        }

        internal void OnDisconnect()
        {
            SocketManager.Instance.OnDisconnect();
        }

        internal void OnLogMessage(string msg)
        {
            SocketManager.Instance.OnLogMessage(msg);
        }

        internal void OnProcessPacketRes(PacketBase packet)
        {
            SocketManager.Instance.OnProcessPacketRes(packet);
        }

        internal void OnReConnect()
        {
            SocketManager.Instance.OnReConnect();
        }

        public virtual void PushConnectEvent(ConnectEvent connectEvent)
        {
            this.m_cConnectEventQueue.Enqueue(connectEvent);
        }

        public bool RegistHandle(ushort cmd, HandleBase handle)
        {
            if (this.m_mapHandles.ContainsKey(cmd))
            {
                return false;
            }
            this.m_mapHandles.Add(cmd, handle);
            return true;
        }

        public void RegistProtocol(PacketProtocolBase _protocol)
        {
            this.protocol = _protocol;
        }

        internal PacketInfo TestReadPacket(StreamBuffer buffer)
        {
            return this.protocol.ResolvePacketHead(buffer);
        }

        public virtual bool Update()
        {
            for (int i = 0; (i < 5) && (this.m_cAckQueue.Size > 0); i++)
            {
                PacketBase base2;
                bool flag = this.m_cAckQueue.Dequeue(out base2);
                if (((base2 != null) && flag) && this.m_mapHandles.ContainsKey(base2.PacketCmd))
                {
                    this.OnProcessPacketRes(base2);
                }
            }
            for (int j = 0; this.m_cConnectEventQueue.Size > 0; j++)
            {
                ConnectEvent event2;
                if (this.m_cConnectEventQueue.Dequeue(out event2))
                {
                    switch (event2)
                    {
                        case ConnectEvent.ConnectEventConnect:
                            this.OnConnect();
                            break;

                        case ConnectEvent.ConnectEventDisConnect:
                            this.OnDisconnect();
                            break;

                        case ConnectEvent.ConnectEventReconnect:
                            this.OnReConnect();
                            break;
                    }
                }
            }
            return true;
        }
    }
}

