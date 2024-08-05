using RDC.Utils;

namespace RDC.RDCProtocol
{
    public static class RDCProtocol
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
            int dataType = 0;
            //Check o tamanho do pacote
            if (pacote.Length < MIN_LENGTH) 
            {
                throw new ArgumentException("Data check nok");
            }

            //Check do CRC
            if (!CRCCheck(pacote))
            {
                throw new Exception("Data check CRC nok");
            }

            //Check cabeçalho
            if ((pacote[0] != 0xF)|| (pacote[1] != 0xF))
            {
                throw new Exception("Data check header nok");
            }

            //Check Tipo de pacote
            if ((pacote[3] < 1)&& (pacote[3] > 6))
            {
                throw new Exception("Data check packge type nok");
            }

            short length = BitConverter.ToInt16(pacote, 4);
            byte[] data = pacote.Take(new Range(6, pacote.Length - length - 1)).ToArray();
            //Classifica pacote
            switch (dataType)
            {
                case 1: //Telemetria
                    return new RDCTelemetria(data);
                case 2: //Controle Manual do Drone
                    return new RDCManualControl(data);
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
        }

        public static byte[] Encode(object data, int type)
        {
            if (data == null) throw new Exception("The data argument cannot be null");
            byte[] dataEncoded;
            switch (type)
            {
                case 1: //Telemetria
                    dataEncoded = new byte[RDCTelemetria.DATA_LENGTH];
                    Array.Copy(((RDCTelemetria)data).RawData, dataEncoded, ((RDCTelemetria)data).RawData.Length);
                    break;
                case 2: //Controle Manual do Drone
                    dataEncoded = new byte[RDCManualControl.DATA_LENGTH];
                    Array.Copy(((RDCManualControl)data).RawData, dataEncoded, ((RDCManualControl)data).RawData.Length);
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
            Int16 dataLenght = Convert.ToInt16(dataEncoded.Length);
            byte[] pacote = new byte[6 + dataLenght];

            //Header
            pacote[0] = 0xF;
            pacote[1] = 0xF;

            //Tipo
            pacote[3] = (byte) type;

            //Tamanho
            Array.Copy(BitConverter.GetBytes(dataLenght), 0, pacote, 4, 2);

            //Dados
            Array.Copy(dataEncoded, 0, pacote, 6, dataLenght);

            //CRC
            pacote[pacote.Length] = CRC8.ComputeChecksum(pacote);

            return pacote;
        }

        public static bool CRCCheck(byte[] data)
        {
            var crc = CRC8.ComputeChecksum(data);
            return crc == 0;
        }
    }
}
