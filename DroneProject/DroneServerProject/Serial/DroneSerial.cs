using DroneServerProject.RDC;
using System;
using System.Threading;

namespace DroneServerProject.Serial
{
    public class DroneSerial : SerialCommunication
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
        public DroneSerial(string portName, int baudRate) : base(portName, baudRate)
        {
            OnSerialDataReceived += ServerSerial_OnSerialDataReceived;
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
                    //Falta adicionar o tratamento de erro aqui
                    try
                    {
                        SendData(ManualControl.Encode(), 10);
                    }
                    catch (Exception ex)
                    {
                        StopSerialDataExchange();
                    }
                    //SendDataAsync(Telemetria.RawData);
                }
            });
            _threadSerialDataExchange.Start();
        }
        public void StopSerialDataExchange()
        {
            _stopThread = true;
            if (_threadSerialDataExchange != null)
                _threadSerialDataExchange.Join();
            Disconnect();
        }
        private void ServerSerial_OnSerialDataReceived(byte[] data)
        {
            try
            {
                object dataProtocol = RDCProtocol.Decode(data);
                if (dataProtocol != null)
                {
                    //Pacote de Manual Control
                    if (dataProtocol is RDCTelemetry)
                    {
                        Telemetria = (RDCTelemetry)dataProtocol;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
