namespace DroneServerProject.Views
{
    partial class ConexaoView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSerialPort = new System.Windows.Forms.ComboBox();
            this.lblTransmissorSerialStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConectarSerial = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSerialPortUpdate = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblConnectedDrones = new System.Windows.Forms.Label();
            this.flowPanelDrones = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(833, 50);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(833, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "Conexão";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.flowPanelDrones);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(833, 453);
            this.panel2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSerialPortUpdate);
            this.groupBox1.Controls.Add(this.cbSerialPort);
            this.groupBox1.Controls.Add(this.lblTransmissorSerialStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnConectarSerial);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 84);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transmissor RF";
            // 
            // cbSerialPort
            // 
            this.cbSerialPort.FormattingEnabled = true;
            this.cbSerialPort.Location = new System.Drawing.Point(86, 20);
            this.cbSerialPort.Name = "cbSerialPort";
            this.cbSerialPort.Size = new System.Drawing.Size(107, 21);
            this.cbSerialPort.TabIndex = 5;
            // 
            // lblTransmissorSerialStatus
            // 
            this.lblTransmissorSerialStatus.AutoSize = true;
            this.lblTransmissorSerialStatus.Location = new System.Drawing.Point(83, 55);
            this.lblTransmissorSerialStatus.Name = "lblTransmissorSerialStatus";
            this.lblTransmissorSerialStatus.Size = new System.Drawing.Size(77, 13);
            this.lblTransmissorSerialStatus.TabIndex = 4;
            this.lblTransmissorSerialStatus.Text = "Desconectado";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status:";
            // 
            // btnConectarSerial
            // 
            this.btnConectarSerial.Location = new System.Drawing.Point(219, 19);
            this.btnConectarSerial.Name = "btnConectarSerial";
            this.btnConectarSerial.Size = new System.Drawing.Size(87, 23);
            this.btnConectarSerial.TabIndex = 2;
            this.btnConectarSerial.Text = "Conectar";
            this.btnConectarSerial.UseVisualStyleBackColor = true;
            this.btnConectarSerial.Click += new System.EventHandler(this.btnConectarSerial_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Porta Serial:";
            // 
            // btnSerialPortUpdate
            // 
            this.btnSerialPortUpdate.BackgroundImage = global::DroneServerProject.Properties.Resources.refresh;
            this.btnSerialPortUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSerialPortUpdate.Location = new System.Drawing.Point(194, 19);
            this.btnSerialPortUpdate.Name = "btnSerialPortUpdate";
            this.btnSerialPortUpdate.Size = new System.Drawing.Size(22, 22);
            this.btnSerialPortUpdate.TabIndex = 6;
            this.btnSerialPortUpdate.UseVisualStyleBackColor = true;
            this.btnSerialPortUpdate.Click += new System.EventHandler(this.btnSerialPortUpdate_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblConnectedDrones);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(338, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(483, 84);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Drones Conectados:";
            // 
            // lblConnectedDrones
            // 
            this.lblConnectedDrones.AutoSize = true;
            this.lblConnectedDrones.Location = new System.Drawing.Point(125, 24);
            this.lblConnectedDrones.Name = "lblConnectedDrones";
            this.lblConnectedDrones.Size = new System.Drawing.Size(13, 13);
            this.lblConnectedDrones.TabIndex = 1;
            this.lblConnectedDrones.Text = "0";
            // 
            // flowPanelDrones
            // 
            this.flowPanelDrones.Location = new System.Drawing.Point(14, 107);
            this.flowPanelDrones.Name = "flowPanelDrones";
            this.flowPanelDrones.Size = new System.Drawing.Size(807, 334);
            this.flowPanelDrones.TabIndex = 7;
            // 
            // ConexaoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 503);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ConexaoView";
            this.Text = "ConexaoView";
            this.Load += new System.EventHandler(this.ConexaoView_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbSerialPort;
        private System.Windows.Forms.Label lblTransmissorSerialStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConectarSerial;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSerialPortUpdate;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblConnectedDrones;
        private System.Windows.Forms.FlowLayoutPanel flowPanelDrones;
    }
}