using System;
using System.Buffers;
using System.IO;
using Be.IO;

namespace NCV.Network.WS
{

    public abstract class Packet
    {
        public abstract short Index { get; }

        protected IPacketData ReadData { get; set; }
        protected Type ReadDataType { get; set; }

        protected byte[] WriteDataBuffer { get; set; }

        public object GetData()
        {
            return ReadData;
        }

        protected void SetData<T>(T value) where T : IPacketData
        {
            ReadData = value;
            ReadDataType = typeof(T);
        }

        protected void ReadPacket(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var buffer = reader.ReadBytes(length);
            ReadData = (IPacketData)WSDataHelper.BytesToObject(ReadDataType, buffer);
        }

        public virtual void WritePacket(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(WriteDataBuffer.Length);
            writer.Write(WriteDataBuffer);

            // Log.Debug("Payload DEC", string.Join(" ", WriteDataBuffer));
            // Log.Debug("Payload HEX", BitConverter.ToString(WriteDataBuffer).Replace("-", " "));
        }

        public static Packet ReceivePacket(byte[] rawBytes, PacketHandler packetHandler)
        {
            if (rawBytes.Length < 2) return null; //| 2Bytes: Packet ID |

            using var stream = new MemoryStream(rawBytes, 0, rawBytes.Length);
            using var reader = new BeBinaryReader(stream);

            try
            {
                short id = reader.ReadInt16();

                var p = packetHandler.GetServerPacket(id);
                if (p == null) return null;

                p.ReadPacket(reader);
                return p;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] StartWritePacket(Packet packet)
        {
            packet.WriteDataBuffer = WSDataHelper.ObjectToBytes(packet.ReadData);
            var buffer =
                ArrayPool<byte>.Shared.Rent(2 /* packetId */ + 4 /* buffer length */ + packet.WriteDataBuffer.Length);
            using var bw = new BeBinaryWriter(new MemoryStream(buffer));
            packet.WritePacket(bw);
            return buffer;
        }


        public static void EndWritePacket(byte[] buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer);

        }
    }

    public interface IPacketData
    {
    }
}
