using RDC.Utils;

namespace RDC.RDCProtocol
{
    public class RDCProtocol
    {
        #region description
        /*
         * Protocolo:
				Byte 1 e 2
					Nome: Inicio do pacote
					Valor fixo: hexadecimal FF
				Byte 3
					Nome: Tipo de pacote:
						1 - Telemetria
						2 - Controle Manual do Drone
						3 - Controle Automatico do Drone
						4 - Rota
						5 - Processamento de dados
						6 - Geral
				Byte 4 e 5
					Nome: Tamanho do pacote a partir desse pacote até o CRC
						Tamanho maximo de 65535 bytes
				Byte 6 ... n
					Nome: Pacote de acordo com o cabeçalho
				Byte n+1
					Nome: CRC-8
         */
        #endregion
        public const int MIN_LENGTH = 7;
        public static object Decode(byte[] pacote)
        {
            object returnPkt = new();
            //Check do CRC
            if (!RDCPackage.CheckPacket(pacote))
            {
                throw new Exception("Data check nok");
            }

            //Classifica pacote
            var dataType = RDCPackage.GetType(pacote);
            try{

                //Classifica pacote
                switch (dataType)
                {
                    case 1: //Telemetria
                        return new RDCTelemetry(pacote);
                    case 2: //Controle Manual do Drone
                        return new RDCManualControl(pacote);
                    case 3: //Controle Automatico do Drone
                        throw new Exception("Tipo de dado não implementado");
                    case 4: //Rota de trabalho
                        throw new Exception("Tipo de dado não implementado");
                    case 5: //Dados de imagem
                        throw new Exception("Tipo de dado não implementado");
                    case 6: //Geral
                        throw new Exception("Tipo de dado não implementado");
                    default:
                        throw new Exception("Tipo de dado não implementado");
                }
            }catch{
                Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
            }
            return returnPkt;
        }
        public static bool CRCCheck(byte[] data)
        {
            var crc = CRC8.ComputeChecksum(data);
            return crc == 0;
        }
    }
}
