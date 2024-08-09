using System.IO.Ports;

namespace DroneProject.ModelContext.Serial
{
    public delegate void SerialDataReceived(byte[] data);
    public class SerialCommunication
    {
        public bool IsConnected
        {
            get
            {
                if (_serialPort != null)
                {
                    return _serialPort.IsOpen;
                }
                return false;
            }
        }
        public int BaudRate { get; set; }
        public string PortName { get; set; }
        SerialPort _serialPort = new SerialPort();
        public event SerialDataReceived OnSerialDataReceived;
        public bool LockDataSend = false;

        public SerialCommunication(string portName, int baudRate)
        {
            PortName = portName;
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
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.DataReceived -= SerialPort_DataReceived;
            }
        }
        public void SendData(byte[] data, int delay) 
        {
            if (LockDataSend) return;

            LockDataSend = true;
            try
            {
                if (_serialPort != null) 
                { 
                    if (IsConnected)
                    {
                        _serialPort.Write(data, 0, data.Length);
                    }
                    else
                    {
                        Connect();
                        Thread.Sleep(delay);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao enviar os dados seriais: " + ex.Message);
            }
            finally
            {
                LockDataSend = false;
            }
        }
        public async Task SendDataAsync(byte[] data)
        {
            if (LockDataSend) return;

            LockDataSend = true;
            try
            {

                if (_serialPort != null)
                {
                    if (IsConnected)
                    {
                        await Task.Run(() => _serialPort.Write(data, 0, data.Length));
                        await Task.Delay(1000);
                    }
                    else
                    {
                        Connect();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao enviar os dados seriais: " + ex.Message);
            }
            finally
            {
                LockDataSend = false;
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
