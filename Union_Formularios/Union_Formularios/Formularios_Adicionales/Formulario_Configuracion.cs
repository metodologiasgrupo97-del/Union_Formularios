using Datos_Acceso.SqlServer;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Formulario_Principal_Car_EFULL.Formularios; 
using Guna.UI2.WinForms;                          


namespace Union_Formularios.Formularios
{
    public partial class Formulario_Configuracion : Form
    {
        public event Action<UserHeaderData> PerfilActualizado;  
        private Form Sub_Formualrio;                            
        private Panel panelEscritorio;                        
        private bool _eventsWired;                             

        private int _userId = 0;               
        private byte[] _fotoBytes = null;      
        private const string ADMIN_POSITION = "Administrador";

        public Formulario_Configuracion(Panel panelContenedor, int userId = 0)
        {
            InitializeComponent();
            panelEscritorio = panelContenedor;
            _userId = userId;

            img_Perfil.SizeMode = PictureBoxSizeMode.Zoom;
            WireNavButtonsOnce();  
            WireProfileEvents();   
        }

        public Formulario_Configuracion()
        {
            InitializeComponent();
            img_Perfil.SizeMode = PictureBoxSizeMode.Zoom;
            WireNavButtonsOnce();
            WireProfileEvents();
        }

        private void WireNavButtonsOnce()
        {
            if (_eventsWired) return;
            HookClickByName("edt_dash_config", edt_dash_config_Click);
            HookClickByName("add_trabajador_config", add_trabajador_config_Click);
            HookClickByName("edt_configproduct_config", config_products_config_Click);
            _eventsWired = true;
        }

        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;
            ctrl.Click -= handler; 
            ctrl.Click += handler;
        }

