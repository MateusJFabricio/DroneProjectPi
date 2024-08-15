using System;
using System.Numerics;
using System.Text;
using RDC.Utils;

namespace CRSF{
    public class CRSF_FRAMETYPE_BATTERY_SENSOR : ICRSFPackage
    {
        private byte[] _rawPacket;
        public float Voltage {get;set;}
        public float Current {get;set;}
        public float UsedCapacity {get;set;}
        public float Percentage {get;set;}
        public CRSF_FRAMETYPE_BATTERY_SENSOR(){}
        public CRSF_FRAMETYPE_BATTERY_SENSOR(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data){
            if (CRSFPackage.CheckPacket(data)){
                var payload = CRSFPackage.GetPayload(data);
                Voltage = BitConverter.ToInt16(new byte[] { payload[1], payload[0] }, 0)/10;
                Current = BitConverter.ToInt16(new byte[] { payload[3], payload[2] }, 0)/10;
                UsedCapacity = 0;// BitConverter.ToSingle([0, payload[6], payload[5], payload[4]], 0);
                Percentage = Convert.ToSingle(data[7]);

                _rawPacket = data;
            }
        }
        public byte[] Encode(){
            return _rawPacket;
        }
        public byte[] ParseToRDC()
        {
            var data = new byte[16];
            Array.Copy(BitConverter.GetBytes(Voltage), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes(Current), 0, data, 3, 4);
            Array.Copy(BitConverter.GetBytes(UsedCapacity), 0, data, 7, 4);
            Array.Copy(BitConverter.GetBytes(Percentage), 0, data, 11, 4);
            return data;
        }
    }
    public class CRSF_FRAMETYPE_GPS : ICRSFPackage
    {
        private byte[] _rawPacket;
        public float Latitude {get; set;}
        public float Longitude {get; set;}
        public float Speed {get; set;}
        public int Course {get; set;}
        public int Altitude {get; set;}
        public int SatelliteCount {get; set;}
        public CRSF_FRAMETYPE_GPS(){}
        public CRSF_FRAMETYPE_GPS(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data){
            if (CRSFPackage.CheckPacket(data)){
                var payload = CRSFPackage.GetPayload(data);
                Latitude = float.Parse(BitConverter.ToInt32(new byte[] { payload[3], payload[2], payload[1], payload[0]}, 0).ToString()) / 10000000;
                Longitude = float.Parse(BitConverter.ToInt32(new byte[] { payload[7], payload[6], payload[5], payload[4] }, 0).ToString()) / 10000000;
                Speed = float.Parse(BitConverter.ToInt16(new byte[] { payload[9], payload[8] }, 0).ToString());// / 10;
                Course = BitConverter.ToInt16(new byte[] { payload[11], payload[10]}, 0) / 100;
                Altitude = BitConverter.ToUInt16(new byte[] { payload[13], payload[12] }, 0) - 1000;
                SatelliteCount = Convert.ToInt32(payload[14]);

                _rawPacket = data;
            }
        }
        public byte[] Encode(){
            return _rawPacket;
        }
        public byte[] ParseToRDC()
        {
            var data = new byte[24];
            Array.Copy(BitConverter.GetBytes(Latitude), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes(Longitude), 0, data, 3, 4);
            Array.Copy(BitConverter.GetBytes(Speed), 0, data, 7, 4);
            Array.Copy(BitConverter.GetBytes(Course), 0, data, 11, 4);
            Array.Copy(BitConverter.GetBytes(Altitude), 0, data, 15, 4);
            Array.Copy(BitConverter.GetBytes(SatelliteCount), 0, data, 19, 4);
            return data;
        }
    }
    public class CRSF_FRAMETYPE_RC_CHANNELS_PACKED : ICRSFPackage
    {
        private const int CHANNEL_MIN_VALUE = 0;
        private const int CHANNEL_MAX_VALUE = 1984;
        public ushort[] Channels { get; set; } = new ushort[16];
        public int Row 
        {
            get
            {
                return Convert.ToInt32(Channels[0]);
            } 
            set
            {
                if (value > CHANNEL_MIN_VALUE || value < CHANNEL_MAX_VALUE)
                {
                    Channels[0] = Convert.ToUInt16(value);
                }
            } 
        }
        public int Pitch 
        {
            get
            {
                return Convert.ToInt32(Channels[1]);
            } 
            set
            {
                if (value > CHANNEL_MIN_VALUE || value < CHANNEL_MAX_VALUE)
                {
                    Channels[1] = Convert.ToUInt16(value);
                }
            } 
        }
        public int Yaw 
        {
            get
            {
                return Convert.ToInt32(Channels[3]);
            } 
            set
            {
                if (value > CHANNEL_MIN_VALUE || value < CHANNEL_MAX_VALUE)
                {
                    Channels[3] = Convert.ToUInt16(value);
                }
            } 
        }
        public int Trotle 
        {
            get
            {
                return Convert.ToInt32(Channels[2]);
            } 
            set
            {
                if (value > CHANNEL_MIN_VALUE || value < CHANNEL_MAX_VALUE)
                {
                    Channels[2] = Convert.ToUInt16(value);
                }
            } 
        }
        public CRSF_FRAMETYPE_RC_CHANNELS_PACKED()
        {
            Channels[0] = 992;
            Channels[1] = 992;
            Channels[2] = 992;
            Channels[3] = 992;
            Channels[4] = 992;
            Channels[5] = 992;
            Channels[6] = 992;
            Channels[7] = 992;
            Channels[8] = 992;
            Channels[9] = 992;
            Channels[10] = 992;
            Channels[11] = 992;
            Channels[12] = 992;
            Channels[13] = 992;
            Channels[14] = 992;
            Channels[15] = 992;
        }
        public CRSF_FRAMETYPE_RC_CHANNELS_PACKED(ushort[] data)
        {
            Channels = data;
        }
        public CRSF_FRAMETYPE_RC_CHANNELS_PACKED(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data){
            throw new NotImplementedException("Encode não implementado ainda");
        }
        public byte[] Encode(){
            byte[] message = new byte[26]; // Sync (1 byte) + tipo de mensagem (1 byte) + dados (22 bytes) + checksum (1 byte)

            //Cabeçalho da mensagem
            message[0] = 0xC8;  // Sync byte
            message[1] = 0x18;  //Lenght
            message[2] = 0x16;  // Tipo de mensagem CRSF_FRAMETYPE_RC_CHANNELS_PACKED

            int offset = 0;
            for (int i = 0; i < Channels.Length; i++)
            {
                WriteUshortInByteArray(ref message, Channels[i], ref offset);
            }

            // Calcular checksum
            byte[] arrayCheckSum = new byte[23];
            Array.Copy(message, 2, arrayCheckSum, 0, 23);
            message[25] = CRC8.ComputeChecksum(arrayCheckSum);

            return message;
        }
        public void SetChannelAux(int auxNum, int value)
        {
            Channels[auxNum + 3] = Convert.ToUInt16(value);
        }
        public ushort GetChannelAuxValue(int auxNum)
        {
            return Channels[auxNum + 3];
        }
        private void WriteUshortInByteArray(ref byte[] byteArray, ushort channel, ref int offset)
        {
            int bytePosition = (offset / 8) + 3;
            int bitPosition = offset % 8;

            uint tempShort = (uint) (channel << bitPosition);
            byteArray[bytePosition] = (byte) (BitConverter.GetBytes(tempShort)[0] | byteArray[bytePosition]);
            byteArray[bytePosition + 1] = (byte) (BitConverter.GetBytes(tempShort)[1] | byteArray[bytePosition + 1]);
            byteArray[bytePosition + 2] = (byte) (BitConverter.GetBytes(tempShort)[2] | byteArray[bytePosition + 2]);
            offset += 11;
        }
    }
    public class CRSF_FRAMETYPE_ATTITUDE : ICRSFPackage
    {
        private byte[] _rawPacket;
        public float Pitch { get; set; }
        public float Row { get; set; }
        public float Yaw { get; set; }
        public CRSF_FRAMETYPE_ATTITUDE(){

        }
        public CRSF_FRAMETYPE_ATTITUDE(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data)
        {
            if (CRSFPackage.CheckPacket(data)){
                var payload = CRSFPackage.GetPayload(data);
                Pitch = ConvertToAngle(BitConverter.ToInt16(new byte[] { payload[1], payload[0] }, 0));
                Row = ConvertToAngle(BitConverter.ToInt16(new byte[] { payload[3], payload[2] }, 0));
                Yaw = ConvertToAngle(BitConverter.ToInt16(new byte[] { payload[5], payload[4] }, 0));

                _rawPacket = data;
            }
        }

