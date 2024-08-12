using System;
using System.IO;
using Iterum.Logs;
using ProtoBuf;

namespace NCV.Network.WS
{
    public static class WSDataHelper
    {
        public static byte[] ObjectToBytes<T>(T instance)
        {
            byte[] result = null;
            try
            {
                if (instance == null)
                {
                    result = Array.Empty<byte>();
                }
                else
                {
                    var memoryStream = new MemoryStream();
                    Serializer.Serialize(memoryStream, instance);
                    result = new byte[memoryStream.Length];
                    memoryStream.Position = 0L;
                    memoryStream.Read(result, 0, result.Length);
                    memoryStream.Dispose();
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return result;
        }


        public static object BytesToObject(Type type, byte[] bytesData)
        {
            object result = null;
            if (bytesData.Length <= 0) return null;

            try
            {
                var memoryStream = new MemoryStream();
                memoryStream.Write(bytesData, 0, bytesData.Length);
                memoryStream.Position = 0L;

                result = Serializer.Deserialize(type, memoryStream);
                memoryStream.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return result;
        }

        public static T BytesToObject<T>(byte[] bytesData)
        {
            return (T)BytesToObject(typeof(T), bytesData);
        }
    }
}
