namespace NetWork
{
    using Frameworks;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    public class ClientSocket
    {
        private bool IsUserClose;
        private SocketDispatch m_cDispatch;
        private StreamBuffer m_cReceiveBuffer;
        private PacketBase m_cSendPack;
        private NetQueue<PacketBase> m_cSendQueue;
        private Socket m_cSocket;
        private SESSION_STATUS m_cStatus = SESSION_STATUS.NO_CONNECT;
        private int m_iPort;
        private int m_iReConnectSecond = 0x7d0;
        private long m_lStartReConnectTime;
        private string m_strAddress;

        public ClientSocket(SocketDispatch _disPatch)
        {
            this.m_cDispatch = _disPatch;
            this.m_cReceiveBuffer = new StreamBuffer();
            this.m_cSendQueue = new NetQueue<PacketBase>(0x4000);
            this.m_cSendQueue.Clear();
        }

        protected void ChangeStatus(SESSION_STATUS status)
        {
            this.m_cStatus = status;
        }

        private bool CheckSockeState()
        {
            return ((this.m_cSocket != null) && this.m_cSocket.Connected);
        }

        public void Connect(string address, int port)
        {
            if (this.m_cSocket == null)
            {
                this.IsUserClose = false;
                try
                {
                    this.m_strAddress = address;
                    this.m_iPort = port;
                    this.m_cDispatch.OnLogMessage(string.Concat(new object[] { "to connect ip:", this.m_strAddress, " port:", this.m_iPort }));
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(address), port);
                    this.m_cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.m_cSocket.NoDelay = false;
                    if (this.m_cSocket == null)
                    {
                        throw new Exception("Socket create error.");
                    }
                    this.m_cSocket.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallBack), this.m_cSocket);
                    this.ChangeStatus(SESSION_STATUS.TOCONNECTING);
                    this.m_lStartReConnectTime = Environment.TickCount;
                }
                catch (Exception exception)
                {
                    this.m_cDispatch.OnLogMessage(exception.StackTrace);
                    if (this.m_cStatus == SESSION_STATUS.TOCONNECTING)
                    {
                        this.ToReConnect();
                    }
                }
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket asyncState = (Socket) ar.AsyncState;
                if (!this.CheckSockeState())
                {
                    this.m_cDispatch.OnLogMessage(string.Concat(new object[] { "Failed Socket connect :", this.m_strAddress, " port:", this.m_iPort }));
                    if (this.m_cStatus == SESSION_STATUS.TOCONNECTING)
                    {
                        this.ToReConnect();
                    }
                }
                else
                {
                    this.m_cDispatch.OnLogMessage(string.Concat(new object[] { "Success connected ip:", this.m_strAddress, " port:", this.m_iPort }));
                    asyncState.EndConnect(ar);
                    this.ChangeStatus(SESSION_STATUS.CONNECT_SUCCESS);
                    this.m_cDispatch.PushConnectEvent(ConnectEvent.ConnectEventConnect);
                    this.Receive();
                }
            }
            catch (Exception exception)
            {
                this.m_cDispatch.OnLogMessage(exception.StackTrace);
                if (this.m_cStatus == SESSION_STATUS.TOCONNECTING)
                {
                    this.ToReConnect();
                }
            }
        }

        public void Disconnect([Optional, System.ComponentModel.DefaultValue(true)] bool _IsUserClose)
        {
            this.IsUserClose = _IsUserClose;
            try
            {
                if (this.m_cSocket != null)
                {
                    if (this.m_cSocket.Connected)
                    {
                        this.m_cSocket.Shutdown(SocketShutdown.Both);
                    }
                    this.m_cSocket.Close();
                }
                this.m_cSocket = null;
                if (this.m_cDispatch != null)
                {
                    this.m_cDispatch.PushConnectEvent(ConnectEvent.ConnectEventDisConnect);
                }
                this.m_cReceiveBuffer.Reset();
                this.m_cDispatch.OnLogMessage("disConnected!");
            }
            catch (Exception exception)
            {
                this.m_cDispatch.OnLogMessage(exception.StackTrace);
            }
            this.ChangeStatus(SESSION_STATUS.NO_CONNECT);
        }

        private void DoReConnect()
        {
            try
            {
                this.Disconnect(false);
                this.Connect(this.m_strAddress, this.m_iPort);
                this.m_cDispatch.PushConnectEvent(ConnectEvent.ConnectEventReconnect);
            }
            catch (Exception exception)
            {
                this.m_cDispatch.OnLogMessage(exception.StackTrace);
                this.ChangeStatus(SESSION_STATUS.RE_CONNECT);
            }
        }

        public PacketBase GetLastSendPack()
        {
            return this.m_cSendPack;
        }

        public SESSION_STATUS GetStatus()
        {
            return this.m_cStatus;
        }

        public bool IsConnect()
        {
            return (this.m_cStatus == SESSION_STATUS.CONNECT_SUCCESS);
        }

        private void ProcessPacket()
        {
            while (this.m_cReceiveBuffer.GetReadBuffLen() >= 2)
            {
                PacketInfo info = this.m_cDispatch.TestReadPacket(this.m_cReceiveBuffer);
                if (info == null)
                {
                    return;
                }
                StreamBuffer data = new StreamBuffer();
                data.Init(info.content, info.size);
                PacketBase packet = this.m_cDispatch.CreatePacket(info.cmd, data, info.size);
                if (packet != null)
                {
                    this.m_cDispatch.AckPacket(packet);
                    this.m_cDispatch.OnLogMessage(string.Format("receivMsg:{0}", info.cmd));
                }
                else
                {
                    this.m_cDispatch.OnLogMessage(string.Format("not found register hander:{0}", info.cmd));
                }
            }
            return;
        }

        private bool Receive()
        {
            if (!this.CheckSockeState())
            {
                this.ToReConnect();
                return false;
            }
            this.m_cSocket.BeginReceive(this.m_cReceiveBuffer.m_lstBuffer, this.m_cReceiveBuffer.WriteIndex, this.m_cReceiveBuffer.GetLength() - this.m_cReceiveBuffer.WriteIndex, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), this.m_cSocket);
            return true;
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            Socket asyncState = (Socket) ar.AsyncState;
            try
            {
                int size = asyncState.EndReceive(ar);
                if (size <= 0)
                {
                    if (size == 0)
                    {
                        throw new Exception("Receive 0 bytes error.");
                    }
                    throw new Exception("Receive bytes error.");
                }
                this.m_cReceiveBuffer.Write(size);
                this.ProcessPacket();
                this.Receive();
            }
            catch (Exception)
            {
                this.m_cDispatch.OnLogMessage(string.Format("ReceiveErro:{0}", "与服务器断开"));
                if (!this.IsUserClose)
                {
                    this.ToReConnect();
                }
            }
        }

        public void reset()
        {
            this.m_cSendPack = null;
            this.m_cStatus = SESSION_STATUS.NO_CONNECT;
            this.m_cReceiveBuffer.Reset();
            this.m_cSendQueue.Clear();
        }

        public void Send()
        {
            if (!this.IsConnect())
            {
                this.ToReConnect();
            }
            else
            {
                PacketBase base2;
                if (this.m_cSendQueue.Dequeue(out base2))
                {
                    if (this.m_cSocket == null)
                    {
                        throw new Exception("The socket is null.");
                    }
                    byte[] buffer = null;
                    StreamBuffer stream = new StreamBuffer();
                    base2.Write(stream);
                    buffer = stream.GetBuffer();
                    if (buffer == null)
                    {
                        throw new Exception("The the send buffer is null.");
                    }
                    this.m_cSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.DontRoute, new AsyncCallback(this.SendCallBack), this.m_cSocket);
                }
            }
        }

        public void Send(PacketBase pb)
        {
            if (!this.IsConnect())
            {
                this.ToReConnect();
            }
            else if (pb != null)
            {
                this.m_cSendPack = pb;
                this.m_cSendQueue.Enqueue(pb);
                this.m_cDispatch.OnLogMessage(string.Format("sendMsg:{0}", pb.PacketCmd));
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket asyncState = (Socket) ar.AsyncState;
            if (this.CheckSockeState() && (this.m_cSocket.EndSend(ar) <= 0))
            {
                throw new Exception("The packet is not be send.");
            }
        }

        public void SetReConnectSecond(int second)
        {
            this.m_iReConnectSecond = second * 0x3e8;
        }

        public void ToReConnect()
        {
            this.IsUserClose = false;
            this.m_lStartReConnectTime = Environment.TickCount;
            this.ChangeStatus(SESSION_STATUS.RE_CONNECT);
        }

        public bool Update()
        {
            if (((this.m_cStatus == SESSION_STATUS.RE_CONNECT) || (this.m_cStatus == SESSION_STATUS.TOCONNECTING)) && ((this.m_iReConnectSecond > 0) && ((Environment.TickCount - this.m_lStartReConnectTime) >= this.m_iReConnectSecond)))
            {
                this.DoReConnect();
            }
            if (this.m_cStatus != SESSION_STATUS.CONNECT_SUCCESS)
            {
                return false;
            }
            if (this.m_cDispatch != null)
            {
                this.m_cDispatch.Update();
            }
            for (int i = 0; i < 5; i++)
            {
                this.Send();
            }
            return true;
        }
    }
}

