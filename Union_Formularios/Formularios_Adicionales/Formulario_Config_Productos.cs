using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Union_Formularios.Formularios;
using Union_Formularios.Formularios_Adicionales;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_Config_Productos : Form
    {
        // ===== Embebido (igual patrón que Formulario_Configuracion) =====
        private Form Sub_Formulario;            // sub-form alojado en el panel host
        private Panel panelEscritorio;          // panel host (se puede inyectar en ctor)
        private bool _eventsWired = false;

        // ===== UserControls internos =====
        private UC_MarcasModelos _viewMM;
        private UC_Repuestos _viewRep;
        private bool _mmInitDone = false;
        private bool _repInitDone = false;

        public Formulario_Config_Productos()
        {
            InitializeComponent();
            this.Load += Formulario_Config_Productos_Load;
            WireNavButtonsOnce();
        }

        // Si prefieres pasar el panel explícitamente:
        public Formulario_Config_Productos(Panel panelContenedor) : this()
        {
            this.panelEscritorio = panelContenedor;
        }

        // ---------- Cableado único (por nombre) ----------
        private void WireNavButtonsOnce()
        {
            if (_eventsWired) return;

            HookClickByName("edt_perfil_products", edt_perfil_products_Click);
            HookClickByName("add_trabajador_products", add_trabajador_products_Click);
            HookClickByName("edt_dash_products", edt_dash_products_Click);

            HookClickByName("btn_Reg_Marc_Model", (s, e) => ShowMM());
            HookClickByName("btn_Reg_Repuestos", (s, e) => ShowRep());

            this.KeyPreview = true;
            this.KeyDown -= Form_KeyDown;
            this.KeyDown += Form_KeyDown;

            _eventsWired = true;
        }

        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;
            ctrl.Click -= handler;
            ctrl.Click += handler;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D1) { ShowMM(); e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.D2) { ShowRep(); e.Handled = true; }
        }

        // ---------- Load ----------
        private void Formulario_Config_Productos_Load(object sender, EventArgs e)
        {
            // Si no te pasaron el panel, intenta detectar el padre donde estás embebido
            if (panelEscritorio == null) panelEscritorio = FindHostPanel();

            TryEnableDoubleBuffer(Panel_Control_Productos);

            _viewMM = new UC_MarcasModelos { Dock = DockStyle.Fill, Visible = false };
            _viewRep = new UC_Repuestos { Dock = DockStyle.Fill, Visible = false };

            Panel_Control_Productos.Controls.Add(_viewMM);
            Panel_Control_Productos.Controls.Add(_viewRep);

            ShowMM();
        }

        // Intenta localizar el panel host donde está incrustado este form
        private Panel FindHostPanel()
        {
            Control p = this.Parent;
            while (p != null && !(p is Panel)) p = p.Parent;
            return p as Panel;
        }

        // ========== TOP NAV: SIEMPRE EMBEBER (sin ventanas) ==========
        private void edt_perfil_products_Click(object sender, EventArgs e)
        {
            // Editar perfil → Formulario_Configuracion
            EmbedInHost(new Formulario_Configuracion(panelEscritorio));
        }

        private void add_trabajador_products_Click(object sender, EventArgs e)
        {
            EmbedInHost(new Formulario_Add_Users(panelEscritorio));
        }

        private void edt_dash_products_Click(object sender, EventArgs e)
        {
            EmbedInHost(new Formulario_EdtDash(panelEscritorio));
        }

        // Igual que Abrir_Sub_Formulario en tu otro form, pero usando el host actual
        private void EmbedInHost(Form form)
        {
            var host = panelEscritorio ?? FindHostPanel();
            if (host == null) return; // No se abre ventana: si no hay host, no navegamos

            // Cierra subform anterior
            if (Sub_Formulario != null) Sub_Formulario.Close();

            Sub_Formulario = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            host.Controls.Clear();
            host.Controls.Add(form);
            host.Tag = form;

            form.BringToFront();
            form.Show();
        }

        // ========== SUB NAV: UCs internos ==========
        private void ShowMM()
        {
            ShowView(_viewMM);
            ActivateNav(btn_Reg_Marc_Model, btn_Reg_Repuestos);

            if (!_mmInitDone)
            {
                _mmInitDone = true;
                var mi = _viewMM.GetType().GetMethod("CargarDatosIniciales");
                if (mi != null) mi.Invoke(_viewMM, null);
            }
        }

        private void ShowRep()
        {
            ShowView(_viewRep);
            ActivateNav(btn_Reg_Repuestos, btn_Reg_Marc_Model);

            if (!_repInitDone)
            {
                _repInitDone = true;
                var mi = _viewRep.GetType().GetMethod("CargarDatosIniciales");
                if (mi != null) mi.Invoke(_viewRep, null);
            }
        }

        private void ShowView(UserControl viewToShow)
        {
            if (viewToShow == null) return;

            Panel_Control_Productos.SuspendLayout();

            foreach (Control c in Panel_Control_Productos.Controls)
                c.Visible = false;

            if (!Panel_Control_Productos.Controls.Contains(viewToShow))
            {
                viewToShow.Dock = DockStyle.Fill;
                Panel_Control_Productos.Controls.Add(viewToShow);
            }

            viewToShow.Visible = true;
            viewToShow.BringToFront();

            Panel_Control_Productos.ResumeLayout(true);
        }

        // Resalta botón activo (Guna2 o Button)
        private void ActivateNav(Control active, Control inactive)
        {
            if (active is Guna2Button ga && inactive is Guna2Button gi)
            {
                ga.FillColor = Color.FromArgb(59, 130, 246);
                ga.ForeColor = Color.White;
                ga.HoverState.FillColor = Color.FromArgb(37, 99, 235);

                gi.FillColor = Color.FromArgb(234, 238, 244);
                gi.ForeColor = Color.FromArgb(30, 41, 59);
                gi.HoverState.FillColor = Color.FromArgb(223, 227, 235);
                return;
            }

            if (active is Button ba && inactive is Button bi)
            {
                ba.BackColor = Color.FromArgb(59, 130, 246);
                ba.ForeColor = Color.White;

                bi.BackColor = Color.FromArgb(230, 230, 230);
                bi.ForeColor = Color.FromArgb(30, 41, 59);
                return;
            }

            active.BackColor = Color.FromArgb(59, 130, 246);
            active.ForeColor = Color.White;
            inactive.BackColor = Color.FromArgb(230, 230, 230);
            inactive.ForeColor = Color.FromArgb(30, 41, 59);
        }

        // Antiflicker
        private void TryEnableDoubleBuffer(Control ctrl)
        {
            try
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, ctrl, new object[] { true });
            }
            catch { }
        }
    }
}
