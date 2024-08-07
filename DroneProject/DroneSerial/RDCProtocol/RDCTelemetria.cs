
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
}
