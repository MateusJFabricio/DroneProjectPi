using System;
using System.Collections.Generic;

namespace CRSF
{
    public static class CRSFProtocol
    {
        public static List<byte[]> SplitPackage(byte[] pacote, byte byteSplit){
            int index = 0;
            List<byte[]> list = new List<byte[]>();

            while(index < pacote.Length){
                
                if(byteSplit == 0xC8){
                    int lenght = pacote[index + 1];
                    if (pacote.Length < index + 2 + lenght) break;
                    byte[] b = new byte[lenght + 2];
                    Array.Copy(pacote, index, b, 0, lenght + 2);
                    list.Add(b);
                    index+=b.Length;
                }
            }

            return list;
        }
        public static List<object> Decode(byte[] pacote){
            var pktList = SplitPackage(pacote, 0XC8);
            List<object> returnList = new List<object>();

            foreach(var pkt in pktList){
                //Check o tamanho do pacote
                if (!CRSFPackage.CheckPacket(pkt))
                {
                    continue;
                }
                
                //Classifica pacote
                var dataType = CRSFPackage.GetType(pkt);
                try{
                    switch (dataType)
                    {
                        case 2: //CRSF_FRAMETYPE_GPS
                            returnList.Add(new CRSF_FRAMETYPE_GPS(pkt));
                            break;
                        case 7: //CRSF_FRAMETYPE_VARIO
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 8: //CRSF_FRAMETYPE_BATTERY_SENSOR
                            returnList.Add(new CRSF_FRAMETYPE_BATTERY_SENSOR(pkt));
                            break;
                        case 9: //CRSF_FRAMETYPE_BARO_ALTITUDE
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 11: //CRSF_FRAMETYPE_HEARTBEAT
                            //Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 20: //CRSF_FRAMETYPE_LINK_STATISTICS
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 22: //CRSF_FRAMETYPE_RC_CHANNELS_PACKED
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 23: //CRSF_FRAMETYPE_SUBSET_RC_CHANNELS_PACKED
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 28: //CRSF_FRAMETYPE_LINK_RX_ID
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 29: //CRSF_FRAMETYPE_LINK_TX_ID
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 30: //CRSF_FRAMETYPE_ATTITUDE
                            returnList.Add(new CRSF_FRAMETYPE_ATTITUDE(pkt));
                            break;
                        case 33: //CRSF_FRAMETYPE_FLIGHT_MODE
                            returnList.Add(new CRSF_FRAMETYPE_FLIGHT_MODE(pkt));
                            break;
                        case 40: //CRSF_FRAMETYPE_DEVICE_PING
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 41: //CRSF_FRAMETYPE_DEVICE_INFO
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 43: //CRSF_FRAMETYPE_PARAMETER_SETTINGS_ENTRY
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 44: //CRSF_FRAMETYPE_PARAMETER_READ
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 45: //CRSF_FRAMETYPE_PARAMETER_WRITE
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 46: //CRSF_FRAMETYPE_ELRS_STATUS
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 50: //CRSF_FRAMETYPE_COMMAND
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        case 58: //CRSF_FRAMETYPE_RADIO_ID
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                        default:
                            Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                            break;
                    }
                }catch{
                    Console.WriteLine("Pacote nao reconhecido de tipo: " +  dataType);
                }
            }
            return returnList;
            
        }
    }
}
