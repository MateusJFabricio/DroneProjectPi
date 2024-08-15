using RDC.RDCProtocol;

namespace DroneProject.ModelContext.Serial.ServerSerial
{
    public class ServerSerial : SerialCommunication
    {
        public RDCTelemetry Telemetria { get; set; } = new RDCTelemetry();
        public RDCManualControl ManualControl { get; set; } = new RDCManualControl();
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
        public ServerSerial(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
            OnSerialConnected += ServerSerial_OnSerialConnected;
            OnSerialDisconnected += ServerSerial_OnSerialDisconnected;
        }
        private void ServerSerial_OnSerialDisconnected()
        {
            Console.WriteLine("Server Serial Desconectado");
        }
        private void ServerSerial_OnSerialConnected()
        {
            Console.WriteLine("Server Serial Conectado");
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
                    try{
                        //Check Connection
                        if (!IsConnected)
                        {
                            Connect();
                        }
                        
                        //Recebimento de dados
                        if (NewDataAvailable){
                            ServerSerial_OnSerialDataReceived(SerialDataReceived);
                        }

                        //Envio de dados
                        //SendData(Telemetria.Encode(), 100);
                        
                        Console.WriteLine("Pitch: " + ManualControl.Pitch + "Yaw: " + ManualControl.Yaw + "Row: " + ManualControl.Row);
                    }catch(Exception ex){

                    }

                    Thread.Sleep(100);
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
        private void ServerSerial_OnSerialDataReceived(byte[] data)
        {
            try
            {
                foreach(var pkt in RDCProtocol.Decode(data)){
                    if (pkt != null)
                    {
                        //Pacote de Manual Control
                        if (pkt is RDCManualControl)
                        {
                            ManualControl = (RDCManualControl)pkt;
                            
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
