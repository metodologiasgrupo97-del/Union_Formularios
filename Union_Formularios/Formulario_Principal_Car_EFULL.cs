using Capa_Corte_Transversal.Cache;
using FontAwesome.Sharp;
using Formulario_Principal_Car_EFULL.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Union_Formularios;
using Union_Formularios.Formularios;
namespace Formulario_Principal_Car_EFULL
{
    public partial class Fr_Dashboard : Form
    {
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form Sub_Formualrio;

        public Fr_Dashboard()
        {
            InitializeComponent();
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            Panel_Dashboard.Controls.Add(leftBorderBtn);
            Cargar_Nombre_Usuario();
            Cargar_Cargo_Usuario();
            Cargar_Correo_Usuario();

            Nom_Usu.ForeColor = Color.White;
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
        }
        private struct RGBColors 
        {
            public static Color color1 = Color.FromArgb(172, 126, 241);
            public static Color color2 = Color.FromArgb(249, 118, 176);
            public static Color color3 = Color.FromArgb(253, 138, 114);
            public static Color color4 = Color.FromArgb(95, 77, 221);
            public static Color color5 = Color.FromArgb(249, 88, 155);
            public static Color color6 = Color.FromArgb(24, 161, 251);
        }
        

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DesactivateButton();
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(34, 44, 126);
                currentBtn.ForeColor = color;
                currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.ForeColor = color;
                currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentBtn.ImageAlign = ContentAlignment.MiddleRight;
                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();
                Icono_Formulario_Principal.IconChar = currentBtn.IconChar;
                Icono_Formulario_Principal.IconColor = color;
            }
        }

        private void DesactivateButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = Color.FromArgb(51, 67, 190);
                currentBtn.ForeColor = Color.Gainsboro;
                currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.ForeColor = Color.Gainsboro;
                currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void Abrir_Sub_Formulario(Form form)
        {
            if (Panel_Escritorio == null)
            {
                MessageBox.Show("Panel no inicializado.");
                return;
            }
            if (Sub_Formualrio != null)
            {
                Sub_Formualrio.Close();
            }
            Sub_Formualrio = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            Panel_Escritorio.Controls.Add(form);
            Panel_Escritorio.Tag = form;
            form.BringToFront();
            form.Show();
            lblInicio.Text = form.Text;
        }

        private void btnMantenimiento_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            Abrir_Sub_Formulario(new Formulario_Mantenimiento());
        }

        private void btnVehiculos_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            Abrir_Sub_Formulario(new Formulario_Vehiculos());
        }

        private void btnPropietarios_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color4);
            Abrir_Sub_Formulario(new Formulario_Propietarios());
        }

        private void btnFacturas_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
            Abrir_Sub_Formulario(new Formulario_Facturas());
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color6);
            Abrir_Sub_Formulario(new Formulario_Reportes());
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            Abrir_Sub_Formulario(new Formulario_Configuracion(Panel_Escritorio)); 
        }
        private void MostrarInicio()
        {
            Panel_Escritorio.Controls.Clear();
            InicioControl inicio = new InicioControl();
            inicio.Dock = DockStyle.Fill;
            Panel_Escritorio.Controls.Add(inicio);
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            if (Sub_Formualrio != null)
            {
                Sub_Formualrio.Close();
                Sub_Formualrio = null; 
            }

            Panel_Escritorio.Controls.Clear();
            MostrarInicio();
            Reset();
        }

        private void Reset() 
        {
            DesactivateButton();
            leftBorderBtn.Visible = false;
            Icono_Formulario_Principal.IconChar = IconChar.Home;
            Icono_Formulario_Principal.IconColor = Color.Gray;
            lblInicio.Text = "Inicio";
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Panel_Titulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estas Seguro de querer cerrar el formulario?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else 
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Estas Seguro de querer cerrar sesión?", "Warning",MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) 
            {
                this.Close();
            }
        }

        private void Cargar_Nombre_Usuario()
        {
            Nom_Usu.Text = Cache_Inicio_Sesion_Usuario.FirstName;
        }
        private void Cargar_Cargo_Usuario() 
        {
            lblCargo.Text = Cache_Inicio_Sesion_Usuario.Position;
        }
        private void Cargar_Correo_Usuario() 
        {
            lblEmail.Text = Cache_Inicio_Sesion_Usuario.Email;
        }

        private void Fr_Dashboard_Load(object sender, EventArgs e)
        {
            MostrarInicio();
        }
    }
}