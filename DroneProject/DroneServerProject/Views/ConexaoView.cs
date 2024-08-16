using DroneServerProject.Controller;
using DroneServerProject.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DroneServerProject.Views
{
    public partial class ConexaoView : Form
    {
        public DroneController Drone { get; set; }
        public ConexaoView(DroneController drone)
        {
            InitializeComponent();
            this.Drone = drone;
        }

        private void ConexaoView_Load(object sender, EventArgs e)
        {
            AtualizarPortas();
        }
        private void AtualizarPortas()
        {
            cbSerialPort.Items.Clear();
            foreach (var porta in SerialPort.GetPortNames())
            {
                cbSerialPort.Items.Add(porta);
            }

            if (cbSerialPort.Items.Count > 0)
                cbSerialPort.SelectedItem = 1;
        }

        private void btnSerialPortUpdate_Click(object sender, EventArgs e)
        {
            AtualizarPortas();
        }

        private void btnConectarSerial_Click(object sender, EventArgs e)
        {
            if (btnConectarSerial.Text == "Conectar")
            {
                if (cbSerialPort.SelectedItem != null)
                {
                    string portName = cbSerialPort.SelectedItem.ToString();
                    if (Drone.DroneSerial == null)
                    {
                        Drone.DroneSerial = new DroneSerial(portName, 57600);
                    }
                    else
                    {
                        Drone.DroneSerial.PortName = portName;
                        Drone.DroneSerial.BaudRate = 57600;
                    }

                    Drone.DroneSerial.Connect();
                    Drone.DroneSerial.StartSerialDataExchange();

                    if (Drone.DroneSerial.IsConnected)
                    {
                        lblTransmissorSerialStatus.Text = "Conectado";
                        btnConectarSerial.Text = "Desconectar";
                    }
                }
            }
            else
            {
                if (Drone.DroneSerial != null)
                {
                    Drone.DroneSerial.StopSerialDataExchange();
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Drone.DroneSerial != null)
            {
                if (Drone.DroneSerial.IsConnected)
                {
                    lblTransmissorSerialStatus.Text = "Conectado";
                    btnConectarSerial.Text = "Desconectar";
                }
                else
                {
                    btnConectarSerial.Text = "Conectar";
                    lblTransmissorSerialStatus.Text = "Desconectado";
                }
            }
        }
    }
}
