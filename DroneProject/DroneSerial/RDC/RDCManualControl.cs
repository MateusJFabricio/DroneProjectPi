using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDC.RDCProtocol
{
    public class RDCManualControl
    {
        public const int DATA_LENGTH = 32;
        private const int ROW = 0;
        private const int PITCH = 2;
        private const int TROTLE = 4;
        private const int YAW = 6;
        private const int ENABLE = 8;
        public ushort Row
        {
            get => BitConverter.ToUInt16(_rawData, ROW);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, ROW, 2);
        }
        public ushort Pitch
        {
            get => BitConverter.ToUInt16(_rawData, TROTLE);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, TROTLE, 2);
        }
        public ushort Trotle
        {
            get => BitConverter.ToUInt16(_rawData, PITCH);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, PITCH, 2);
        }

        public ushort Yaw
        {
            get => BitConverter.ToUInt16(_rawData, YAW);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, YAW, 2);
        }
        public ushort Enable
        {
            get => BitConverter.ToUInt16(_rawData, ENABLE);
            set => Array.Copy(BitConverter.GetBytes(value), 0, _rawData, ENABLE, 2);
        }

        public ushort[] Channels
        {
            get
            {
                var channels = new ushort[16];
                for (int i = 0; i < channels.Length; i++)
                {
                    channels[i] = BitConverter.ToUInt16(_rawData, i * 2);
                }
                return channels;
            }
        }
        public byte[] RawData { get => _rawData; set => _rawData = value; }
        private byte[] _rawData = new byte[DATA_LENGTH];
        public RDCManualControl()
        {

        }
        public RDCManualControl(byte[] data)
        {
            if (data.Length != DATA_LENGTH)
            {
                throw new Exception("Data nok");
            }
            RawData = data;
        }

        public void SetChannel(int channel, ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, channel * 2, 2);
        }

        /// <summary>
        /// Set do canal auxiliar iniciando em 1 até 12
        /// </summary>
        /// <param name="aux">Numero do canal auxiliar</param>
        /// <param name="value">valor do canal auxiliar</param>
        public void SetAuxChannel(int aux, ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, 6 + (aux * 2), 2);
        }
    }
}
