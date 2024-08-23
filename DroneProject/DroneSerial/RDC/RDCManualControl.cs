using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDC.Utils;

namespace RDC.RDCProtocol
{
    public class RDCManualControl
    {
        public const int DATA_LENGTH = 32;
        private const int ROW = 0;
        private const int PITCH = 1;
        private const int TROTLE = 3;
        private const int YAW = 2;
        private const int ENABLE = 4;
        public ushort Row
        {
            get => Channels[ROW];
            set => Channels[ROW] = value;
        }
        public ushort Pitch
        {
            get => Channels[PITCH];
            set => Channels[PITCH] = value;
        }
        public ushort Trotle
        {
            get => Channels[TROTLE];
            set => Channels[TROTLE] = value;
        }

        public ushort Yaw
        {
            get => Channels[YAW];
            set => Channels[YAW] = value;
        }
        public ushort Enable
        {
            get => Channels[ENABLE];
            set => Channels[ENABLE] = value;
        }

        public ushort[] Channels { get; set; } = new ushort[16];
        public RDCManualControl()
        {
            for (int i = 0; i < Channels.Length; i++)
            {
                Channels[i] = 720;
            }
        }
        public RDCManualControl(byte[] data)
        {
            var payload = RDCPackage.GetPayload(data);
            if (payload.Length != DATA_LENGTH)
            {
                throw new Exception("Data nok");
            }
            for (var i = 0; i < 16; i++)
            {
                Channels[i] = BitConverter.ToUInt16(payload, i * 2);
            }
        }

        public void SetChannel(int channel, ushort value)
        {
            Channels[channel + 4] = value;
        }

        public byte[] Encode()
        {
            byte[] message = new byte[36];

            //Cabeçalho da mensagem
            message[0] = 0xFF;  // Sync byte
            message[1] = Convert.ToByte(message.Length - 2);  //Lenght
            message[2] = 0x02;

            //Adicionar o Payload
            var c = Channels;
            for (int i = 0; i < 16; i++)
            {
                Array.Copy(BitConverter.GetBytes(c[i]), 0, message, 3 + i * 2, 2);

            }

            // Calcular checksum
            byte[] arrayCheckSum = new byte[message.Length - 3];
            Array.Copy(message, 2, arrayCheckSum, 0, arrayCheckSum.Length);
            message[message.Length - 1] = CRC8.ComputeChecksum(arrayCheckSum);

            return message;
        }
    }
}
