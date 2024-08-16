namespace DroneServerProject.Views
{
    partial class ControleView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbTrotle = new System.Windows.Forms.TrackBar();
            this.lblRow = new System.Windows.Forms.Label();
            this.lblTrotle = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlTrotleRow = new System.Windows.Forms.Panel();
            this.lblPitch = new System.Windows.Forms.Label();
            this.lblYaw = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlYawPitch = new System.Windows.Forms.Panel();
            this.tbYaw = new System.Windows.Forms.TrackBar();
            this.tbRow = new System.Windows.Forms.TrackBar();
            this.tbPitch = new System.Windows.Forms.TrackBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnDroneEnable = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrotle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPitch)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(945, 50);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(945, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "Controle";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnDroneEnable);
            this.panel2.Controls.Add(this.tbTrotle);
            this.panel2.Controls.Add(this.lblRow);
            this.panel2.Controls.Add(this.lblTrotle);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.pnlTrotleRow);
            this.panel2.Controls.Add(this.lblPitch);
            this.panel2.Controls.Add(this.lblYaw);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.pnlYawPitch);
            this.panel2.Controls.Add(this.tbYaw);
            this.panel2.Controls.Add(this.tbRow);
            this.panel2.Controls.Add(this.tbPitch);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(945, 537);
            this.panel2.TabIndex = 1;
            // 
            // tbTrotle
            // 
            this.tbTrotle.Location = new System.Drawing.Point(758, 59);
            this.tbTrotle.Maximum = 100;
            this.tbTrotle.Minimum = -100;
            this.tbTrotle.Name = "tbTrotle";
            this.tbTrotle.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbTrotle.Size = new System.Drawing.Size(45, 273);
            this.tbTrotle.TabIndex = 11;
            // 
            // lblRow
            // 
            this.lblRow.Location = new System.Drawing.Point(588, 359);
            this.lblRow.Name = "lblRow";
            this.lblRow.Size = new System.Drawing.Size(64, 14);
            this.lblRow.TabIndex = 7;
            this.lblRow.Text = "Row: 0";
            this.lblRow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTrotle
            // 
            this.lblTrotle.Location = new System.Drawing.Point(784, 188);
            this.lblTrotle.Name = "lblTrotle";
            this.lblTrotle.Size = new System.Drawing.Size(80, 18);
            this.lblTrotle.TabIndex = 6;
            this.lblTrotle.Text = "Trotle: 0";
            this.lblTrotle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(478, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(279, 27);
            this.label3.TabIndex = 5;
            this.label3.Text = "Trotle / Row";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTrotleRow
            // 
            this.pnlTrotleRow.Location = new System.Drawing.Point(478, 65);
            this.pnlTrotleRow.Name = "pnlTrotleRow";
            this.pnlTrotleRow.Size = new System.Drawing.Size(279, 261);
            this.pnlTrotleRow.TabIndex = 4;
            this.pnlTrotleRow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTrotleRow_MouseDown);
            this.pnlTrotleRow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTrotleRow_MouseMove);
            this.pnlTrotleRow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTrotleRow_MouseUp);
            // 
            // lblPitch
            // 
            this.lblPitch.Location = new System.Drawing.Point(387, 190);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(64, 14);
            this.lblPitch.TabIndex = 3;
            this.lblPitch.Text = "Pitch: 0";
            this.lblPitch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYaw
            // 
            this.lblYaw.Location = new System.Drawing.Point(173, 355);
            this.lblYaw.Name = "lblYaw";
            this.lblYaw.Size = new System.Drawing.Size(80, 18);
            this.lblYaw.TabIndex = 2;
            this.lblYaw.Text = "Yaw: 0";
            this.lblYaw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(77, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(251, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "Yaw / Pitch";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlYawPitch
            // 
            this.pnlYawPitch.Location = new System.Drawing.Point(77, 65);
            this.pnlYawPitch.Name = "pnlYawPitch";
            this.pnlYawPitch.Size = new System.Drawing.Size(279, 261);
            this.pnlYawPitch.TabIndex = 0;
            this.pnlYawPitch.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlYawPitch_MouseDown);
            this.pnlYawPitch.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlYawPitch_MouseMove);
            this.pnlYawPitch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlYawPitch_MouseUp);
            // 
            // tbYaw
            // 
            this.tbYaw.Location = new System.Drawing.Point(69, 328);
            this.tbYaw.Maximum = 100;
            this.tbYaw.Minimum = -100;
            this.tbYaw.Name = "tbYaw";
            this.tbYaw.Size = new System.Drawing.Size(295, 45);
            this.tbYaw.TabIndex = 8;
            // 
            // tbRow
            // 
            this.tbRow.Location = new System.Drawing.Point(471, 328);
            this.tbRow.Maximum = 100;
            this.tbRow.Minimum = -100;
            this.tbRow.Name = "tbRow";
            this.tbRow.Size = new System.Drawing.Size(295, 45);
            this.tbRow.TabIndex = 9;
            // 
            // tbPitch
            // 
            this.tbPitch.Location = new System.Drawing.Point(358, 59);
            this.tbPitch.Maximum = 100;
            this.tbPitch.Minimum = -100;
            this.tbPitch.Name = "tbPitch";
            this.tbPitch.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbPitch.Size = new System.Drawing.Size(45, 273);
            this.tbPitch.TabIndex = 10;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnDroneEnable
            // 
            this.btnDroneEnable.Location = new System.Drawing.Point(87, 395);
            this.btnDroneEnable.Name = "btnDroneEnable";
            this.btnDroneEnable.Size = new System.Drawing.Size(119, 60);
            this.btnDroneEnable.TabIndex = 12;
            this.btnDroneEnable.Text = "Enable";
            this.btnDroneEnable.UseVisualStyleBackColor = true;
            this.btnDroneEnable.Click += new System.EventHandler(this.btnDroneEnable_Click);
            // 
            // ControleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 587);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ControleView";
            this.Text = "ConexaoView";
            this.Load += new System.EventHandler(this.ControleView_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrotle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbYaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPitch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlYawPitch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.Label lblYaw;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlTrotleRow;
        private System.Windows.Forms.Label lblRow;
        private System.Windows.Forms.Label lblTrotle;
        private System.Windows.Forms.TrackBar tbYaw;
        private System.Windows.Forms.TrackBar tbTrotle;
        private System.Windows.Forms.TrackBar tbRow;
        private System.Windows.Forms.TrackBar tbPitch;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button btnDroneEnable;
    }
}