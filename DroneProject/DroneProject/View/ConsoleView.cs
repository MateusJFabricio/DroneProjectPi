using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using DroneProject.ModelContext.Database;

namespace View
{
    public class ConsoleView
    {
        Thread _thread;
        Application App = new();
        public void Start(bool interact){
            Console.Write("Inicializando ConsoleView");
            if (interact){
                _thread = new Thread(Run);
                _thread.Start();
            }
            else{
                Inicializar();
            }
        }
        public void Run(){
            int menu = -1;
            while(menu!=9){
                menu = ShowMenuPrincipal();
                switch (menu)
                {
                    case 1:
                        Inicializar();
                        break;
                    case 2:
                        VisualizarParametros();
                        break;
                    case 3:
                        VisualizarSeriais();
                        break;
                    case 4:
                        VisualizarLogs();
                        break;
                    case 5:
                        VisualizarMenuConfiguracao();
                        break;
                    case 6:
                        VisualizarStatus();
                        break;
                    case 7:
                        InicializarFlyControllerSerial();
                        break;
                    case 8:
                        InicializarServerSerial();
                        break;
                    case 9:
                        StopProcess();
                        break;
                }
            }
        }
        public void StopProcess(){
            Console.WriteLine("");
            Console.WriteLine("PARANDO OS PROCESSOS");
            App.StopSerial();
        }
        public void VisualizarStatus(){
            Console.WriteLine("");
            Console.WriteLine("STATUS");

            //Fly controller
            Console.WriteLine(" 1 - Status do FlyController");
            if(App.FlyControllerSerial != null){
                Console.WriteLine("     Conexao Serial=" + (App.FlyControllerSerial.IsConnected ? "ON" : "OFF"));
                Console.WriteLine("     Troca de dados Serial=" + (App.FlyControllerSerial.SeriaDataExchangeRunning ? "ON" : "OFF"));
            }
            else
            {
                Console.WriteLine("     Conexao Serial=OFF");
                Console.WriteLine("     Troca de dados Serial=OFF");
            }

            //Server
            Console.WriteLine(" 2 - Status do Server");
            if(App.ServerSerial != null){
                Console.WriteLine("     Conexao Serial=" + (App.ServerSerial.IsConnected ? "ON" : "OFF"));
                Console.WriteLine("     Troca de dados Serial=" + (App.ServerSerial.SeriaDataExchangeRunning ? "ON" : "OFF"));
            }
            else
            {
                Console.WriteLine("     Conexao Serial=OFF");
                Console.WriteLine("     Troca de dados Serial=OFF");
            }

            //Status controle
            Console.WriteLine(" 3 - Status do Controle");
            if(App.FlyControllerSerial != null){
                var channels = App.FlyControllerSerial.ManualControl.Channels;
                for (int i = 0; i < channels.Length; i++)
                {
                    Console.WriteLine($"    CH{i + 1} - Valor: {channels[i].ToString()}");
                }
            }
            else
            {
                Console.WriteLine("     Manual Control=OFF");
            }

            //Status GPS
            Console.WriteLine(" 4 - Posição GPS");
            if(App.FlyControllerSerial != null && App.FlyControllerSerial.Gps != null){
                var gps = App.FlyControllerSerial.Gps;
                Console.WriteLine($"    Latitude: {gps.Latitude} - Logitude: {gps.Longitude} - Altitude: {gps.Altitude}");
                 Console.WriteLine($"   Velocidade: {gps.Speed} - Satelites: {gps.SatelliteCount} - Course: {gps.Course}");
            }else
            {
                Console.WriteLine("     GPS não encontrado");
            }

            //Status Bateria
            Console.WriteLine(" 5 - Status Bateria");
            if(App.FlyControllerSerial != null && App.FlyControllerSerial.Battery != null){
                var bateria = App.FlyControllerSerial.Battery;
                Console.WriteLine($"    Tensão: {bateria.Voltage} - Corrente: {bateria.Current}");
                Console.WriteLine($"    Porcentagem: {bateria.Percentage} - Capacidade usada: {bateria.UsedCapacity}");
            }else
            {
                Console.WriteLine("     Bateria não encontrada");
            }

            Console.WriteLine(" 6 - Orientacao");
            if (App.FlyControllerSerial != null && App.FlyControllerSerial.Orientation != null)
            {
                var orientation = App.FlyControllerSerial.Orientation;
                Console.WriteLine($"    YAW: {orientation.Yaw} - ROW: {orientation.Row}");
                Console.WriteLine($"    PITCH: {orientation.Pitch}");
            }
            else
            {
                Console.WriteLine("     Bateria não encontrada");
            }
        }
        public int ShowMenuPrincipal(){
            while(true){
                Console.WriteLine("");
                Console.WriteLine("Menu:");
                Console.WriteLine(" 1 - Inicializacao");
                Console.WriteLine(" 2 - Visualizar Parametros");
                Console.WriteLine(" 3 - Visualizar Seriais");
                Console.WriteLine(" 4 - Visualizar Logs");
                Console.WriteLine(" 5 - Configurações");
                Console.WriteLine(" 6 - Status");
                Console.WriteLine(" 7 - FlyController Start Serial");
                Console.WriteLine(" 8 - Server Start Serial");
                Console.WriteLine(" 9 - Fechar app");

                int key = -1;
                if (int.TryParse(Console.ReadLine().ToString(), out key)){
                    return key;
                }else{
                    Console.WriteLine("Valor invalido");
                    continue;
                }
            }
        }
        public void VisualizarParametros(){
            Console.WriteLine("");
            Console.WriteLine("PARAMETROS");
            ParameterModelContext parameters = new();
            parameters.ParameterlList.ForEach((x)=>{
                Console.WriteLine($"    ID:{x.Id} - Key:{x.Key} - Value:{x.Value}");
            });
        }
        public void VisualizarSeriais(){
            Console.WriteLine("");
            Console.WriteLine("SERIAIS");
            SerialModelContext serial = new();
            serial.SerialList.ForEach((x)=>{
                Console.WriteLine($"    ID:{x.Id} - Nome:{x.Nome} - BaudRate:{x.BaudRate} - Port:{x.PortCOM}");
            });
        }
        public void VisualizarLogs(){
            Console.WriteLine("");
            Console.WriteLine("LOGS");
            ErrorLogModelContext log = new();
            log.ErrorLogList.ForEach((x)=>{
                Console.WriteLine($"    ID:{x.Id} - Origem:{x.Origem} - Tipo:{x.Tipo} - Descricao:{x.Descricao.Substring(0, 40)}");
            });
        }
        public void VisualizarMenuConfiguracao(){
            int key = -1;

            while (key!=3)
            {
                Console.WriteLine("");
                Console.WriteLine("Menu Configuração:");
                Console.WriteLine(" 1 - Parametros");
                Console.WriteLine(" 2 - Serial");
                Console.WriteLine(" 3 - Voltar");

                if (int.TryParse(Console.ReadLine().ToString(), out key))
                {
                    if (key == 1){
                        while(true)
                        {
                            VisualizarParametros();
                            Console.WriteLine("");
                            Console.WriteLine("Escolha o ID do registro para ser atualizado: ");
                            if (int.TryParse(Console.ReadLine().ToString(), out key))
                            {
                                ParameterModelContext parameters = new();
                                var p = parameters.ParameterlList.FirstOrDefault((x)=>x.Id==key);
                                if (p != null){
                                    try{
                                        Console.WriteLine(" Digite o novo valor para o campo KEY: ");
                                        p.Key = Console.ReadLine().ToString();
                                        Console.WriteLine(" Digite o novo valor para o campo VALUE: ");
                                        p.Value = Console.ReadLine().ToString();
                                        parameters.Update(p);
                                        Console.WriteLine(" Registro atualizado");
                                        return;
                                    }catch{
                                        Console.WriteLine(" Finalizado com erro. Tente novamente");
                                    }
                                }else{
                                   Console.WriteLine(" Valor de ID não consta no BD. Tente novamente"); 
                                }
                            }else
                                Console.WriteLine(" Valor invalido");
                        }
                    }else if(key==2){
                        while(true)
                        {
                            VisualizarSeriais();
                            Console.WriteLine("");
                            Console.WriteLine("Escolha o ID do registro para ser atualizado: ");
                            if (int.TryParse(Console.ReadLine().ToString(), out key))
                            {
                                SerialModelContext seriais = new();
                                var p = seriais.SerialList.FirstOrDefault((x)=>x.Id==key);
                                if (p != null){
                                    try{
                                        Console.WriteLine(" Digite o novo valor para o campo NOME: ");
                                        p.Nome = Console.ReadLine();
                                        Console.WriteLine(" Digite o novo valor para o campo BAUD_RATE: ");
                                        p.BaudRate = Convert.ToInt32(Console.ReadLine());
                                        Console.WriteLine(" Digite o novo valor para o campo PORT_COM: ");
                                        p.PortCOM = Console.ReadLine();
                                        seriais.Update(p);
                                        Console.WriteLine(" Registro atualizado");
                                        return;
                                    }catch{
                                        Console.WriteLine(" Finalizado com erro. Tente novamente");
                                    }
                                    
                                }else{
                                   Console.WriteLine(" Valor de ID não consta no BD. Tente novamente"); 
                                }
                            }else
                                Console.WriteLine(" Valor invalido");
                        }
                    }
                    
                }else{
                    Console.WriteLine("Valor invalido");
                    continue;
                }
            }
        }
        public void Inicializar(){
            Console.WriteLine("Inicializando...");
            Console.WriteLine("");
            try
            {
                Console.WriteLine("Buscando o banco de dados");
                //Database path
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                    DatabaseConnection.ConnectionString = "Data Source=/home/Robosoft/Desktop/GIT/DronePIProject/DroneProject/Drone.db";
                }else{
                    DatabaseConnection.ConnectionString = "Data Source=Drone.db";
                }
                
                Console.WriteLine("Carregando os parametros");
                //Buscar dados e parametros
                ParameterModelContext parameter = new ParameterModelContext();
                var droneNomeParam = parameter.GetParameterByKey("DRONE_NOME");
                Console.WriteLine("Bem vindo ao " + droneNomeParam.Value);
                Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine("Inicializando as comunicações seriais");
                App.StartSerial();

                //Status Bateria
                Console.WriteLine(" 5 - Status Bateria");
                if(App.FlyControllerSerial != null && App.FlyControllerSerial.Battery != null){
                    var bateria = App.FlyControllerSerial.Battery;
                    Console.WriteLine($"    Tensão: {bateria.Voltage} - Corrente: {bateria.Current}");
                    Console.WriteLine($"    Porcentagem: {bateria.Percentage} - Capacidade usada: {bateria.UsedCapacity}");
                }else
                {
                    Console.WriteLine("     Bateria não encontrada");
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine( ex.Message);
            }
        }
        public void InicializarFlyControllerSerial()
        {
            Console.WriteLine("Inicializando FlyController Serial...");
            Console.WriteLine("");
            try
            {
                Console.WriteLine("Buscando o banco de dados");
                //Database path
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    DatabaseConnection.ConnectionString = "Data Source=/home/Robosoft/Desktop/GIT/DronePIProject/DroneProject/Drone.db";
                }
                else
                {
                    DatabaseConnection.ConnectionString = "Data Source=Drone.db";
                }

                App.StartFlyControllerSerial();

                if (App.FlyControllerSerial.IsConnected)
                    Console.WriteLine("Conectado");
                else
                {
                    App.FlyControllerSerial.StopSerialDataExchange();
                    Console.WriteLine("Não foi possivel conectar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }
        public void InicializarServerSerial()
        {
            Console.WriteLine("Inicializando Server Serial...");
            Console.WriteLine("");
            try
            {
                Console.WriteLine("Buscando o banco de dados");
                //Database path
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    DatabaseConnection.ConnectionString = "Data Source=/home/Robosoft/Desktop/GIT/DronePIProject/DroneProject/Drone.db";
                }
                else
                {
                    DatabaseConnection.ConnectionString = "Data Source=Drone.db";
                }

                App.StartServerSerial();

                if (App.ServerSerial.IsConnected)
                    Console.WriteLine("Conectado");
                else
                {
                    App.ServerSerial.StopSerialDataExchange();
                    Console.WriteLine("Não foi possivel conectar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }
    }
}