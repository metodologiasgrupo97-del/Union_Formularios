using Capa_Corte_Transversal.Cache;
using Dominio;
using Formulario_Principal_Car_EFULL;
using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Union_Formularios
{
    public partial class Formulario_Login : Form
    {
        public Formulario_Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void Panel_Registro_Paint(object sender, PaintEventArgs e)
        {
        }
        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90); 
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90); 
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90); 
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90); 
            path.CloseFigure();

            return path;
        }
        private void txtEnter(object sender, EventArgs e)
        {
            if(txtUsuI.Text == "Ingrese el usuario") 
            {
                txtUsuI.Text = "";
                txtUsuI.ForeColor = Color.Black;
            }

            TextBox tx = sender as TextBox;
            foreach (Control ctr in Panel_Iniciar_sesion.Controls)
            {
                if (ctr is Panel && ctr.Name == "p" + tx.Tag.ToString())
                {
                    ctr.BackColor = Color.FromArgb(46, 72, 186);
                }
            }
        }
        private void txtLeave(object sender, EventArgs e)
        {
            if (txtUsuI.Text == "")
            {
                txtUsuI.Text = "Ingrese el usuario";
                txtUsuI.ForeColor = Color.Gray;
            }

            TextBox tx = sender as TextBox;
            foreach (Control ctr in Panel_Iniciar_sesion.Controls)
            {
                if (ctr is Panel && ctr.Name == "p" + tx.Tag.ToString())
                {
                    ctr.BackColor = Color.Silver;
                }
            }
        }
        private void txtEnter2(object sender, EventArgs e)
        {
            if (txtContraI.Text == "Ingrese la contraseña")
            {
                txtContraI.Text = "";
                txtContraI.ForeColor = Color.Black;

                if (!mostrarContraseña)
                    txtContraI.UseSystemPasswordChar = true;
            }

            TextBox tx = sender as TextBox;
            foreach (Control ctr in Panel_Iniciar_sesion.Controls)
            {
                if (ctr is Panel && ctr.Name == "p" + tx.Tag.ToString())
                {
                    ctr.BackColor = Color.FromArgb(46, 72, 186);
                }
            }
        }
        private void txtLeave2(object sender, EventArgs e)
        {
            if (txtContraI.Text == "")
            {
                txtContraI.UseSystemPasswordChar = false;
                txtContraI.Text = "Ingrese la contraseña";
                txtContraI.ForeColor = Color.Gray;
            }

            TextBox tx = sender as TextBox;
            foreach (Control ctr in Panel_Iniciar_sesion.Controls)
            {
                if (ctr is Panel && ctr.Name == "p" + tx.Tag.ToString())
                {
                    ctr.BackColor = Color.Silver;
                }
            }
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void Panel_Principal_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(txtUsuI.Text != "Ingrese el usuario") 
            {
                if (txtContraI.Text != "Ingrese la contraseña")
                {
                    Modelo_Dominio_Usuario usser = new Modelo_Dominio_Usuario();
                    var validarLogin = usser.LoginUser(txtUsuI.Text, txtContraI.Text);
                    if (validarLogin == true)
                    {
                        var menu = new Formulario_Principal_Car_EFULL.Fr_Dashboard();
                        string nombre = Cache_Inicio_Sesion_Usuario.FirstName;
                        string mensaje = $"😎 ¡Bienvenido, {nombre}!\n\n" + "Nos alegra tenerte de vuelta 🚀\n" + "Explora el sistema y sácale el máximo provecho 💼";
                        MessageBox.Show(mensaje, "Inicio de sesión exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        menu.Show();
                        menu.FormClosed += Cerrar_Sesion;
                        this.Hide();
                    }
                    else 
                    {
                        msgError("Usuario o Contraseña incorrecto \n Por favor intenta de nuevo");
                        txtContraI.Text = "Ingrese la contraseña";
                        txtUsuI.Focus();
                    }
                }
                else msgError(" Porfavor ingrese la contraseña");
            }
            else msgError(" Porfavor ingrese el usuario");
        }
        private void msgError(string msg) 
        {
            lbl_Mensaje_de_Error_Login.Text = " " + msg;
            lbl_Mensaje_de_Error_Login.Visible = true;
            Simbolo_Error.Visible = true;
        }

        private void Cerrar_Sesion(object sender, FormClosedEventArgs e) 
        {
            txtContraI.Clear();
            if (txtContraI.Text == "")
            {
                txtContraI.UseSystemPasswordChar = false;
                txtContraI.Text = "Ingrese la contraseña";
                txtContraI.ForeColor = Color.Gray;
            }
            txtUsuI.Clear();
            if (txtUsuI.Text == "")
            {
                txtUsuI.Text = "Ingrese el usuario";
                txtUsuI.ForeColor = Color.Gray;
            }
            Simbolo_Error.Visible = false;
            lbl_Mensaje_de_Error_Login.Visible = false;
            this.Show();
        }

        private void txtUsuI_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContraI_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
        }

        private void iconButton1_Click_1(object sender, EventArgs e)
        {
        }

        bool mostrarContraseña = false;
        private void Mostrar_Cont_MouseClick(object sender, MouseEventArgs e)
        {
            mostrarContraseña = true;
            Ocultar_Cont.BringToFront();
            if (txtContraI.Text != "Ingrese la contraseña")
                txtContraI.UseSystemPasswordChar = false;
        }

        private void Ocultar_Cont_MouseClick(object sender, MouseEventArgs e)
        {
            mostrarContraseña = false;
            Mostrar_Cont.BringToFront();
            if (txtContraI.Text != "Ingrese la contraseña")
                txtContraI.UseSystemPasswordChar = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var Recuperar_Contraseña = new Recuperar_Contraseña_Formulario();
            Recuperar_Contraseña.ShowDialog();
        }

        private void Panel_Iniciar_sesion_Paint(object sender, PaintEventArgs e)
        {
            int radio = 30;
            Panel_Iniciar_sesion.Region = new Region(GetRoundedRectPath(Panel_Iniciar_sesion.ClientRectangle, radio));
        }

      
        private void lbl_Mensaje_de_Error_Login_Click(object sender, EventArgs e)
        {

        }
    }
}