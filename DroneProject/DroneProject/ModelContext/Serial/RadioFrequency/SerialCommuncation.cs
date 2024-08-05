using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Device.Gpio;

namespace DroneProject.ModelContext.Serial.RadioFrequency
{
    public delegate void SerialDataReceived(byte[] data);
    public class SerialCommuncation
    {
        public int TxPin { get; set; }
        public int RxPin { get; set; }
        public int BaudRate { get; set; }
        public string PortName { get; set; }
        bool _continue;
        SerialPort _serialPort;
        public event SerialDataReceived OnSerialDataReceived;

        public SerialCommuncation(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
        }

        public SerialCommuncation(int txPin, int rxPin, int baudRate)
        {
            var controller = new GpioController();
            TxPin = txPin;
            RxPin = rxPin;
            BaudRate = baudRate;
        }

        public void Connect()
        {
            Disconnect();
            _serialPort = new SerialPort(PortName, BaudRate);
            try
            {
                _serialPort.Open();
                _serialPort.DataReceived += SerialPort_DataReceived;
            }
            catch (Exception ex)
            {
                
            }
        }
        public void Disconnect() 
        {
            if (_serialPort != null) { 
                _serialPort.Close();
                _serialPort.DataReceived -= SerialPort_DataReceived;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] data = new byte[sp.BytesToRead];
            sp.Read(data, 0, data.Length);
            OnSerialDataReceived?.Invoke(data);
        }
    }
}
