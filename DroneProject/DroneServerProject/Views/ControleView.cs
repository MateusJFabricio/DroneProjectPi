using DroneServerProject.Controller;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DroneServerProject.Views
{
    public partial class ControleView : Form
    {
        public DroneController DroneController { get; set; }
        public bool EnableDrawYawPitch { get; private set; }
        VirtualControl VirtualControlYawPitch { get; set; }

        public ControleView(DroneController droneController)
        {
            InitializeComponent();
            DroneController = droneController;
            VirtualControlYawPitch = new VirtualControl(pnlYawPitch.Width / 2, pnlYawPitch.Height / 2, 20, pnlYawPitch.Height / 2);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //neController.DroneSerial.ManualControl.Yaw = Convert.ToUInt16(txtYaw.Text);
            //neController.DroneSerial.ManualControl.Pitch = Convert.ToUInt16(txtPitch.Text);
            //DroneController.DroneSerial.ManualControl.Row = Convert.ToUInt16(txtRow.Text);
        }

        private void pnlYawPitch_MouseDown(object sender, MouseEventArgs e)
        {
            EnableDrawYawPitch = true;
        }

        private void pnlYawPitch_MouseUp(object sender, MouseEventArgs e)
        {
            EnableDrawYawPitch = false;
        }

        private void pnlYawPitch_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableDrawYawPitch)
            {
                var bitmap = new Bitmap(pnlYawPitch.Height, pnlYawPitch.Width, );
                VirtualControlYawPitch.SetPosition(e.X, e.Y);
                bitmap.
            }
        }
    }
    public class VirtualControl
    {
        public int Raio { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ScaledX { get; set; }
        public int ScaledY { get; set; }
        private Point _startPoint;
        private int _maxRaio;
        private Color _color = Color.Blue;
        public VirtualControl(int startX, int startY, int raio, int maxRaio) 
        {
            _startPoint = new Point(startX, startY);
            Raio = raio;
            _maxRaio = maxRaio;
        }
        public void SetPosition(int x, int y)
        {
            if (InWorkPlane(x, y))
            {
                X = x;
                Y = y;
            }
        }
        public void RestorePosition()
        {
            X = _startPoint.X;
            Y = _startPoint.Y;
        }
        public void DrawControl(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (Brush b = new SolidBrush(_color))
                {
                    g.FillEllipse(b, X, Y, Raio, Raio);
                }
            }
        }
        private bool InWorkPlane(int x, int y)
        {
            //√(x−0)2 + (y−0)2 =√x2 + y2
            return Math.Sqrt((x - _startPoint.X) * (y - _startPoint.Y)) < _maxRaio;
        }

    }
}
