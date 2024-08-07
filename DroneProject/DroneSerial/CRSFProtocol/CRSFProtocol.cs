using RDC.Utils;

namespace RDC.CRSFProtocol
{
    public class CRSFProtocol
    {
        private const int CHANNEL_MIN_VALUE = 0;
        private const int CHANNEL_MAX_VALUE = 1984;
        private ushort[] channels = new ushort[16];
        public ushort[] Channels { get => GetChannels(); set => SetChannels(value); }
        public CRSFProtocol()
        {
            channels[0] = 992;
            channels[1] = 992;
            channels[2] = 992;
            channels[3] = 992;
            channels[4] = 992;
            channels[5] = 992;
            channels[6] = 992;
            channels[7] = 992;
            channels[8] = 992;
            channels[9] = 992;
            channels[10] = 992;
            channels[11] = 992;
            channels[12] = 992;
            channels[13] = 992;
            channels[14] = 992;
            channels[15] = 992;
        }
        public CRSFProtocol(ushort[] startChannelValues)
        {
            for (int i = 0; i < channels.Length; i++) 
            {
                channels[i] = startChannelValues[i];
            }
        }
        private void SetChannels(ushort[] channels)
        {
            try
            {
                this.channels = channels;
                for (int i = 0; i < channels.Length; i++)
                {
                    if (channels[i] < CHANNEL_MIN_VALUE)
                    {
                        channels[i] = CHANNEL_MIN_VALUE;
                    }

                    if (channels[i] > CHANNEL_MAX_VALUE)
                    {
                        channels[i] = CHANNEL_MAX_VALUE;
                    }
                }
            }
            catch
            {
                throw new Exception("Formato invalido para os canais");
            }
        }
        private ushort[] GetChannels() 
        {
            for (int i = 0; i < channels.Length; i++)
            {
                if (channels[i] < CHANNEL_MIN_VALUE)
                {
                    channels[i] = CHANNEL_MIN_VALUE;
                }

                if (channels[i] > CHANNEL_MAX_VALUE)
                {
                    channels[i] = CHANNEL_MAX_VALUE;
                }
            }
            return channels;
        }
        public byte[] BuildChannelPacket()
        {
            return BuildChannelPacket(Channels);
        }
        public byte[] BuildChannelPacket(ushort[] channels)
        {
            byte[] message = new byte[26]; // Sync (1 byte) + tipo de mensagem (1 byte) + dados (22 bytes) + checksum (1 byte)

            //Cabeçalho da mensagem
            message[0] = 0xC8;  // Sync byte
            message[1] = 0x18;  //Lenght
            message[2] = 0x16;  // Tipo de mensagem CRSF_FRAMETYPE_RC_CHANNELS_PACKED

            int offset = 0;
            for (int i = 0; i < channels.Length; i++)
            {
                WriteUshortInByteArray(ref message, channels[i], ref offset);
            }

            // Calcular checksum
            byte[] arrayCheckSum = new byte[23];
            Array.Copy(message, 2, arrayCheckSum, 0, 23);
            message[25] = CRC8.ComputeChecksum(arrayCheckSum);

            return message;
        }
        public void WriteUshortInByteArray(ref byte[] byteArray, ushort channel, ref int offset)
        {
            int bytePosition = (offset / 8) + 3;
            int bitPosition = offset % 8;

            uint tempShort = (uint) (channel << bitPosition);
            byteArray[bytePosition] = (byte) (BitConverter.GetBytes(tempShort)[0] | byteArray[bytePosition]);
            byteArray[bytePosition + 1] = (byte) (BitConverter.GetBytes(tempShort)[1] | byteArray[bytePosition + 1]);
            byteArray[bytePosition + 2] = (byte) (BitConverter.GetBytes(tempShort)[2] | byteArray[bytePosition + 2]);
            offset += 11;
        }
        public void SetChannelRow(int value)
        {
            Channels[0] = Convert.ToUInt16(value);
        }
        public void SetChannelPitch(int value)
        {
            Channels[1] = Convert.ToUInt16(value);
        }
        public void SetChannelYaw(int value)
        {
            Channels[3] = Convert.ToUInt16(value);
        }
        public void SetChannelTrotle(int value)
        {
            Channels[2] = Convert.ToUInt16(value);
        }
        public void SetChannelAux(int auxNum, int value)
        {
            Channels[auxNum + 3] = Convert.ToUInt16(value);
        }
        public ushort GetAuxValue(int auxNum)
        {
            return Channels[auxNum + 3];
        }
    }
}
