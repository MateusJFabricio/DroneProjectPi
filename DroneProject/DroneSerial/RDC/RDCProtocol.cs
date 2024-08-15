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
        public static List<object> Decode(byte[] pacote)
        {
            List<object> returnList = new();
            if (pacote == null) return returnList;

            foreach(var pkt in SplitPackage(pacote, 0xFF)){
                object returnPkt = new();
                //Check do CRC
                if (!RDCPackage.CheckPacket(pkt))
                {
                    continue;
                }

                //Classifica pacote
                var dataType = RDCPackage.GetType(pkt);
                try{

                    //Classifica pacote
                    switch (dataType)
                    {
                        case 1: //Telemetria
                            returnList.Add(new RDCTelemetry(pkt));
                            break;
                        case 2: //Controle Manual do Drone
                            returnList.Add(new RDCManualControl(pkt));
                            break;
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
            }
            
            return returnList;
        }
        public static bool CRCCheck(byte[] data)
        {
            var crc = CRC8.ComputeChecksum(data);
            return crc == 0;
        }
        public static List<byte[]> SplitPackage(byte[] pacote, byte byteSplit){
            int index = 0;
            List<byte[]> list = new();

            while(index < pacote.Length){
                try{
                    if(byteSplit == pacote[index]){
                        if (index + 4 < pacote.Length)
                        {
                            int lenght = pacote[index + 1];
                            if (pacote.Length < index + 2 + lenght) break;
                            byte[] b = new byte[lenght + 2];
                            Array.Copy(pacote, index, b, 0, lenght + 2);
                            list.Add(b);
                            index += b.Length;
                        }
                        else
                        {
                            index++;
                        }
                    }else{
                        index++;
                    }

                }catch{

                }
            }

            return list;
        }
    }
}
