using DroneServerProject.Controller;
using System;
using System.Windows.Forms;

namespace DroneServerProject.Views
{
    public partial class ControleView : Form
    {
        public DroneController DroneController { get; set; }
        public ControleView(DroneController droneController)
        {
            InitializeComponent();
            DroneController = droneController;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DroneController.DroneSerial.ManualControl.Yaw = Convert.ToUInt16(txtYaw.Text);
            DroneController.DroneSerial.ManualControl.Pitch = Convert.ToUInt16(txtPitch.Text);
            DroneController.DroneSerial.ManualControl.Row = Convert.ToUInt16(txtRow.Text);
        }
    }
}
