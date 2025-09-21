using Dominio;
using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Union_Formularios
{
    public partial class Formulario_Login : Form
    {
        private int _failedCount = 0;
        private DateTime? _lockUntil = null;
        private readonly Timer _lockTimer = new Timer() { Interval = 1000 };
        bool mostrarContraseña = false;

        public Formulario_Login()
        {
            InitializeComponent();
            EmpresaConfigService.Cargar();
            AplicarTemaLogin();
            EmpresaConfigService.TemaCambiado += AplicarTemaLogin;
            _lockTimer.Tick += (s, e) =>
            {
                if (_lockUntil == null) { _lockTimer.Stop(); return; }
                var rest = _lockUntil.Value - DateTime.Now;
                if (rest <= TimeSpan.Zero)
                {
                    _lockUntil = null;
                    button2.Enabled = true;
                    button2.Text = "Iniciar sesión";
                    _lockTimer.Stop();
                    msgError("");
                    lbl_Mensaje_de_Error_Login.Visible = false;
                    Simbolo_Error.Visible = false;
                }
                else
                {
                    button2.Enabled = false;
                    button2.Text = $"Reintentar en {rest.Seconds:00}s";
                }
            };
            txtContraI.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { button2_Click(button2, EventArgs.Empty); }
                if (Control.IsKeyLocked(Keys.CapsLock))
                    msgError("Bloq Mayús activado. Podría causar errores de contraseña.");
            };
            txtUsuI.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { button2_Click(button2, EventArgs.Empty); }
            };
        }

        private void AplicarTemaLogin()
        {
            var cfg = EmpresaConfigService.EmpresaActual;
            EmpresaConfigService.AplicarColor(Panel_Iniciar_sesion, cfg.ColorPrimario);
            EmpresaConfigService.AplicarColor(pContraI, cfg.ColorPrimario);
            EmpresaConfigService.AplicarColor(pUsuarioI, cfg.ColorPrimario);
            EmpresaConfigService.AplicarColor(button2, cfg.ColorPrimario);
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
            if (txtUsuI.Text == "Ingrese el usuario")
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
        private bool ValidateInputs(out string message, out string user, out string pass)
        {
            user = (txtUsuI.Text ?? "").Trim();
            pass = (txtContraI.Text ?? "").Trim();
            if (user.Equals("Ingrese el usuario", StringComparison.OrdinalIgnoreCase)) user = "";
            if (pass.Equals("Ingrese la contraseña", StringComparison.OrdinalIgnoreCase)) pass = "";
            if (string.IsNullOrEmpty(user))
            {
                message = "Por favor ingrese el usuario.";
                txtUsuI.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(pass))
            {
                message = "Por favor ingrese la contraseña.";
                txtContraI.Focus();
                return false;
            }
            if (user.Length < 3)
            {
                message = "El usuario debe tener al menos 3 caracteres.";
                txtUsuI.Focus();
                return false;
            }
            if (pass.Length < 4)
            {
                message = "La contraseña debe tener al menos 4 caracteres.";
                txtContraI.Focus();
                return false;
            }
            message = null;
            return true;
        }
        private async Task<bool> TryLoginAsync(string user, string pass)
        {
            return await Task.Run(() =>
            {
                var usser = new Modelo_Dominio_Usuario();
                return usser.LoginUser(user, pass);
            });
        }
        private async Task ShakeAsync(Control target)
        {
            int originalX = target.Left;
            for (int i = 0; i < 6; i++)
            {
                target.Left = originalX + ((i % 2 == 0) ? 6 : -6);
                await Task.Delay(22);
            }
            target.Left = originalX;
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            if (_lockUntil != null)
            {
                msgError("Demasiados intentos fallidos. Espere para volver a intentar.");
                return;
            }
            if (!ValidateInputs(out string msg, out string user, out string pass))
            {
                msgError(msg);
                await ShakeAsync(Panel_Iniciar_sesion);
                return;
            }
            if (Control.IsKeyLocked(Keys.CapsLock))
                msgError("Bloq Mayús activado. Verifique su contraseña.");
            try
            {
                button2.Enabled = false;
                Cursor = Cursors.WaitCursor;
                bool ok = await TryLoginAsync(user, pass);
                if (ok)
                {
                    _failedCount = 0;
                    Cursor = Cursors.Default;
                    button2.Enabled = true;
                    var menu = new Formulario_Principal_Car_EFULL.Fr_Dashboard();
                    string nombre = Capa_Corte_Transversal.Cache.Cache_Inicio_Sesion_Usuario.FirstName;
                    string mensaje = $"😎 ¡Bienvenido, {nombre}!\n\nNos alegra tenerte de vuelta 🚀";
                    MessageBox.Show(mensaje, "Inicio de sesión exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    menu.Show();
                    menu.FormClosed += Cerrar_Sesion;
                    this.Hide();
                }
                else
                {
                    _failedCount++;
                    Cursor = Cursors.Default;
                    button2.Enabled = true;
                    msgError("Usuario o contraseña incorrectos. Inténtalo nuevamente.");
                    txtContraI.UseSystemPasswordChar = false;
                    txtContraI.Text = "Ingrese la contraseña";
                    txtContraI.ForeColor = Color.Gray;
                    txtUsuI.Focus();
                    await ShakeAsync(Panel_Iniciar_sesion);
                    if (_failedCount >= 5)
                    {
                        _lockUntil = DateTime.Now.AddSeconds(30);
                        _lockTimer.Start();
                        msgError("Has excedido el número de intentos. Espera 30 segundos.");
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                button2.Enabled = true;
                msgError("Ocurrió un error al iniciar sesión. " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void msgError(string msg)
        {
            lbl_Mensaje_de_Error_Login.Text = " " + (msg ?? "");
            bool show = !string.IsNullOrWhiteSpace(msg);
            lbl_Mensaje_de_Error_Login.Visible = show;
            Simbolo_Error.Visible = show;
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
    }
}