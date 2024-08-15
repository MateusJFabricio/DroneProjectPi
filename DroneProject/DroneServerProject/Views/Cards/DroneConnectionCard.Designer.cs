namespace DroneServerProject.Views.Cards
{
    partial class DroneConnectionCard
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
            this.lblDroneName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblVelocidade = new System.Windows.Forms.Label();
            this.lblBateria = new System.Windows.Forms.Label();
            this.lblSensor1 = new System.Windows.Forms.Label();
            this.lblSensor2 = new System.Windows.Forms.Label();
            this.lblSensor3 = new System.Windows.Forms.Label();
            this.lblSensor4 = new System.Windows.Forms.Label();
            this.lblModoOperacao = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAltura = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDroneName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 35);
            this.panel1.TabIndex = 0;
            // 
            // lblDroneName
            // 
            this.lblDroneName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDroneName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDroneName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDroneName.ForeColor = System.Drawing.Color.White;
            this.lblDroneName.Location = new System.Drawing.Point(0, 0);
            this.lblDroneName.Name = "lblDroneName";
            this.lblDroneName.Size = new System.Drawing.Size(284, 35);
            this.lblDroneName.TabIndex = 0;
            this.lblDroneName.Text = "Nome Drone";
            this.lblDroneName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Controls.Add(this.lblStatus);
            this.panel2.Controls.Add(this.lblModoOperacao);
            this.panel2.Controls.Add(this.lblBateria);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 181);
            this.panel2.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(3, 0);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(66, 13);
            this.lblLatitude.TabIndex = 0;
            this.lblLatitude.Text = "Latitude: 0.0";
            // 
            // lblVelocidade
            // 
            this.lblVelocidade.AutoSize = true;
            this.lblVelocidade.Location = new System.Drawing.Point(3, 40);
            this.lblVelocidade.Name = "lblVelocidade";
            this.lblVelocidade.Size = new System.Drawing.Size(81, 13);
            this.lblVelocidade.TabIndex = 2;
            this.lblVelocidade.Text = "Velocidade: 0.0";
            // 
            // lblBateria
            // 
            this.lblBateria.AutoSize = true;
            this.lblBateria.Location = new System.Drawing.Point(211, 13);
            this.lblBateria.Name = "lblBateria";
            this.lblBateria.Size = new System.Drawing.Size(61, 13);
            this.lblBateria.TabIndex = 3;
            this.lblBateria.Text = "Bateria: 0.0";
            // 
            // lblSensor1
            // 
            this.lblSensor1.AutoSize = true;
            this.lblSensor1.Location = new System.Drawing.Point(3, 0);
            this.lblSensor1.Name = "lblSensor1";
            this.lblSensor1.Size = new System.Drawing.Size(75, 13);
            this.lblSensor1.TabIndex = 4;
            this.lblSensor1.Text = "Sensor 1: OFF";
            // 
            // lblSensor2
            // 
            this.lblSensor2.AutoSize = true;
            this.lblSensor2.Location = new System.Drawing.Point(3, 20);
            this.lblSensor2.Name = "lblSensor2";
            this.lblSensor2.Size = new System.Drawing.Size(75, 13);
            this.lblSensor2.TabIndex = 5;
            this.lblSensor2.Text = "Sensor 2: OFF";
            // 
            // lblSensor3
            // 
            this.lblSensor3.AutoSize = true;
            this.lblSensor3.Location = new System.Drawing.Point(3, 40);
            this.lblSensor3.Name = "lblSensor3";
            this.lblSensor3.Size = new System.Drawing.Size(75, 13);
            this.lblSensor3.TabIndex = 6;
            this.lblSensor3.Text = "Sensor 3: OFF";
            // 
            // lblSensor4
            // 
            this.lblSensor4.AutoSize = true;
            this.lblSensor4.Location = new System.Drawing.Point(3, 60);
            this.lblSensor4.Name = "lblSensor4";
            this.lblSensor4.Size = new System.Drawing.Size(75, 13);
            this.lblSensor4.TabIndex = 7;
            this.lblSensor4.Text = "Sensor 4: OFF";
            // 
            // lblModoOperacao
            // 
            this.lblModoOperacao.AutoSize = true;
            this.lblModoOperacao.Location = new System.Drawing.Point(12, 13);
            this.lblModoOperacao.Name = "lblModoOperacao";
            this.lblModoOperacao.Size = new System.Drawing.Size(150, 13);
            this.lblModoOperacao.TabIndex = 8;
            this.lblModoOperacao.Text = "Modo de Operação: MANUAL";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 31);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(137, 13);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "Status: Aguardando Enable";
            // 
            // lblAltura
            // 
            this.lblAltura.AutoSize = true;
            this.lblAltura.Location = new System.Drawing.Point(3, 60);
            this.lblAltura.Name = "lblAltura";
            this.lblAltura.Size = new System.Drawing.Size(46, 13);
            this.lblAltura.TabIndex = 10;
            this.lblAltura.Text = "Altura: 0";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblLatitude, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAltura, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblLongitude, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblVelocidade, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 54);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(102, 100);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(3, 20);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(75, 13);
            this.lblLongitude.TabIndex = 1;
            this.lblLongitude.Text = "Longitude: 0.0";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblSensor1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSensor2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblSensor3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblSensor4, 0, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(151, 54);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(102, 100);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // DroneConnectionCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(284, 216);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DroneConnectionCard";
            this.Text = "ConexaoView";
            this.Load += new System.EventHandler(this.ConexaoView_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblDroneName;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblModoOperacao;
        private System.Windows.Forms.Label lblSensor4;
        private System.Windows.Forms.Label lblSensor3;
        private System.Windows.Forms.Label lblSensor2;
        private System.Windows.Forms.Label lblSensor1;
        private System.Windows.Forms.Label lblBateria;
        private System.Windows.Forms.Label lblVelocidade;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblAltura;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblLongitude;
    }
}