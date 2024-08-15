using DroneServerProject.Controller;
using DroneServerProject.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DroneServerProject
{
    public partial class FormMain : Form
    {
        private DroneController droneController = new DroneController();
        private ConexaoView ConexaoView;
        private ControleView ControleView;
        private SettingView SettingView;
        private WorkPlanView WorkPlanView;
        public FormMain()
        {
            InitializeComponent();
            ConexaoView = new ConexaoView(droneController);
            ControleView = new ControleView(droneController);
        }
        private void ResetMenuItemColor()
        {
            lblConexao.BackColor = SystemColors.Control;
            lblControle.BackColor = SystemColors.Control;
            lblConfiguracao.BackColor = SystemColors.Control;
            lblPlanoTrabalho.BackColor = SystemColors.Control;

        }

        private void pnlAplicacao_Resize(object sender, EventArgs e)
        {
            if ((sender as Panel).Controls.Count > 0)
            {
                var form = ((sender as Panel).Controls[0] as Form);
                form.WindowState = FormWindowState.Minimized;
                form.WindowState = FormWindowState.Maximized;
            }
        }
        private void lblConexao_Click(object sender, EventArgs e)
        {
            ResetMenuItemColor();
            lblConexao.BackColor = Color.LightSlateGray;

            pnlAplicacao.Controls.Clear();

            ConexaoView.TopLevel = false;
            pnlAplicacao.Controls.Add(ConexaoView);
            ConexaoView.WindowState = FormWindowState.Maximized;
            ConexaoView.BringToFront();
            ConexaoView.Show();
        }
        private void lblControle_Click(object sender, EventArgs e)
        {
            ResetMenuItemColor();
            lblControle.BackColor = Color.LightSlateGray;

            pnlAplicacao.Controls.Clear();

            ControleView.TopLevel = false;
            pnlAplicacao.Controls.Add(ControleView);
            ControleView.WindowState = FormWindowState.Maximized;
            ControleView.BringToFront();
            ControleView.Show();
        }

        private void lblPlanoTrabalho_Click(object sender, EventArgs e)
        {
            ResetMenuItemColor();
            lblPlanoTrabalho.BackColor = Color.LightSlateGray;

            pnlAplicacao.Controls.Clear();

            WorkPlanView = new WorkPlanView();
            WorkPlanView.TopLevel = false;
            pnlAplicacao.Controls.Add(WorkPlanView);
            WorkPlanView.WindowState = FormWindowState.Maximized;
            WorkPlanView.BringToFront();
            WorkPlanView.Show();
        }

        private void lblConfiguracao_Click(object sender, EventArgs e)
        {
            ResetMenuItemColor();
            lblConfiguracao.BackColor = Color.LightSlateGray;

            pnlAplicacao.Controls.Clear();

            SettingView = new SettingView();
            SettingView.TopLevel = false;
            pnlAplicacao.Controls.Add(SettingView);
            SettingView.WindowState = FormWindowState.Maximized;
            SettingView.BringToFront();
            SettingView.Show();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lblConexao_Click(null, null);
        }
    }
}
