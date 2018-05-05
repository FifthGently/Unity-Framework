namespace Frameworks
{
    using System;

    public class PacketHandle<T> : HandleBase where T: PacketBase, new()
    {
        public override PacketBase Create(StreamBuffer data, ushort cmd, ushort packetSize)
        {
            T local = Activator.CreateInstance<T>();
            local.Load(data, cmd, packetSize);
            return local;
        }
    }
}

