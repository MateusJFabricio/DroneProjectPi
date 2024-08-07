using DroneProject.ModelContext.Database;
using DroneProject.ModelContext.Serial.FlyController;
using DroneProject.ModelContext.Serial.ServerSerial;
using DroneProject.Models;

namespace DroneProject
{
    public class Program
    {
        static int Main(string[] args)
        {
            //Database path
            DatabaseConnection.ConnectionString = "Data Source=Drone.db";

            //Buscar dados e parametros
            ParameterModelContext parameter = new ParameterModelContext();
            var droneNomeParam = parameter.GetParameterByKey("DRONE_NOME");
            Console.WriteLine("Bem vindo ao " + droneNomeParam.Value);
            Console.WriteLine("Inicializando...");
            Console.WriteLine();

            //Log de erro context
            ErrorLogModelContext errorLogModel = new ErrorLogModelContext();

            //SQLITE
            Console.WriteLine("Buscando os dados da comunicação serial:");
            SerialModelContext serialDataContext = new SerialModelContext();
            SerialModel serialFlyController = new SerialModel();
            SerialModel serialServer = new SerialModel();
            SerialModel serialKinect = new SerialModel();
            try
            {
                Console.WriteLine("1: " + "Serial FLY_CONTROLLER");
                serialFlyController = serialDataContext.GetSerialByName("FLY_CONTROLLER");
                Console.WriteLine("     BaudRate: " + serialFlyController.BaudRate.ToString());
                Console.WriteLine("     Port: " + serialFlyController.PortCOM);

                Console.WriteLine("2: " + "Serial SERVER");
                serialServer = serialDataContext.GetSerialByName("SERVER");
                Console.WriteLine("     BaudRate: " + serialServer.BaudRate.ToString());
                Console.WriteLine("     Port: " + serialServer.PortCOM);

                //var serialKinect = serialDataContext.GetSerialByName("KINECT");

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                errorLogModel.Insert(new ErrorLogModel { Tipo = "Erro", Origem = "MAIN", Descricao = ex.Message });
            }

            Console.WriteLine();
            Console.WriteLine("Inicializando as communicações seriais");
            //Estabelecer as conexoes seriais
            Console.WriteLine("1: FlyController Serial");
            FlyControllerSerial FlyControllerSerial = new(serialFlyController.PortCOM, serialFlyController.BaudRate); //416666
            Console.WriteLine("     Sucesso");

            Console.WriteLine("2: Server Serial");
            ServerSerial ServerSerial = new(serialServer.PortCOM, serialServer.BaudRate); //115200
            Console.WriteLine("     Sucesso");

            //SerialCommunication Kinect = new SerialCommunication("", 9600);

            while (true) 
            {
                try
                {
                    //Check Connection

                    //Data exchange Fly Controller
                    //if (!FlyControllerSerial.SeriaDataExchangeRunning)
                    //{
                    //    FlyControllerSerial.StartSerialDataExchange();
                    //}

                    //Data exchange Server
                    if (!ServerSerial.SeriaDataExchangeRunning)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Tentando a comunicação com o Server...");
                        Console.WriteLine(" MENU: ");
                        Console.WriteLine(" 1 - Atualizar dados de acesso");
                        Console.WriteLine(" 2 - Visualizar dados");
                        while (!ServerSerial.IsConnected)
                        {
                            if (Console.KeyAvailable)
                            {
                                var keyValue = Console.ReadKey();
                                if (keyValue.KeyChar == '1')
                                {
                                    try
                                    {
                                        int codRetorno = 0;
                                        Console.WriteLine("Atualizando os dados de acesso:");
                                        Console.WriteLine("Digite a nova porta COM de acesso: ");
                                        var novaPortaAcesso = Console.ReadLine();
                                        if (novaPortaAcesso != null)
                                        {
                                            serialServer.PortCOM = novaPortaAcesso;
                                            Console.WriteLine("Digite o BAUDRATE de acesso: ");
                                            var newBaudRate = Console.ReadLine();
                                            if (newBaudRate != null)
                                            {
                                                serialServer.BaudRate = Convert.ToInt32(newBaudRate);
                                                serialDataContext.Update(serialServer); //Atualiza os dados no DB
                                                codRetorno = 1;
                                            }
                                        }

                                        if (codRetorno == 0)
                                            Console.WriteLine("Operação concluida sem exito");
                                        else Console.WriteLine("Concluido");
                                    }
                                    catch (Exception ex)
                                    {

                                        Console.WriteLine("Operação cancelada por erro! Erro:" + ex.Message);
                                    }

                                }
                                else if (keyValue.KeyChar == '2')
                                {
                                    Console.WriteLine("Listando os dados: ");
                                    Console.WriteLine("Porta:" + ServerSerial.PortName);
                                    Console.WriteLine("BaudRate:" + ServerSerial.BaudRate.ToString());
                                }
                                
                            }
                            ServerSerial.Connect();
                        }

                        if (ServerSerial.IsConnected)
                        {
                            ServerSerial.StartSerialDataExchange();
                            Console.WriteLine("     Sucesso");
                        }
                        else
                        {
                            Console.WriteLine("     Não foi possivel se conectar com o server.");
                            return 0;
                        }                        
                    }

                    //FlyControllerSerial.ManualControl = ServerSerial.ManualControl;
                    ServerSerial.Telemetria = FlyControllerSerial.Telemetria;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                    //Data Log
                }
            }
        }

    }
}