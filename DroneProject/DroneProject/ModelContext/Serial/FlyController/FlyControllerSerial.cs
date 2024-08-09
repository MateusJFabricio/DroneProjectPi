using CRSF;

namespace DroneProject.ModelContext.Serial.FlyController
{
    public class FlyControllerSerial : SerialCommunication
    {
        public CRSF_FRAMETYPE_BATTERY_SENSOR Battery {get; set; } = new();
        public CRSF_FRAMETYPE_GPS Gps {get; set; } = new();
        public CRSF_FRAMETYPE_RC_CHANNELS_PACKED ManualControl {get; set; } = new();
        public CRSF_FRAMETYPE_ATTITUDE Orientation {get; set; } = new();
        public CRSF_FRAMETYPE_FLIGHT_MODE FlighMode {get; set; } = new();
        private Thread _threadSerialDataExchange;
        private bool _stopThread = false;
        public bool SeriaDataExchangeRunning
        {
            get
            {
                if (_threadSerialDataExchange != null)
                {
                    return _threadSerialDataExchange.IsAlive;
                }
                else
                {
                    return false;
                }
            }
        }
        public FlyControllerSerial(string portName, int baudRate) : base(portName, baudRate)
        {
            OnSerialDataReceived += FlyControllerSerial_OnSerialDataReceived;
        }
        public void StartSerialDataExchange()
        {
            _stopThread = false;

            if (_threadSerialDataExchange != null)
            {
                if (_threadSerialDataExchange.IsAlive) return;
            }

            _threadSerialDataExchange = new Thread(() =>
            {
                while (!_stopThread)
                {
                    var crsfData = ManualControl.Encode();

                    //Falta adicionar o tratamento de erro aqui
                    SendData(crsfData, 1000);
                }
            });
            _threadSerialDataExchange.Start();
        }
        public void StopSerialDataExchange() 
        {
            _stopThread = true;
            if (_threadSerialDataExchange != null)
                _threadSerialDataExchange.Join();
        }
        private void FlyControllerSerial_OnSerialDataReceived(byte[] data)
        {
            try{
                foreach(var crsfPkt in CRSFProtocol.Decode(data))
                {
                    //Pacote de telemetria
                    if (crsfPkt is CRSF_FRAMETYPE_BATTERY_SENSOR)
                    {
                        Battery = (CRSF_FRAMETYPE_BATTERY_SENSOR)crsfPkt;
                    }

                    //Pacote de telemetria
                    if (crsfPkt is CRSF_FRAMETYPE_GPS)
                    {
                        Gps = (CRSF_FRAMETYPE_GPS)crsfPkt;
                    }

                    //Pacote de telemetria
                    if (crsfPkt is CRSF_FRAMETYPE_ATTITUDE)
                    {
                        Orientation = (CRSF_FRAMETYPE_ATTITUDE)crsfPkt;
                    }

                    //Pacote de telemetria
                    if (crsfPkt is CRSF_FRAMETYPE_FLIGHT_MODE)
                    {
                        FlighMode = (CRSF_FRAMETYPE_FLIGHT_MODE)crsfPkt;
                    }
                }
                
            }catch{

            }
        }
    }
}
