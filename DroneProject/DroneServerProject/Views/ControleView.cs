using DroneServerProject.Controller;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DroneServerProject.Views
{
    public partial class ControleView : Form
    {
        public DroneController DroneController { get; set; }
        public bool EnableDrawYawPitch { get; private set; }
        public bool EnableDrawTrotleRow { get; private set; }
        VirtualControl VirtualControlYawPitch { get; set; }
        VirtualControl VirtualControlTrotleRow { get; set; }
        public bool DroneEnable { get; set; } = false;

        public ControleView(DroneController droneController)
        {
            InitializeComponent();
            DroneController = droneController;

            pnlYawPitch.Height = 260;
            pnlYawPitch.Width = 260;
            VirtualControlYawPitch = new VirtualControl(pnlYawPitch.Width, pnlYawPitch.Height);

            pnlTrotleRow.Height = 260;
            pnlTrotleRow.Width = 260;

            VirtualControlTrotleRow = new VirtualControl(pnlTrotleRow.Width, pnlTrotleRow.Height);
        }
        private void InicializaControles()
        {

            VirtualControlYawPitch.BackgroundColor = SystemColors.Control;

            using (Graphics g = pnlYawPitch.CreateGraphics())
            {
                var bitmap = VirtualControlYawPitch.DrawControl();
                g.DrawImage(bitmap, 0, 0);
            }

            VirtualControlTrotleRow.BackgroundColor = SystemColors.Control;

            using (Graphics g = pnlTrotleRow.CreateGraphics())
            {
                var bitmap = VirtualControlTrotleRow.DrawControl();
                g.DrawImage(bitmap, 0, 0);
            }
        }

        private void pnlYawPitch_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                EnableDrawYawPitch = true;
            else
            {
                VirtualControlYawPitch.PointerPosition = new Point(0, 0);

                using (Graphics g = pnlYawPitch.CreateGraphics())
                {
                    var bitmap = VirtualControlYawPitch.DrawControl();
                    g.DrawImage(bitmap, 0, 0);
                }
                lblPitch.Text = "Pitch: " + Math.Round(VirtualControlYawPitch.PointerScaledPositionX * 100).ToString();
                lblYaw.Text = "Yaw: " + Math.Round(VirtualControlYawPitch.PointerScaledPositionY * 100).ToString();
            }
        }

        private void pnlYawPitch_MouseUp(object sender, MouseEventArgs e)
        {
            EnableDrawYawPitch = false;
            VirtualControlYawPitch.PointerPosition = new Point(0, 0);
            using (Graphics g = pnlYawPitch.CreateGraphics())
            {
                var bitmap = VirtualControlYawPitch.DrawControl();
                g.DrawImage(bitmap, 0, 0);
            }
        }

        private void pnlYawPitch_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableDrawYawPitch)
            {
                VirtualControlYawPitch.PointerPosition = new Point(e.X - VirtualControlYawPitch.Width / 2, e.Y - VirtualControlYawPitch.Height / 2);

                using (Graphics g = pnlYawPitch.CreateGraphics())
                {
                    var bitmap = VirtualControlYawPitch.DrawControl();
                    g.DrawImage(bitmap, 0, 0);
                }
                lblPitch.Text = "Pitch: " + Math.Round(VirtualControlYawPitch.PointerScaledPositionX * 100).ToString();
                lblYaw.Text = "Yaw: " + Math.Round(VirtualControlYawPitch.PointerScaledPositionY * 100).ToString();
            }
        }

        private void ControleView_Load(object sender, EventArgs e)
        {
            InicializaControles();
        }

        private void pnlTrotleRow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                EnableDrawTrotleRow = true;
            else
            {
                VirtualControlTrotleRow.PointerPosition = new Point(0, 0);

                using (Graphics g = pnlTrotleRow.CreateGraphics())
                {
                    var bitmap = VirtualControlTrotleRow.DrawControl();
                    g.DrawImage(bitmap, 0, 0);
                }
                lblTrotle.Text = "Trotle: " + Math.Round(VirtualControlTrotleRow.PointerScaledPositionX * 100).ToString();
                lblRow.Text = "Row: " + Math.Round(VirtualControlTrotleRow.PointerScaledPositionY * 100).ToString();
            }
        }

        private void pnlTrotleRow_MouseUp(object sender, MouseEventArgs e)
        {
            EnableDrawTrotleRow = false;

            VirtualControlTrotleRow.PointerPosition = new Point(0, VirtualControlTrotleRow.PointerPosition.Y * -1);
            using (Graphics g = pnlTrotleRow.CreateGraphics())
            {
                var bitmap = VirtualControlTrotleRow.DrawControl();
                g.DrawImage(bitmap, 0, 0);
            }
        }

        private void pnlTrotleRow_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableDrawTrotleRow)
            {
                VirtualControlTrotleRow.PointerPosition = new Point(e.X - VirtualControlTrotleRow.Width / 2, e.Y - VirtualControlTrotleRow.Height / 2);

                using (Graphics g = pnlTrotleRow.CreateGraphics())
                {
                    var bitmap = VirtualControlTrotleRow.DrawControl();
                    g.DrawImage(bitmap, 0, 0);
                }
                lblTrotle.Text = "Trotle: " + Math.Round(VirtualControlTrotleRow.PointerScaledPositionX * 100).ToString();
                lblRow.Text = "Row: " + Math.Round(VirtualControlTrotleRow.PointerScaledPositionY * 100).ToString();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DroneController.DroneSerial.ManualControl.Yaw = Convert.ToUInt16(VirtualControlYawPitch.PointerScaledPositionX * 1000 + 1000);
            DroneController.DroneSerial.ManualControl.Pitch = Convert.ToUInt16(VirtualControlYawPitch.PointerScaledPositionY * 1000 + 1000);
            DroneController.DroneSerial.ManualControl.Trotle = Convert.ToUInt16(VirtualControlTrotleRow.PointerScaledPositionX * 1000 + 1000);
            DroneController.DroneSerial.ManualControl.Row = Convert.ToUInt16(VirtualControlTrotleRow.PointerScaledPositionY * 1000 + 1000);
            if (DroneEnable)
            {
                DroneController.DroneSerial.ManualControl.Enable = 1400;
            }
            else
            {
                DroneController.DroneSerial.ManualControl.Enable = 720;
            }
            
        }

        private void btnDroneEnable_Click(object sender, EventArgs e)
        {
            DroneEnable = !DroneEnable;
            if (DroneEnable)
            {
                btnDroneEnable.BackColor = Color.Green;
            }
            else
            {
                btnDroneEnable.BackColor = SystemColors.Control;
            }
        }

        private void ControleView_Resize(object sender, EventArgs e)
        {
            InicializaControles();
        }
    }

    public class VirtualControl
    {
        public Point PointerPosition { 
            get => new Point(VirtualPointer.X, VirtualPointer.Y) ; 
            set => VirtualPointer.SetPosition(value); 
        }
        public double PointerScaledPositionX { get => VirtualPointer.X_Scaled; }
        public double PointerScaledPositionY { get => VirtualPointer.Y_Scaled; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int PointerRadio { get => VirtualPointer.Raio; set => VirtualPointer.Raio = value; }
        private int _border {  get => VirtualPointer.Raio; }
        private VirtualPointer VirtualPointer { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Color PointerColor {
            get { return VirtualPointer.Color; } 
            set { VirtualPointer.Color = value; } 
        }
        public Color LineColor {  get; set; } = Color.White;
        public Color Color { get; set; } = Color.Gray;
        public VirtualControl(int with, int height)
        {
            Width = with;
            Height = height;
            var raio = 20;
            VirtualPointer = new VirtualPointer(Width / 2, Height /2 , raio, Width / 2 - (raio));
        }
        public Bitmap DrawControl()
        {
            var bitmap = new Bitmap(Height, Width);
            var bitmapWithBorder = new Bitmap(Height - _border * 2, Width - _border * 2);
            using (Graphics g = Graphics.FromImage(bitmapWithBorder))
            {                
                g.Clear(BackgroundColor); //Background

                using (Brush brush = new SolidBrush(Color))
                {
                    g.FillEllipse(brush, 0, 0, Width - _border * 2, Height - _border * 2); //Circle
                }

                using (Pen pen = new Pen(LineColor, 3))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    g.DrawLine(pen, 0, bitmapWithBorder.Height / 2, bitmapWithBorder.Width, bitmapWithBorder.Height / 2); //Horizontal
                    g.DrawLine(pen, bitmapWithBorder.Width / 2, 0, bitmapWithBorder.Width / 2, bitmapWithBorder.Height); //Vertical
                }

            }   

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(BackgroundColor);
                g.DrawImage(bitmapWithBorder, _border, _border);
                g.DrawImage(
                    VirtualPointer.DrawControl(), 
                    VirtualPointer.X + Width / 2 - VirtualPointer.Raio, 
                    VirtualPointer.Y * -1 + Height / 2 - VirtualPointer.Raio
                    );
            }

            return bitmap;
        }
    }
    public class VirtualPointer
    {
        public int Raio { get; set; }
        private int _x = 0;
        private int _y = 0;
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value * -1; }
        public Color Color { get; set; } = Color.FromArgb(200, Color.Blue);
        private int _maxDistance;
        public double ActualDistancePercentage { get; set; } = 0;
        public double X_Scaled { get =>  Convert.ToDouble(X) / Convert.ToDouble(_maxDistance);}
        public double Y_Scaled { get => Convert.ToDouble(Y) / Convert.ToDouble(_maxDistance); }
        public VirtualPointer(int startX, int startY, int raio, int _maxDistanceFromOrigin)
        {
            Raio = raio;
            X = 0;
            Y = 0;
            _maxDistance = _maxDistanceFromOrigin;
        }
        public void SetPosition(Point point)
        {
            SetPosition(point.X, point.Y);
        }
        public void SetPosition(int x, int y)
        {
            if (InWorkPlane(x, y))
            {
                X = x;
                Y = y;
            }
            else
            {
                
            }
        }
        public void RestorePosition()
        {
            X = 0;
            Y = 0;
        }
        public Bitmap DrawControl()
        {
            var bitmap = new Bitmap(Raio * 2, Raio * 2);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (Brush b = new SolidBrush(Color))
                {
                    g.FillEllipse(b, 0, 0, Raio * 2, Raio * 2);
                }
            }

            return bitmap;
        }
        private bool InWorkPlane(int x, int y)
        {
            //v(x-0)2 + (y-0)2 =vx2 + y2
            var actualDistance = Math.Sqrt(Math.Pow((x - 0), 2) + Math.Pow((y - 0), 2));
            ActualDistancePercentage = actualDistance / _maxDistance;
            if (ActualDistancePercentage > 1) ActualDistancePercentage = 1;
            return actualDistance < _maxDistance;
        }
    }
}
