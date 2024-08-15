using RDC.Utils;
using System;

namespace CRSF{
    public class CRSFPackage{
        public static byte[] GetPayload(byte[] data){
            int payloadLenght = GetLenght(data) - 2;
            var payload = new byte[payloadLenght];
            Array.Copy(data, 3, payload, 0, payloadLenght);
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
            return CRC8.ComputeChecksum(crcPkt) == 0;
        }
        public static bool CheckPacket(byte[] data){
            if (data.Length < 5)
                return false;
            
            if (data[0] != 200)
                return false;
            
            if (!CheckCRC(data))
                return false;
            
            return true;
        }
    }
}