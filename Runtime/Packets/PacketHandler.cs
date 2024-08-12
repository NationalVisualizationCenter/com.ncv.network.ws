namespace NCV.Network.WS
{

    public abstract class PacketHandler
    {

        public abstract Packet GetServerPacket(short id);

        public abstract void Invoke(Packet result);
    }
}
