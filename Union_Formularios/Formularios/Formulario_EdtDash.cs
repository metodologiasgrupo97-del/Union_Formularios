using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_EdtDash : Form
    {
        private Form Sub_Formualrio;
        private Panel panelEscritorio;
        private bool _eventsWired;

        public Formulario_EdtDash(Panel panelContenedor)
        {
            InitializeComponent();
            this.panelEscritorio = panelContenedor;
            WireNavButtonsOnce();
        }

        public Formulario_EdtDash()
        {
            InitializeComponent();
            WireNavButtonsOnce();
        }

        // ==================== Asignar eventos a los botones ====================
        private void WireNavButtonsOnce()
        {
            if (_eventsWired) return;

            HookClickByName("edt_perfil_dash", edt_perfil_dash_Click);
            HookClickByName("add_trabajador_dash", add_trabajador_dash_Click);

            _eventsWired = true;
        }

        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;
            ctrl.Click -= handler;
            ctrl.Click += handler;
        }

        // ==================== Métodos de apertura ====================
        private void OpenOrEmbed(Form form)
        {
            if (panelEscritorio == null || panelEscritorio.IsDisposed)
            {
                form.Dispose();
                return;
            }

            var existente = panelEscritorio.Controls
                .OfType<Form>()
                .FirstOrDefault(f => f.GetType() == form.GetType());

            if (existente != null)
            {
                existente.BringToFront();
                existente.Show();
                existente.Focus();
                form.Dispose();
                return;
            }

            if (Sub_Formualrio != null)
                Sub_Formualrio.Close();

            Sub_Formualrio = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelEscritorio.Controls.Clear();
            panelEscritorio.Controls.Add(form);
            panelEscritorio.Tag = form;
            form.BringToFront();
            form.Show();
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

        // ==================== Handlers de los botones ====================
        private void edt_perfil_dash_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Configuracion(panelEscritorio));
        }

        private void add_trabajador_dash_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Add_Users(panelEscritorio));
        }

        // ==================== Validaciones opcionales ====================
        private void guna2TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txt_RUC_Config_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}
