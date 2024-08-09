using System.Runtime.InteropServices;
using DroneProject.ModelContext.Database;
using DroneProject.ModelContext.Serial.FlyController;
using DroneProject.ModelContext.Serial.ServerSerial;
using DroneProject.Models;

namespace View{
    public class Application{
        private Thread _threadDataExchanger;
        public FlyControllerSerial FlyControllerSerial { get; set; }
        public ServerSerial ServerSerial { get; set; }
        public SerialModelContext SerialDataContext { get; set; } = new SerialModelContext();
        public SerialModel FlyControllerModel { get; set; } = new SerialModel();
        public SerialModel ServerModel { get; set; } = new SerialModel();
        public void StartDataExchanger(){
            if (_threadDataExchanger == null){
                _threadDataExchanger = new Thread(()=>{
                    if (FlyControllerSerial.IsConnected){
                        if (ServerSerial.IsConnected){

                            //Fly Controller to Server
                            ServerSerial.Telemetria.Battery = FlyControllerSerial.Battery;
                            ServerSerial.Telemetria.Gps = FlyControllerSerial.Gps;
                            ServerSerial.Telemetria.Orientation = FlyControllerSerial.Orientation;
                            ServerSerial.Telemetria.FlighMode = FlyControllerSerial.FlighMode;

                            //Server to Fly Controller
                            FlyControllerSerial.ManualControl.Channels = FlyControllerSerial.ManualControl.Channels;

                            Thread.Sleep(1000); //A cada 1 seg
                        }
                    }
                });
            }
        }
        public void StartSerial(){
            StartFlyControllerSerial();
            StartServerSerial();
            if (FlyControllerSerial.IsConnected && ServerSerial.IsConnected){
                StartDataExchanger();
            }
        }
        public void StartFlyControllerSerial(){
            FlyControllerModel = SerialDataContext.GetSerialByName("FLY_CONTROLLER");
            if (FlyControllerSerial == null){
                FlyControllerSerial = new(FlyControllerModel.PortCOM, FlyControllerModel.BaudRate); //416666
            }else
            {
                FlyControllerSerial.StopSerialDataExchange();
                FlyControllerSerial.Disconnect();
            }

            FlyControllerSerial.Connect();
            FlyControllerSerial.StartSerialDataExchange();
        }
        public void StartServerSerial(){
            ServerModel = SerialDataContext.GetSerialByName("SERVER");
            Console.WriteLine($"Server - {ServerModel.PortCOM} - {ServerModel.BaudRate}");
            if (ServerSerial == null){
                ServerSerial = new(ServerModel.PortCOM, ServerModel.BaudRate); //416666
            }else
            {
                ServerSerial.StopSerialDataExchange();
                ServerSerial.Disconnect();
            }

            ServerSerial.Connect();
            ServerSerial.StartSerialDataExchange();
        }

        public void StopSerial(){
            if (FlyControllerSerial != null)
            {
                FlyControllerSerial.StopSerialDataExchange();
            }

            if (ServerSerial != null)
            {
                ServerSerial.StopSerialDataExchange();
            }          

        }

    }
}