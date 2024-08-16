using View;
using System.Runtime.InteropServices;
using DroneProject.ModelContext.Database;
using DroneProject.ModelContext.Serial.FlyController;
using DroneProject.ModelContext.Serial.ServerSerial;
using DroneProject.Models;
using System.IO.Ports;
using RDC;
using RDC.RDCProtocol;
using CRSF;

namespace DroneProject
{
    public class Program
    {
        static ConsoleView ConsoleView = new();
        
        public static RDCManualControl RDCManualControl { get; set; } = new RDCManualControl();
        public static RDCTelemetry RDCTelemetry { get; set; } = new RDCTelemetry();
        public static CRSF_FRAMETYPE_GPS Gps { get; set; } = new CRSF_FRAMETYPE_GPS();
        public static CRSF_FRAMETYPE_ATTITUDE Orientation { get; set; } = new CRSF_FRAMETYPE_ATTITUDE();
        public static CRSF_FRAMETYPE_BATTERY_SENSOR Bateria { get; set; } = new CRSF_FRAMETYPE_BATTERY_SENSOR();

        public static byte[] FlyControllerSerialDataReceived { get; private set; }
        public static byte[] ServerSerialDataReceived { get; private set; }
        public static object ServerSerialLocker { get; private set; } = new object();
        public static object FlyControllerSerialLocker { get; private set; } = new object();

        public static SerialPort FlyControllerSerial;
        public static SerialPort ServerSerial;
        //Dados
        public static SerialModelContext SerialDataContext = new SerialModelContext();
        public static SerialModel FlyControllerModel;
        public static SerialModel ServerModel;


        static void Main(string[] args)
        {
            DateTime timeElapsed = DateTime.Now;
            FlyControllerModel = SerialDataContext.GetSerialByName("FLY_CONTROLLER");
            ServerModel = SerialDataContext.GetSerialByName("SERVER");

            Task _taskServer = new Task(() =>
            {
                Console.WriteLine("Inicializando task server...");
                try
                {
                    Task _taskServerSendData = new Task(async () =>
                    {
                        while (true)
                        {
                            ServerWriteData();
                            await Task.Delay(1000);
                        }
                    });

                    while (true)
                    {
                        try
                        {
                            ServerCheckConnection();

                            //if (_taskServerSendData.Status == TaskStatus.Created)
                            //{
                            //    _taskServerSendData. Start();
                            //}

                            ServerReadData();
                            Task.Delay(50);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro Task Server: " + ex.Message);
                        }
                    }
                }
                finally
                {
                    if (ServerSerial != null)
                        if (ServerSerial.IsOpen)
                            ServerSerial.Close();
                }
            });

            Task _taskFlyController = new Task(() =>
            {
                Console.WriteLine("Inicializando task flyController...");
                try
                {
                    while (true)
                    {
                        try
                        {
                            FlyControllerCheckConnection();
                            //FlyControllerReadData();
                            FlyControllerWriteData();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro Task FlyController: " + ex.Message);
                        }
                    }
                }
                finally
                {
                    if (FlyControllerSerial != null)
                        if (FlyControllerSerial.IsOpen)
                            FlyControllerSerial.Close();
                }
            });
            
            _taskFlyController.Start();
            _taskServer.Start();

            _taskFlyController.Wait();
            _taskServer.Wait();
        }
        private static void FlyControllerCheckConnection()
        {
            if (FlyControllerSerial == null) 
                FlyControllerSerial = new SerialPort(FlyControllerModel.PortCOM, FlyControllerModel.BaudRate);

            if (!FlyControllerSerial.IsOpen)
            {
                try
                {
                    FlyControllerSerial.Open();
                    Console.WriteLine("FlyController serial Open");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FlyController Serial Error: " + ex.Message);
                }
            }
        }

        private static void ServerCheckConnection()
        {
            if (ServerSerial == null)
                ServerSerial = new SerialPort(ServerModel.PortCOM, ServerModel.BaudRate);

            if (!ServerSerial.IsOpen)
            {
                try
                {
                    ServerSerial.Open();
                    Console.WriteLine("Server serial Open");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server Serial Error: " + ex.Message);
                }
            }
        }
        private static void FlyControllerWriteData()
        {
            lock(FlyControllerSerialLocker)
            {
                if (FlyControllerSerial.IsOpen)
                {
                    var manual = new CRSF_FRAMETYPE_RC_CHANNELS_PACKED(RDCManualControl.Channels);
                    var pkt = manual.Encode();

                    FlyControllerSerial.Write(pkt, 0, pkt.Length);
                    Console.WriteLine("A:" + manual.Channels[0] + ", E:" + manual.Channels[1] + ", R:" + manual.Channels[2] + ", T:" + manual.Channels[3]);
                }
            }
        }

        private static void ServerWriteData()
        {
            lock (ServerSerialLocker)
            {
                if (ServerSerial.IsOpen)
                {
                    var pkt = Gps.ParseToRDC();

                    ServerSerial.Write(pkt, 0, pkt.Length);
                }
            }
        }

        private static void FlyControllerReadData()
        {
            lock (FlyControllerSerialLocker)
            {
                if (FlyControllerSerial.IsOpen)
                {
                    if (FlyControllerSerial.BytesToRead > 10)
                    {
                        byte[] data = new byte[FlyControllerSerial.BytesToRead];
                        FlyControllerSerial.Read(data, 0, data.Length);

                        foreach (var pkt in CRSFProtocol.Decode(data))
                        {
                            if (pkt is CRSF_FRAMETYPE_GPS)
                            {
                                Gps = (CRSF_FRAMETYPE_GPS)pkt;
                            }
                        }
                    }
                }
            }
        }

        private static void ServerReadData()
        {
            lock (ServerSerialLocker)
            {
                if (ServerSerial.IsOpen)
                {
                    if (ServerSerial.BytesToRead > 10)
                    {
                        byte[] data = new byte[ServerSerial.BytesToRead];
                        ServerSerial.Read(data, 0, data.Length);

                        foreach (var pkt in RDCProtocol.Decode(data))
                        {
                            if (pkt is RDCManualControl)
                            {
                                RDCManualControl = (RDCManualControl)pkt;
                            }
                        }
                    }
                }
            }
        }
    }
}