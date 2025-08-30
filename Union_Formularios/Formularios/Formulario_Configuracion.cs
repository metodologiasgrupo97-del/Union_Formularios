using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Configuracion : Form
    {
        private Form Sub_Formualrio;      
        private Panel panelEscritorio;
        private bool _eventsWired;

        public Formulario_Configuracion(Panel panelContenedor)
        {
            InitializeComponent();
            this.panelEscritorio = panelContenedor;
            WireNavButtonsOnce();
        }

        public Formulario_Configuracion()
        {
            InitializeComponent();
            WireNavButtonsOnce();
        }

        private void WireNavButtonsOnce()
        {
            if (_eventsWired) return;
            HookClickByName("edt_dash_config", edt_dash_config_Click);

            _eventsWired = true;
        }


        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;
            ctrl.Click -= handler;
            ctrl.Click += handler;
        }


        private void OpenOrEmbed(Form form)
        {
            try
            {
                if (panelEscritorio != null && !panelEscritorio.IsDisposed)
                {
                    var existente = panelEscritorio.Controls
                        .OfType<Form>()
                        .FirstOrDefault(f => f.GetType() == form.GetType());

                    if (existente != null)
                    {
                        if (existente.WindowState == FormWindowState.Minimized)
                            existente.WindowState = FormWindowState.Normal;
                        existente.BringToFront();
                        existente.Show();
                        existente.Focus();
                        form.Dispose(); 
                        return;
                    }

                    Abrir_Sub_Formulario(form); 
                }
                else
                {
                    using (form)
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el formulario solicitado:\n" + ex.Message,
                    "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Abrir_Sub_Formulario(Form form)
        {
            if (Sub_Formualrio != null)
                Sub_Formualrio.Close();

            Sub_Formualrio = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            if (!panelEscritorio.Controls.Contains(form))
            {
                panelEscritorio.Controls.Clear();
                panelEscritorio.Controls.Add(form);
            }

            panelEscritorio.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void edt_dash_config_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_EdtDash(panelEscritorio));
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Add_Users(panelEscritorio));
        }

        private void txt_Edit_telf_Profile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}