        public byte[] Encode()
        {
            return _rawPacket;
        }
        public byte[] ParseToRDC()
        {
            var data = new byte[12];
            Array.Copy(BitConverter.GetBytes(Pitch), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes(Row), 0, data, 3, 4);
            Array.Copy(BitConverter.GetBytes(Yaw), 0, data, 7, 4);
            return data;
        }
        private float ConvertToAngle(short value){
            return float.Parse(value.ToString()) / 10000 * 180/ 3.1415f;
        }
    }
    public class CRSF_FRAMETYPE_FLIGHT_MODE : ICRSFPackage
    {
        private byte[] _rawPacket;
        public string FlightMode { get; set; } = "";
        public CRSF_FRAMETYPE_FLIGHT_MODE(){

        }
        public CRSF_FRAMETYPE_FLIGHT_MODE(byte[] data){
            Decode(data);
        }
        public void Decode(byte[] data)
        {
            if (CRSFPackage.CheckPacket(data)){
                var payload = CRSFPackage.GetPayload(data);
                FlightMode = System.Text.Encoding.Default.GetString(payload);

                _rawPacket = data;
            }
        }
        public byte[] Encode()
        {
            return _rawPacket;
        }
        public byte[] ParseToRDC()
        {
            return Encoding.ASCII.GetBytes(FlightMode);
        }
    }
    
}