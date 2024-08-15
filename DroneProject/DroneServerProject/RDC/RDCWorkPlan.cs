using RDC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DroneServerProject.RDC
{
    public class Path
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Hight { get; set; }
        public float Speed { get; set; }
        public bool IsWorkPoint { get; set; }
        public bool IsFinished { get; set; }
        public bool IsHome { get; set; }
        public byte[] Encode()
        {
            byte[] data = new byte[17];
            Array.Copy(BitConverter.GetBytes(Latitude), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes(Longitude), 0, data, 3, 4);
            Array.Copy(BitConverter.GetBytes(Hight), 0, data, 7, 4);
            Array.Copy(BitConverter.GetBytes(Speed), 0, data, 11, 4);

            data[16] = (byte) (data[16] | (IsWorkPoint ? 1 :0));
            data[16] = (byte)(data[16] | (IsFinished ? 2 : 0));
            data[16] = (byte)(data[16] | (IsHome ? 4 : 0));

            return data;
        }
    }
    public class RDCWorkPlan
    {
        public List<Path> Path = new List<Path>();
        public RDCWorkPlan() { }
        public RDCWorkPlan(byte[] data)
        {
            Decode(data);
        }
        public void Decode(byte[] data)
        {
            if (RDCPackage.CheckPacket(data))
            {
                var payload = RDCPackage.GetPayload(data);

                var path = new Path();
                path.Latitude = BitConverter.ToSingle(payload, 0);
                path.Longitude = BitConverter.ToSingle(payload, 3);
                path.Hight = BitConverter.ToSingle(payload, 7);
                path.Speed = BitConverter.ToSingle(payload, 11);
                path.IsWorkPoint = (payload[17] & (1 << 1 - 1)) != 0;
                path.IsFinished = (payload[17] & (1 << 2 - 1)) != 0;
                path.IsHome = (payload[17] & (1 << 3 - 1)) != 0;
            }
        }
        public byte[] Encode()
        {
            //Payload
            //var batteryData =  
            
            byte[] message = new byte[4 + Path.Count * 17];

            //Cabeçalho da mensagem
            message[0] = 0xFF;  // Sync byte
            message[1] = Convert.ToByte(message.Length - 2);  //Lenght
            message[2] = 4; //Tipo da info

            //Adicionar o Payload
            int index = 3;
            for (int i = 0; i < Path.Count; i++)
            {
                Array.Copy(Path[i].Encode(), 0, message, index, 17);
                index += 17;
            }

            // Calcular checksum
            byte[] arrayCheckSum = new byte[message.Length - 2];
            Array.Copy(message, 2, arrayCheckSum, 0, arrayCheckSum.Length);
            message[arrayCheckSum.Length - 1] = CRC8.ComputeChecksum(arrayCheckSum);

            return message;
        }
    }
}
