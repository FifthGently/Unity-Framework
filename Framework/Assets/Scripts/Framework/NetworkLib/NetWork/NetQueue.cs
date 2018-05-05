namespace NetWork
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class NetQueue<T>
    {
        private readonly object m_cLock;
        private int m_iCount;
        private int m_iHead;
        private int m_iSize;
        private T[] m_lstItems;

        public NetQueue(int capacity)
        {
            this.m_cLock = new object();
            this.m_lstItems = new T[capacity];
        }

        public void Clear()
        {
            lock (this.m_cLock)
            {
                for (int i = 0; i < this.m_lstItems.Length; i++)
                {
                    this.m_lstItems[i] = default(T);
                }
                this.m_iHead = 0;
                this.m_iSize = 0;
            }
        }

        public bool Contain(T item)
        {
            lock (this.m_cLock)
            {
                int iHead = this.m_iHead;
                for (int i = 0; i < this.m_iSize; i++)
                {
                    if (this.m_lstItems[iHead] == null)
                    {
                        if (item == null)
                        {
                            return true;
                        }
                    }
                    else if (this.m_lstItems[iHead].Equals(item))
                    {
                        return true;
                    }
                    iHead = (iHead + 1) % this.m_lstItems.Length;
                }
                return false;
            }
        }

        public bool Dequeue(out T item)
        {
            if (this.m_iSize == 0)
            {
                item = default(T);
                return false;
            }
            lock (this.m_cLock)
            {
                item = this.m_lstItems[this.m_iHead];
                this.m_lstItems[this.m_iHead] = default(T);
                this.m_iHead = (this.m_iHead + 1) % this.m_lstItems.Length;
                this.m_iSize--;
                return true;
            }
        }

        public int DequeueAll(IList<T> addTo)
        {
            if (this.m_iSize == 0)
            {
                return 0;
            }
            lock (this.m_cLock)
            {
                int iSize = this.m_iSize;
                while (this.m_iSize > 0)
                {
                    T item = this.m_lstItems[this.m_iHead];
                    addTo.Add(item);
                    this.m_lstItems[this.m_iHead] = default(T);
                    this.m_iHead = (this.m_iHead + 1) % this.m_lstItems.Length;
                    this.m_iSize--;
                }
                return iSize;
            }
        }

        public void Enqueue(T item)
        {
            lock (this.m_cLock)
            {
                if (this.m_iSize == this.m_lstItems.Length)
                {
                    this.SetCapacity(this.m_lstItems.Length + 8);
                }
                int index = (this.m_iHead + this.m_iSize) % this.m_lstItems.Length;
                this.m_lstItems[index] = item;
                this.m_iSize++;
            }
        }

        public void Enqueue(IEnumerable<T> items)
        {
            lock (this.m_cLock)
            {
                foreach (T local in items)
                {
                    if (this.m_iSize == this.m_lstItems.Length)
                    {
                        this.SetCapacity(this.m_lstItems.Length + 8);
                    }
                    int index = (this.m_iHead + this.m_iSize) % this.m_lstItems.Length;
                    this.m_lstItems[index] = local;
                    this.m_iSize++;
                }
            }
        }

        public void EnqueueFront(T item)
        {
            lock (this.m_cLock)
            {
                if (this.m_iSize >= this.m_lstItems.Length)
                {
                    this.SetCapacity(this.m_lstItems.Length + 8);
                }
                this.m_iHead--;
                if (this.m_iHead < 0)
                {
                    this.m_iHead = this.m_lstItems.Length - 1;
                }
                this.m_lstItems[this.m_iHead] = item;
                this.m_iSize++;
            }
        }

        public T Peek(int offset)
        {
            if (this.m_iSize == 0)
            {
                return default(T);
            }
            lock (this.m_cLock)
            {
                return this.m_lstItems[(this.m_iHead + offset) % this.m_lstItems.Length];
            }
        }

        private void SetCapacity(int newSize)
        {
            if (this.m_iSize == 0)
            {
                this.m_lstItems = new T[newSize];
                this.m_iHead = 0;
            }
            else
            {
                T[] destinationArray = new T[newSize];
                if (((this.m_iHead + this.m_iSize) - 1) < this.m_lstItems.Length)
                {
                    Array.Copy(this.m_lstItems, this.m_iHead, destinationArray, 0, this.m_iSize);
                }
                else
                {
                    Array.Copy(this.m_lstItems, this.m_iHead, destinationArray, 0, this.m_lstItems.Length - this.m_iHead);
                    Array.Copy(this.m_lstItems, 0, destinationArray, this.m_lstItems.Length - this.m_iHead, this.m_iSize - (this.m_lstItems.Length - this.m_iHead));
                }
                this.m_lstItems = destinationArray;
                this.m_iHead = 0;
            }
        }

        public T[] ToArray()
        {
            lock (this.m_cLock)
            {
                T[] localArray = new T[this.m_iSize];
                int iHead = this.m_iHead;
                for (int i = 0; i < this.m_iSize; i++)
                {
                    localArray[i] = this.m_lstItems[iHead++];
                    if (iHead >= this.m_lstItems.Length)
                    {
                        iHead = 0;
                    }
                }
                return localArray;
            }
        }

        public int Capacity
        {
            get
            {
                return this.m_lstItems.Length;
            }
        }

        public int Size
        {
            get
            {
                return this.m_iSize;
            }
        }
    }
}

