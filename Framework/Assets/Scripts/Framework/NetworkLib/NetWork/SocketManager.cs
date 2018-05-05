namespace NetWork
{
    using Frameworks;
    using System;
    using System.Runtime.InteropServices;

    public class SocketManager
    {
        public static SocketManager Instance { get { return Singleton<SocketManager>.Instance; } }

        private bool isInitProtocol;
        private ClientSocket m_lstSocket;
        private EventDispatcher mEventDispatcher = new EventDispatcher();
        private SocketDispatch mSocketDispatch = new SocketDispatch();

        public void AddEventListener(int eventType, EventListenerDelegate listener)
        {
            this.mEventDispatcher.AddEventListener(eventType.ToString(), listener);
        }

        public void AddEventListener(string eventType, EventListenerDelegate listener)
        {
            this.mEventDispatcher.AddEventListener(eventType, listener);
        }

        public void Connect(string ip, int port)
        {
            if (!this.isInitProtocol)
            {
                throw new Exception("Please Call InitProtocol Method First!");
            }
            this.CreateClientSocket();
            this.m_lstSocket.Connect(ip, port);
        }

        private void CreateClientSocket()
        {
            if (this.m_lstSocket == null)
            {
                this.m_lstSocket = new ClientSocket(this.mSocketDispatch);
            }
        }

        public void Destory()
        {
            this.Disconnect();
            if (this.mSocketDispatch != null)
            {
                this.mSocketDispatch.Destory();
            }
            this.mSocketDispatch = null;
            this.m_lstSocket = null;
            if (this.mEventDispatcher != null)
            {
                this.mEventDispatcher.Destory();
            }
            this.mEventDispatcher = null;
        }

        public void Disconnect()
        {
            if (this.m_lstSocket != null)
            {
                this.m_lstSocket.Disconnect(true);
            }
        }

        public void InitProtocol(PacketProtocolBase protocol)
        {
            this.mSocketDispatch.RegistProtocol(protocol);
            this.isInitProtocol = true;
        }

        internal void OnConnect()
        {
            BaseEvent baseEvent = new BaseEvent(SocketEvent.OnConnection);
            this.ProcessEvents(baseEvent);
        }

        internal void OnDisconnect()
        {
            BaseEvent baseEvent = new BaseEvent(SocketEvent.OnDisconnection);
            this.ProcessEvents(baseEvent);
        }

        internal void OnLogMessage(string msg)
        {
            BaseEvent baseEvent = new BaseEvent(SocketEvent.LogMessage) {
                Target = msg
            };
            this.ProcessEvents(baseEvent);
        }

        internal void OnProcessPacketRes(PacketBase packet)
        {
            BaseEvent baseEvent = new BaseEvent(packet.PacketCmd.ToString()) {
                Target = packet
            };
            this.ProcessEvents(baseEvent);
        }

        internal void OnReConnect()
        {
            BaseEvent baseEvent = new BaseEvent(SocketEvent.OnReConnection);
            this.ProcessEvents(baseEvent);
        }

        public void ProcessEvents(BaseEvent baseEvent)
        {
            this.mEventDispatcher.BroadcastEvent(baseEvent);
        }

        public void ReConnect()
        {
            if (this.m_lstSocket != null)
            {
                this.m_lstSocket.ToReConnect();
            }
        }

        public bool RegistHandle(ushort cmd, HandleBase handle)
        {
            return this.mSocketDispatch.RegistHandle(cmd, handle);
        }

        public void RemoveAllEventListeners()
        {
            this.mEventDispatcher.RemoveAll();
        }

        public void RemoveEventListener(int eventType, [Optional, UnityEngine.Internal.DefaultValue(null)] EventListenerDelegate listener)
        {
            if (listener != null)
            {
                this.mEventDispatcher.RemoveEventListener(eventType.ToString(), listener);
            }
            else
            {
                this.mEventDispatcher.RemoveEventListener(eventType.ToString());
            }
        }

        public void RemoveEventListener(string eventType, [Optional, UnityEngine.Internal.DefaultValue(null)] EventListenerDelegate listener)
        {
            if (listener != null)
            {
                this.mEventDispatcher.RemoveEventListener(eventType, listener);
            }
            else
            {
                this.mEventDispatcher.RemoveEventListener(eventType);
            }
        }

        public void Send(PacketBase packet)
        {
            if (this.m_lstSocket == null)
            {
                this.CreateClientSocket();
            }
            else if (!this.m_lstSocket.IsConnect())
            {
                this.m_lstSocket.ToReConnect();
            }
            else
            {
                this.m_lstSocket.Send(packet);
            }
        }

        public void SetReConnectSecond(int second)
        {
            if (this.m_lstSocket != null)
            {
                this.m_lstSocket.SetReConnectSecond(second);
            }
        }

        public bool Update()
        {
            if (this.m_lstSocket == null)
            {
                return false;
            }
            if (this.m_lstSocket != null)
            {
                this.m_lstSocket.Update();
            }
            return true;
        }
    }
}

