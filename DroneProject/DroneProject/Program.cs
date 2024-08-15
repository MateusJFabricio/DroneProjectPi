using View;
using System.Runtime.InteropServices;
using DroneProject.ModelContext.Database;
using DroneProject.ModelContext.Serial.FlyController;
using DroneProject.ModelContext.Serial.ServerSerial;
using DroneProject.Models;
using System.IO.Ports;

namespace DroneProject
{
    public class Program
    {
        static ConsoleView ConsoleView = new();
        public static byte[] SerialDataReceived { get; set; }
        static void Main(string[] args)
        {
            //Dados
            SerialModelContext SerialDataContext = new SerialModelContext();
            SerialModel FlyControllerModel = SerialDataContext.GetSerialByName("FLY_CONTROLLER");
            SerialModel ServerModel = SerialDataContext.GetSerialByName("SERVER");

            //Comunicacao
            Task threadDataExchanger = new(()=>{
                //Inicializa a conexao
                SerialPort serverSerialPort = new SerialPort(ServerModel.PortCOM, ServerModel.BaudRate);
                SerialPort flyControlllerSerialPort = new SerialPort(ServerModel.PortCOM, ServerModel.BaudRate);

                try{
                    while(true){
                        //Server serial Conection
                        try{
                            if(!serverSerialPort.IsOpen){
                                serverSerialPort.Open();
                                serverSerialPort.DataReceived += ServerSerialPort_DataReceived;
                            }
                        }catch(Exception ex){
                            Console.WriteLine(ex.Message);
                        }

                        //DataExchange
                        try{
                            if (serverSerialPort.IsOpen && SerialDataReceived != null){
                                Console.WriteLine("New: " + string.Join("-", SerialDataReceived));
                            }
                        }catch(Exception ex){

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
                    SerialDataReceived = data;
                }
            }catch(Exception ex){
                Console.WriteLine("Problema no recebimento de dados");
            }
        }  
    }
}