        private void edt_dash_config_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_EdtDash(panelEscritorio));
        }
        private void add_trabajador_config_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Add_Users(panelEscritorio));
        }
        private void config_products_config_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Config_Productos());
        }

        private void OpenOrEmbed(Form form)
        {
            try
            {
                if (panelEscritorio != null && !panelEscritorio.IsDisposed)
                {
                    var existente = panelEscritorio.Controls.OfType<Form>()
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

            if (panelEscritorio != null)
            {
                panelEscritorio.Controls.Clear();
                panelEscritorio.Controls.Add(form);
                panelEscritorio.Tag = form;
            }

            form.BringToFront();
            form.Show();
        }
        private void WireProfileEvents()
        {
            this.Shown -= Formulario_Configuracion_Shown;
            this.Shown += Formulario_Configuracion_Shown;

            btn_Cargar_Foto_Perfil.Click -= btn_Cargar_Foto_Perfil_Click;
            btn_Cargar_Foto_Perfil.Click += btn_Cargar_Foto_Perfil_Click;

            btn_Guardar_Act_Perfil.Click -= btn_Guardar_Act_Perfil_Click;
            btn_Guardar_Act_Perfil.Click += btn_Guardar_Act_Perfil_Click;

            txt_Edit_telf_Profile.KeyPress -= txt_Edit_telf_Profile_KeyPress;
            txt_Edit_telf_Profile.KeyPress += txt_Edit_telf_Profile_KeyPress;
        }

        private void Formulario_Configuracion_Shown(object sender, EventArgs e)
        {
            try { CargarPerfil(); }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando perfil: " + ex.Message, "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPerfil()
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();

                if (_userId == 0)
                {
                    using (var cmdId = new SqlCommand(
                        "SELECT TOP 1 UserID FROM Users WHERE Position=@p ORDER BY UserID", cn))
                    {
                        cmdId.Parameters.AddWithValue("@p", ADMIN_POSITION);
                        var r = cmdId.ExecuteScalar();
                        if (r == null) throw new Exception("No se encontró un usuario Administrador.");
                        _userId = Convert.ToInt32(r);
                    }
                }

                string sql = @"SELECT UserID, FirstName, LastName, LoginName, Email, Telefono, FotoPerfil
                               FROM Users WHERE UserID = @id";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", _userId);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) throw new Exception("Usuario no encontrado.");

                        txt_Edit_Nom_Profile.Text = rd["FirstName"]?.ToString();
                        txt_Edit_LastName_Profile.Text = rd["LastName"]?.ToString();
                        txt_Edit_Usu_Profile.Text = rd["LoginName"]?.ToString();
                        txt_Edit_Cor_Profile.Text = rd["Email"]?.ToString();
                        txt_Edit_telf_Profile.Text = rd["Telefono"]?.ToString();
                        txt_Edit_Pass_Profile.Clear(); 

                        // Foto en imagen o null
                        if (rd["FotoPerfil"] != DBNull.Value)
                        {
                            var buffer = (byte[])rd["FotoPerfil"];
                            _fotoBytes = buffer;
                            img_Perfil.Image = BytesToImage(buffer);
                        }
                        else
                        {
                            _fotoBytes = null;
                            img_Perfil.Image = null;
                        }
                    }
                }
            }
        }

        private void btn_Cargar_Foto_Perfil_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()
            {
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Selecciona una foto de perfil"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd.FileName);
                    img_Perfil.Image = img;
                    _fotoBytes = ImageToBytes(img);
                }
            }
        }

        private void btn_Guardar_Act_Perfil_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarInputs()) return;

                if (!PasswordCorrecta(_userId, txt_Edit_Pass_Profile.Text))
                {
                    MessageBox.Show("Contraseña incorrecta. No se guardaron cambios.", "Perfil",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (LoginNameExisteEnOtro(txt_Edit_Usu_Profile.Text.Trim(), _userId))
                {
                    MessageBox.Show("El nombre de usuario ya existe. Elige otro.", "Perfil",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                {
                    cn.Open();
                    using (var tx = cn.BeginTransaction())
                    {
                        string sql = @"UPDATE Users SET
                                         FirstName = @n,
                                         LastName  = @a,
                                         LoginName = @u,
                                         Email     = @e,
                                         Telefono  = @t" +
                                      (_fotoBytes != null ? ", FotoPerfil = @f" : "") +
                                      " WHERE UserID = @id;";

                        using (var cmd = new SqlCommand(sql, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", txt_Edit_Nom_Profile.Text.Trim());
                            cmd.Parameters.AddWithValue("@a", txt_Edit_LastName_Profile.Text.Trim());
                            cmd.Parameters.AddWithValue("@u", txt_Edit_Usu_Profile.Text.Trim());
                            cmd.Parameters.AddWithValue("@e", txt_Edit_Cor_Profile.Text.Trim());
                            cmd.Parameters.AddWithValue("@t", txt_Edit_telf_Profile.Text.Trim());
                            if (_fotoBytes != null)
                                cmd.Parameters.Add("@f", SqlDbType.VarBinary, _fotoBytes.Length).Value = _fotoBytes;
                            cmd.Parameters.AddWithValue("@id", _userId);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();

                        PerfilActualizado?.Invoke(new UserHeaderData
                        {
                            UserID = _userId,
                            FirstName = txt_Edit_Nom_Profile.Text.Trim(),
                            LastName = txt_Edit_LastName_Profile.Text.Trim(),
                            LoginName = txt_Edit_Usu_Profile.Text.Trim(),
                            Email = txt_Edit_Cor_Profile.Text.Trim(),
                            Telefono = txt_Edit_telf_Profile.Text.Trim(),
                            FotoPerfil = _fotoBytes
                        });
                    }
                }

                MessageBox.Show("Perfil actualizado correctamente.", "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_Edit_Pass_Profile.Clear(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el perfil:\n" + ex.Message, "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarInputs()
        {
            if (string.IsNullOrWhiteSpace(txt_Edit_Nom_Profile.Text) ||
                string.IsNullOrWhiteSpace(txt_Edit_LastName_Profile.Text) ||
                string.IsNullOrWhiteSpace(txt_Edit_Usu_Profile.Text) ||
                string.IsNullOrWhiteSpace(txt_Edit_Cor_Profile.Text) ||
                string.IsNullOrWhiteSpace(txt_Edit_telf_Profile.Text))
            {
                MessageBox.Show("Completa todos los campos requeridos.", "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!EsEmailValido(txt_Edit_Cor_Profile.Text.Trim()))
            {
                MessageBox.Show("Correo no válido.", "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            var phone = new string(txt_Edit_telf_Profile.Text.Where(char.IsDigit).ToArray());
            if (phone.Length < 9 || phone.Length > 10)
            {
                MessageBox.Show("Número de teléfono inválido (9–10 dígitos).", "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txt_Edit_Pass_Profile.Text))
            {
                MessageBox.Show("Confirma con tu contraseña para guardar.", "Perfil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool EsEmailValido(string email) =>
            Regex.IsMatch(email, @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
                RegexOptions.IgnoreCase);

        private bool PasswordCorrecta(int userId, string passIngresada)
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                using (var cmd = new SqlCommand("SELECT Password FROM Users WHERE UserID = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    var dbPass = Convert.ToString(cmd.ExecuteScalar() ?? "");
                    return string.Equals(dbPass, passIngresada, StringComparison.Ordinal); 
                }
            }
        }

        private bool LoginNameExisteEnOtro(string login, int userId)
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                string sql = "SELECT COUNT(1) FROM Users WHERE LoginName = @u AND UserID <> @id";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@u", login);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private void txt_Edit_telf_Profile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true; 
        }

        private static Image BytesToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            using (var ms = new MemoryStream(bytes))
            using (var tmp = Image.FromStream(ms, true, true))
                return new Bitmap(tmp);
        }

        private static byte[] ImageToBytes(Image img)
        {
            if (img == null) return null;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }
        public class UserHeaderData
        {
            public int UserID { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string LoginName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Telefono { get; set; } = "";
            public byte[] FotoPerfil { get; set; }
        }

        private void txt_Edit_Nom_Profile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txt_Edit_LastName_Profile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txt_Edit_Usu_Profile_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}