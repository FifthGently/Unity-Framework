namespace Frameworks
{
    using NetWork;
    using System;

    public class StreamBuffer
    {
        private const int CheckLength = 0x400;
        private const int DEFAULT_BUFFER_SIZE = 0x10000;
        private int m_iRead;
        private int m_iSize;
        private int m_iWrite;
        public byte[] m_lstBuffer;

        public StreamBuffer()
        {
            this.m_lstBuffer = new byte[0x10000];
            this.m_iSize = 0;
            this.m_iRead = this.m_iWrite = 0;
        }

        public StreamBuffer(byte[] buffer, int size)
        {
            this.m_lstBuffer = buffer;
            this.m_iWrite = this.m_iSize = size;
        }

        private void CheckSizeFull()
        {
            if (this.m_iWrite >= (this.m_lstBuffer.Length - 0x400))
            {
                this.FormationBuffer();
            }
        }

        private void FormationBuffer()
        {
            Array.Copy(this.m_lstBuffer, this.m_iRead, this.m_lstBuffer, 0, this.m_iSize);
            this.m_iRead = 0;
            this.m_iWrite = this.m_iRead + this.m_iSize;
        }

        public byte[] GetBuffer()
        {
            ushort length = 2;
            byte[] destinationArray = new byte[this.m_iSize + length];
            Array.Copy(BitConverter.GetBytes((int) (this.m_iSize + length)), 0, destinationArray, 0, length);
            Array.Copy(this.m_lstBuffer, this.m_iRead, destinationArray, length, this.m_iSize);
            return destinationArray;
        }

        public int GetLength()
        {
            if (this.m_lstBuffer == null)
            {
                return 0;
            }
            return this.m_lstBuffer.Length;
        }

        public int GetReadBuffLen()
        {
            if (this.m_iWrite > this.m_iRead)
            {
                return (this.m_iWrite - this.m_iRead);
            }
            return 0;
        }

        public void Init(byte[] buffer, int size)
        {
            this.m_lstBuffer = buffer;
            this.m_iWrite = this.m_iSize = size;
        }

        public void PrevReadIndexStep(int step)
        {
            this.m_iRead -= step;
            this.m_iSize += step;
        }

        public byte[] Read(int size)
        {
            if ((this.m_iRead + size) > this.m_iWrite)
            {
                SocketManager.Instance.OnLogMessage("Read Buffer is not have enough content.");
                return null;
            }
            byte[] destinationArray = new byte[size];
            Array.Copy(this.m_lstBuffer, this.m_iRead, destinationArray, 0, size);
            this.m_iRead += size;
            this.m_iSize -= size;
            return destinationArray;
        }

        public bool ReadBool()
        {
            return BitConverter.ToBoolean(this.Read(1), 0);
        }

        public byte ReadByte()
        {
            return this.Read(1)[0];
        }

        public char ReadChar()
        {
            return BitConverter.ToChar(this.Read(2), 0);
        }

        public char[] ReadChars(int num)
        {
            char[] chArray = new char[num];
            for (int i = 0; i < num; i++)
            {
                byte[] buffer = this.Read(2);
                chArray[i] = BitConverter.ToChar(buffer, 0);
            }
            return chArray;
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(this.Read(8), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(this.Read(4), 0);
        }

        public short ReadInt16()
        {
            return BitConverter.ToInt16(this.Read(2), 0);
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(this.Read(4), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(this.Read(8), 0);
        }

        public string ReadStr(int num)
        {
            string str = "";
            for (int i = 0; i < num; i++)
            {
                byte[] buffer = this.Read(2);
                str = str + BitConverter.ToChar(buffer, 0);
            }
            return str;
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(this.Read(2), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(this.Read(4), 0);
        }

        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(this.Read(8), 0);
        }

        public ushort ReadUnShort()
        {
            return this.Read(1)[0];
        }

        public void Reset()
        {
            this.m_iSize = this.m_iRead = this.m_iWrite = 0;
        }

        public void Write(int size)
        {
            this.m_iWrite += size;
            this.m_iSize += size;
            this.CheckSizeFull();
        }

        public bool Write(byte[] buffer, int size)
        {
            if ((this.m_iWrite + size) >= this.m_lstBuffer.Length)
            {
                SocketManager.Instance.OnLogMessage("Write Buffer is not enough.");
                return false;
            }
            Array.Copy(buffer, 0, this.m_lstBuffer, this.m_iWrite, size);
            this.Write(size);
            return true;
        }

        public void WriteData(bool data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(byte data)
        {
            byte[] buffer = new byte[] { data };
            this.Write(buffer, buffer.Length);
        }

        public void WriteData(char data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(double data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(short data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(int data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(long data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(float data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(string data)
        {
            foreach (char ch in data.ToCharArray())
            {
                byte[] bytes = BitConverter.GetBytes(ch);
                this.Write(bytes, bytes.Length);
            }
        }

        public void WriteData(ushort data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(uint data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(ulong data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            this.Write(bytes, bytes.Length);
        }

        public void WriteData(char[] data)
        {
            foreach (char ch in data)
            {
                byte[] bytes = BitConverter.GetBytes(ch);
                this.Write(bytes, bytes.Length);
            }
        }

        public int ReadIndex
        {
            get
            {
                return this.m_iRead;
            }
        }

        public int WriteIndex
        {
            get
            {
                return this.m_iWrite;
            }
        }
    }
}

