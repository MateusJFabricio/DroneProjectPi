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

namespace DroneServerProject.Views.Cards
{
    public partial class DroneConnectionCard : Form
    {
        public DroneController Drone { get; set; }
        public DroneConnectionCard(DroneController drone)
        {
            InitializeComponent();
            this.Drone = drone;
        }

        private void ConexaoView_Load(object sender, EventArgs e)
        {
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
