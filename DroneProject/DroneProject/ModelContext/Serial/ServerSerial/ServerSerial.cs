using RDC.RDCProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        public ServerSerial(string portName, int baudRate) : base(portName, baudRate)
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
                    SendData(Telemetria.Encode(), 1000);
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
        }
        private void ServerSerial_OnSerialDataReceived(byte[] data)
        {
            try
            {
                object dataProtocol = RDCProtocol.Decode(data);
                if (dataProtocol != null)
                {
                    //Pacote de Manual Control
                    if (dataProtocol is RDCManualControl)
                    {
                        ManualControl = (RDCManualControl)dataProtocol;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
