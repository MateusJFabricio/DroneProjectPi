﻿using System;
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
            var payload = RDC.RDCPackage.GetPayload(data);
            if (payload.Length != DATA_LENGTH)
            {
                throw new Exception("Data nok");
            }
            RawData = payload;
        }
        public void Initialize()
        {
            ushort value = 720;

            Yaw = value;
            Pitch = value;
            Row = value;
            Trotle = value;

            for (int i = 0; i < 12; i++)
            {
                SetAuxChannel(i, value);
            }
        }
        public void SetChannel(int channel, ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, channel * 2, 2);
        }

        /// <summary>
        /// Set do canal auxiliar iniciando em 0 até 11
        /// </summary>
        /// <param name="aux">Numero do canal auxiliar</param>
        /// <param name="value">valor do canal auxiliar</param>
        public void SetAuxChannel(int aux, ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, _rawData, 8 + (aux * 2), 2);
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
