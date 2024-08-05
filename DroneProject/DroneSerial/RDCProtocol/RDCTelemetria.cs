
namespace RDC.RDCProtocol
{
    public class RDCTelemetria
    {
        public const int DATA_LENGTH = 32;
        public struct GPSData
        {
            public int latitude;
            public int longitude;
            public int speed;
            public int course;
            public int altitude;
            public int satellite_count;
        }
        public struct BatteryData
        {
            public int voltage;
            public int current;
            public int used_capacity;
            public int percentage;
        }
        public GPSData GPS {
            get
            {
                var gps = new GPSData();
                int i = 0;

                gps.latitude = BitConverter.ToInt32(_rawData, i += 4);
                gps.longitude = BitConverter.ToInt32(_rawData, i += 4);
                gps.speed = BitConverter.ToInt32(_rawData, i += 4);
                gps.course = BitConverter.ToInt32(_rawData, i += 4);
                gps.altitude = BitConverter.ToInt32(_rawData, i += 4);
                gps.satellite_count = BitConverter.ToInt32(_rawData, i += 4);
                return gps;
            }
            set 
            {
                int i = 0;
                Array.Copy(BitConverter.GetBytes(value.latitude), 0, _rawData, i+=4, 4);
                Array.Copy(BitConverter.GetBytes(value.longitude), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.speed), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.course), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.altitude), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.satellite_count), 0, _rawData, i += 4, 4);
            }
        }
        
        public DateTime DataHora {
            get 
            {
                int i = 28;
                return new DateTime(BitConverter.ToInt64(_rawData, i));
            }
            set
            {
                int i = 28;
                Array.Copy(BitConverter.GetBytes(value.Ticks), 0, _rawData, i, 4);
            }
        }
        public BatteryData Bateria 
        {
            get
            {
                var battery = new BatteryData();
                int i = 24;

                battery.voltage = BitConverter.ToInt32(_rawData, i += 4);
                battery.current = BitConverter.ToInt32(_rawData, i += 4);
                battery.used_capacity = BitConverter.ToInt32(_rawData, i += 4);
                battery.percentage = BitConverter.ToInt32(_rawData, i += 4);

                return battery;
            }
            set
            {
                int i = 24;
                Array.Copy(BitConverter.GetBytes(value.voltage), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.current), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.used_capacity), 0, _rawData, i += 4, 4);
                Array.Copy(BitConverter.GetBytes(value.percentage), 0, _rawData, i += 4, 4);
            }
        }
        public byte[] RawData { get => _rawData; set => _rawData = value; }
        private byte[] _rawData = new byte[DATA_LENGTH];
        public RDCTelemetria()
        {

        }
        public RDCTelemetria(byte[] data)
        {
            if (data.Length != DATA_LENGTH)
            {
                throw new Exception("Data nok");
            }

            _rawData = data;
        }
    }

    public class RDCManualControl
    {
        public const int DATA_LENGTH = 16;
        private const int ROW = 0;
        private const int PITCH = 2;
        private const int TROTLE = 4;
        private const int YAW = 6;
        private const int ENABLE = 8;
        public short Row
        {
            get => BitConverter.ToInt16(_rawData, ROW);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, ROW, 2);
        }
        public short Pitch
        {
            get => BitConverter.ToInt16(_rawData, TROTLE);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, TROTLE, 2);
        }
        public short Trotle
        {
            get => BitConverter.ToInt16(_rawData, PITCH);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, PITCH, 2);
        }
        
        public short Yaw { 
            get => BitConverter.ToInt16(_rawData, YAW);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, YAW, 2);
        }
        public short Enable
        {
            get => BitConverter.ToInt16(_rawData, ENABLE);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, ENABLE, 2);
        }

        public short[] Channels { 
            get 
            { 
                var channels = new short[16];
                for (int i = 0; i < channels.Length; i++)
                {
                    channels[i] = BitConverter.ToInt16(_rawData, i*2);
                }
                return channels;
            } 
        }
        public byte[] RawData { get => _rawData; set => _rawData = value; }
        private byte[] _rawData = new byte[DATA_LENGTH];
        public RDCManualControl()
        {
            
        }
        public RDCManualControl(byte[] data)
        {
            if (data.Length != DATA_LENGTH)
            {
                throw new Exception("Data nok");
            }
            RawData = data;
        }

        public void SetChannel(int channel, short value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, channel*2, 2);
        }
        /// <summary>
        /// Set do canal auxiliar iniciando em 1 até 12
        /// </summary>
        /// <param name="aux">Numero do canal auxiliar</param>
        /// <param name="value">valor do canal auxiliar</param>
        public void SetAuxChannel(int aux, short value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, 6 + (aux * 2), 2);
        }
    }
}
