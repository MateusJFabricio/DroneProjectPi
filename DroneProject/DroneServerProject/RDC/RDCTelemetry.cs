using CRSF;
using RDC.Utils;
using System;
using System.Collections.Generic;

namespace DroneServerProject.RDC
{
    public class RDCTelemetry
    {
        public CRSF_FRAMETYPE_BATTERY_SENSOR Battery {get; set; } = new CRSF_FRAMETYPE_BATTERY_SENSOR();
        public CRSF_FRAMETYPE_GPS Gps {get; set; } = new CRSF_FRAMETYPE_GPS();
        public CRSF_FRAMETYPE_ATTITUDE Orientation {get; set; } = new CRSF_FRAMETYPE_ATTITUDE();
        public CRSF_FRAMETYPE_FLIGHT_MODE FlighMode {get; set; } = new CRSF_FRAMETYPE_FLIGHT_MODE();
        public RDCTelemetry(){}
        public RDCTelemetry(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data){
            if (RDCPackage.CheckPacket(data)){
                var payload = RDCPackage.GetPayload(data);

                var pkt = SplitPackage(payload, 0XC8);
                
                Battery.Decode(pkt[0]);
                Gps.Decode(pkt[1]);
                Orientation.Decode(pkt[2]);
                FlighMode.Decode(pkt[3]);
            }
        }
        public byte[] Encode(){
            //Payload
            var batteryData = Battery.ParseToRDC();
            var gpsData = Gps.ParseToRDC();
            var orientationData = Orientation.ParseToRDC();
            var flightModeData = FlighMode.ParseToRDC();

            byte[] message = new byte[4 + batteryData.Length + gpsData.Length + orientationData.Length + flightModeData.Length]; 

            //Cabeçalho da mensagem
            message[0] = 0xFF;  // Sync byte
            message[1] = Convert.ToByte(message.Length - 2);  //Lenght
            message[2] = 0x01;

            //Adicionar o Payload
            int index = 3;
            Array.Copy(batteryData, 0, message, index, batteryData.Length);
            index += batteryData.Length;
            Array.Copy(gpsData, 0, message, index, gpsData.Length);
            index += gpsData.Length;
            Array.Copy(orientationData, 0, message, index, orientationData.Length);
            index += orientationData.Length;
            Array.Copy(flightModeData, 0, message, index, flightModeData.Length);
            index += flightModeData.Length;

            // Calcular checksum
            byte[] arrayCheckSum = new byte[message.Length - 2];
            Array.Copy(message, 2, arrayCheckSum, 0, arrayCheckSum.Length);
            message[arrayCheckSum.Length - 1] = CRC8.ComputeChecksum(arrayCheckSum);
            
            return message;
        }
        public List<byte[]> SplitPackage(byte[] pacote, byte byteSplit){
            int index = 0;
            List<byte[]> list = new List<byte[]>();

            while(index < pacote.Length){
                
                if(byteSplit == 0xC8){
                    int lenght = pacote[index + 1];
                    if (pacote.Length < index + 2 + lenght) break;
                    byte[] b = new byte[lenght + 2];
                    Array.Copy(pacote, index, b, 0, lenght + 2);
                    list.Add(b);
                    index+=b.Length;
                }
            }

            return list;
        }
    }
}
