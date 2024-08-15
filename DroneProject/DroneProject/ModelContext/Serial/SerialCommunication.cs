using System.IO.Ports;

namespace DroneProject.ModelContext.Serial
{
    public delegate void SerialConnected();
    public delegate void SerialDisconnected();
    public class SerialCommunication
    {
        public bool IsConnected
        {
            get
            {
                if (_serialPort != null)
                {
                    return CheckConnection();
                }
                return false;
            }
        }
        private bool _isConnected = false;
        public int BaudRate { get; set; }
        public string PortName { get; set; }
        SerialPort _serialPort = new SerialPort();
        public event SerialConnected OnSerialConnected;
        public event SerialDisconnected OnSerialDisconnected;
        public bool LockDataSend = false;
        public bool NewDataAvailable { get => SerialDataReceived != null ? SerialDataReceived.Length > 0 : false; }
        public byte[] SerialDataReceived { get; set; }
        private DateTime _lastConnectionTime = DateTime.Now;
        private bool CheckConnection(){
            _isConnected = (DateTime.Now - _lastConnectionTime).TotalMilliseconds < 3000;
            return _serialPort.IsOpen;
            return _isConnected && _serialPort.IsOpen;
        }
        public void Connect(){
            Connect(PortName, BaudRate);
        }
        public void Connect(string portName, int baudRate)
        {
            if (_serialPort != null)
            {
                //Desconecta
                if (_serialPort.IsOpen)
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        _serialPort.Read(SerialDataReceived, 0, _serialPort.BytesToRead);
                    }
                }
                _serialPort.Close();
                _serialPort.DataReceived -= SerialPort_DataReceived;
            }else
            {
                _serialPort = new SerialPort();
            }

            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            
            try
            {
                _serialPort.Open();
                OnSerialConnected?.Invoke();
            }
            catch (Exception ex)
            {
                
            }

            _lastConnectionTime = DateTime.Now;
        }
        public void Disconnect()
        {
            if (_serialPort != null)
            {
                //Desconecta
                if (_serialPort.IsOpen){
                    if (_serialPort.BytesToRead > 0)
                    {
                        _serialPort.Read(SerialDataReceived, 0, _serialPort.BytesToRead);
                    }
                }
                _serialPort.Close();
                _serialPort.DataReceived -= SerialPort_DataReceived;
            }
            OnSerialDisconnected?.Invoke();
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
                    }

                    Thread.Sleep(delay);
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
            try{
                SerialPort sp = (SerialPort)sender;
                if (sp.BytesToRead > 20){
                    byte[] data = new byte[sp.BytesToRead];
                    sp.Read(data, 0, data.Length);
                    SerialDataReceived = data;
                }
            }catch(Exception ex){
                Console.WriteLine("Problema no recebimento de dados");
            }

            _lastConnectionTime = DateTime.Now;
        }
    }
}
