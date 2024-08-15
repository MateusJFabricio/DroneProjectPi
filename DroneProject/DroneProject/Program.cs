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

        static void Main(string[] args)
        {
            DateTime timeElapsed = DateTime.Now;
            //Dados
            SerialModelContext SerialDataContext = new SerialModelContext();
            SerialModel FlyControllerModel = SerialDataContext.GetSerialByName("FLY_CONTROLLER");
            SerialModel ServerModel = SerialDataContext.GetSerialByName("SERVER");

            //Comunicacao
            Task threadDataExchanger = new(()=>{
                //Inicializa a conexao
                SerialPort serverSerialPort = new SerialPort(ServerModel.PortCOM, ServerModel.BaudRate);
                SerialPort flyControlllerSerialPort = new SerialPort(FlyControllerModel.PortCOM, FlyControllerModel.BaudRate);

                try{
                    while(true){
                        //Server serial Conection
                        try{
                            if (!serverSerialPort.IsOpen)
                            {
                                serverSerialPort.Open();
                                serverSerialPort.DataReceived += ServerSerialPort_DataReceived;
                            }
                            else
                            {
                                if (serverSerialPort.BytesToRead >= serverSerialPort.ReadBufferSize - 1000)
                                {
                                    ServerSerialPort_DataReceived(serverSerialPort, null);
                                }
                            }
                        }catch(Exception ex){
                            Console.WriteLine(ex.Message);
                        }

                        //FlyController serial Conection
                        try
                        {
                            if (!flyControlllerSerialPort.IsOpen)
                            {
                                flyControlllerSerialPort.Open();
                                flyControlllerSerialPort.DataReceived += FlyControllerSerialPort_DataReceived;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        //DataExchange Server
                        //try
                        //{
                        //    if (serverSerialPort.IsOpen){
                        //        
                        //        
                        //    }
                        //}catch(Exception ex){
                        //
                        //}

                        //DataExchange FlyController
                        try
                        {
                            if (flyControlllerSerialPort.IsOpen)
                            {
                                var manual = new CRSF_FRAMETYPE_RC_CHANNELS_PACKED(RDCManualControl.Channels);
                                var pkt = manual.Encode();
                                flyControlllerSerialPort.Write(pkt, 0, pkt.Length);

                                Console.WriteLine("Time(ms): " + (DateTime.Now - timeElapsed).TotalMilliseconds);
                                timeElapsed = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }catch(Exception ex){
                    Console.WriteLine("Erro:" + ex.Message);
                }finally{
                    serverSerialPort.Close();
                    flyControlllerSerialPort.Close();
                }
            });

            threadDataExchanger.Start();

            threadDataExchanger.Wait();
        }
        private static void ServerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try{
                SerialPort sp = (SerialPort)sender;
                if (sp.BytesToRead > 20){
                    byte[] data = new byte[sp.BytesToRead];
                    sp.Read(data, 0, data.Length);

                    try
                    {
                        foreach (var pkt in RDCProtocol.Decode(data))
                        {
                            if (pkt is RDCManualControl)
                            {
                                RDCManualControl = (RDCManualControl)pkt;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }catch(Exception ex){
                Console.WriteLine("Problema no recebimento de dados");
            }
        }

        private static void FlyControllerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                if (sp.BytesToRead > 20)
                {
                    byte[] data = new byte[sp.BytesToRead];
                    sp.Read(data, 0, data.Length);

                    try
                    {
                        foreach (var pkt in CRSFProtocol.Decode(data))
                        {
                            if (pkt is CRSF_FRAMETYPE_GPS)
                            {
                                var gps = (CRSF_FRAMETYPE_GPS)pkt;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problema no recebimento de dados");
            }
        }
    }
}