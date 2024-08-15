using RDC.Utils;
using System;

namespace DroneServerProject.RDC
{
    public class RDCPackage{
        public static byte[] GetPayload(byte[] data){
            var payload = new byte[GetLenght(data) - 2];
            Array.Copy(data, 3, payload, 0, payload.Length);
            return payload;
        }
        public static int GetType(byte[] data){
            return data[2];
        }

        public static int GetLenght(byte[] data){
            return data[1];
        }
        public static bool CheckCRC(byte[] data){
            var crcPkt = new byte[data.Length - 2];
            Array.Copy(data, 2, crcPkt, 0, data.Length - 2);
            return CRC8.ComputeChecksum(data) == 0;
        }
        public static bool CheckPacket(byte[] data){
            if (data.Length < 5)
                return false;
            
            if (data[0] != 0xFF)
                return false;
            
            if (!CheckCRC(data))
                return false;
            
            return true;
        }
    }
}