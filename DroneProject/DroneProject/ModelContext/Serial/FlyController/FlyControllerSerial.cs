﻿using RDC.CRSFProtocol;
using RDC.RDCProtocol;

namespace DroneProject.ModelContext.Serial.FlyController
{
    internal class FlyControllerSerial : SerialCommunication
    {
        public RDCTelemetria Telemetria { get; set; } = new RDCTelemetria();
        public RDCManualControl ManualControl { get; set; } = new RDCManualControl();
        public CRSFProtocol CRSF { get; set; } = new CRSFProtocol();
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

            //Set initial value for channels
            for (int i = 0; i < CRSF.Channels.Length; i++) 
            { 
                ManualControl.SetChannel(i, CRSF.Channels[i]);
            }
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
                    var crsfData = CRSF.BuildChannelPacket(ManualControl.Channels);

                    //Falta adicionar o tratamento de erro aqui
                    SendData(crsfData);
                }
            });
            _threadSerialDataExchange.Start();
        }
        public void StopSerialDataExchange() 
        {
            _stopThread = true;
        }
        private void FlyControllerSerial_OnSerialDataReceived(byte[] data)
        {
            object dataProtocol = RDCProtocol.Decode(data);
            if (dataProtocol != null)
            {
                //Pacote de telemetria
                if (dataProtocol is RDCTelemetria)
                {
                    Telemetria = (RDCTelemetria)dataProtocol;
                }
            }
        }
    }
}
