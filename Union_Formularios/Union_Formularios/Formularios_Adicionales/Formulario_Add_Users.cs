using Dominio;
using Formulario_Principal_Car_EFULL.Formularios;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Add_Users : Form
    {
        private Panel panelEscritorio;
        private Form subFormulario;
        private byte[] imagenEnBytes;
        private bool _modoEdicion = false;
        private int _editUserId = 0;

        private bool _navWired = false;

        public Formulario_Add_Users()
        {
            InitializeComponent();
            WireNavButtonsOnce();  
        }

        public Formulario_Add_Users(Panel panel)
        {
            InitializeComponent();
            this.panelEscritorio = panel;
            WireNavButtonsOnce();  
        }

        private void WireNavButtonsOnce()
        {
            if (_navWired) return;

            HookClickByName("guna2Button2", guna2Button2_Click);        
            HookClickByName("guna2Button1", guna2Button1_Click);        
            HookClickByName("btn_Edt_Usu", btn_Edt_Usu_Click);         
            HookClickByName("edt_configproducts_trabajador", edt_configproducts_trabajador_Click);

            _navWired = true;
        }

        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;

            ctrl.Click -= handler;
            ctrl.Click += handler;
        }

        private void GoDashboard()
        {
            if (panelEscritorio != null && !panelEscritorio.IsDisposed)
            {
                Abrir_Sub_Formulario(new Formulario_EdtDash(panelEscritorio));
                this.Close(); 
            }
            else
            {
                using (var f = new Formulario_EdtDash(null))
                {
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.ShowDialog(this);
                }
            }
        }

        private void GoConfiguracion()
        {
            if (panelEscritorio != null && !panelEscritorio.IsDisposed)
            {
                Abrir_Sub_Formulario(new Formulario_Configuracion(panelEscritorio));
                this.Close();
            }
            else
            {
                using (var f = new Formulario_Configuracion())
                {
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.ShowDialog(this);
                }
            }
        }
        private void GoConfigProductos()
        {
            if (panelEscritorio != null && !panelEscritorio.IsDisposed)
            {
                Abrir_Sub_Formulario(new Formulario_Config_Productos());
                this.Close();
            }
            else
            {
                using (var f = new Formulario_Config_Productos())
                {
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.ShowDialog(this);
                }
            }
        }
        private void OpenSelectorEditarUsuario()
        {
            var f = new Formulario_EdtUsu();
            f.StartPosition = FormStartPosition.CenterParent;
            f.UsuarioSeleccionado += d =>
            {
                _modoEdicion = true;
                _editUserId = d.Id;

                txt_Edit_Nom_Usu.Text = d.Usuario ?? "";
                txt_Edit_Pass_Profile.Text = d.Contrasena ?? "";
                txt_Edit_Name_Profile.Text = d.Nombre ?? "";
                txt_Edit_LastName_Profile.Text = d.Apellido ?? "";
                txt_Cor_Edit.Text = d.Correo ?? "";

                if (d.Foto != null) pbFoto.Image = (Image)d.Foto.Clone();

                const string cargoDeseado = "Trabajadores";
                bool existe = false;
                foreach (var item in Cmbox_Select_Plaque.Items)
                    if (string.Equals(Convert.ToString(item), cargoDeseado, StringComparison.OrdinalIgnoreCase))
                    { existe = true; break; }
                if (!existe) Cmbox_Select_Plaque.Items.Add(cargoDeseado);
                Cmbox_Select_Plaque.SelectedItem = cargoDeseado;
            };
            f.ShowDialog(this);
        }

        private void edt_configproducts_trabajador_Click(object sender, EventArgs e)
        {
            GoConfigProductos();
        }


        private void guna2Button2_Click(object sender, EventArgs e) => GoDashboard();
        private void guna2Button1_Click(object sender, EventArgs e) => GoConfiguracion();
        private void btn_Edt_Usu_Click(object sender, EventArgs e) => OpenSelectorEditarUsuario();

        public void CargarEdicion(int id, string usuario, string contrasena, string nombre, string apellido, string correo, Image foto)
        {
            txt_Edit_Nom_Usu.Text = usuario ?? "";
            txt_Edit_Pass_Profile.Text = contrasena ?? "";
            txt_Edit_Name_Profile.Text = nombre ?? "";
            txt_Edit_LastName_Profile.Text = apellido ?? "";
            txt_Cor_Edit.Text = correo ?? "";

            if (foto != null) pbFoto.Image = (Image)foto.Clone();

            const string cargoDeseado = "Trabajadores";
            bool existe = false;
            foreach (var item in Cmbox_Select_Plaque.Items)
                if (string.Equals(Convert.ToString(item), cargoDeseado, StringComparison.OrdinalIgnoreCase))
                { existe = true; break; }
            if (!existe) Cmbox_Select_Plaque.Items.Add(cargoDeseado);
            Cmbox_Select_Plaque.SelectedItem = cargoDeseado;
        }

        private void btnSubirFoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Archivos de imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pbFoto.Image = Image.FromFile(dialog.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    pbFoto.Image.Save(ms, pbFoto.Image.RawFormat);
                    imagenEnBytes = ms.ToArray();
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string usuario = txt_Edit_Nom_Usu.Text;
            string contraseña = txt_Edit_Pass_Profile.Text;
            string nombre = txt_Edit_Name_Profile.Text;
            string apellido = txt_Edit_LastName_Profile.Text;
            string correo = txt_Cor_Edit.Text;
            string puesto = Cmbox_Select_Plaque.SelectedItem?.ToString();
            string telefono = txt_Add_telf_User.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario))
            {
                MessageBox.Show("Por favor ingrese un nombre de usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Edit_Nom_Usu.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Por favor ingrese una contraseña.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Edit_Pass_Profile.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Por favor ingrese el nombre del usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Edit_Name_Profile.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Por favor ingrese el apellido del usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Edit_LastName_Profile.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("Por favor ingrese el correo del usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Cor_Edit.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(puesto))
            {
                MessageBox.Show("Por favor seleccione un puesto.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Cmbox_Select_Plaque.Focus();
                return;
            }
            if (imagenEnBytes == null)
            {
                MessageBox.Show("Por favor cargue una fotografía del usuario.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show("Por favor ingrese el número de teléfono.", "Campo obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Add_telf_User.Focus();
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^09\d{8}$"))
            {
                MessageBox.Show("Ingrese un número de teléfono celular válido de Ecuador (ej: 0998765432).", "Formato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Add_telf_User.Focus();
                return;
            }

            Modelo_Registro_Usuario modelo = new Modelo_Registro_Usuario();
            string resultado = modelo.RegistrarTrabajador(usuario, contraseña, nombre, apellido, puesto, correo, telefono, imagenEnBytes);
            MessageBox.Show(resultado, "Resultado del registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Abrir_Sub_Formulario(Form form)
        {
            if (subFormulario != null)
                subFormulario.Close();

            subFormulario = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelEscritorio.Controls.Add(form);
            panelEscritorio.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void txt_Add_telf_User_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            btn_Mostrar_numtelf_secundario.BringToFront();
            lbl_numtelf_secund.Visible = false;
            lbl_num_secun_593.Visible = false;
            txt_numtelf_secun.Visible = false;
            txt_numtelf_secun.Clear();
        }

        private void btn_Mostrar_numtelf_secundario_Click(object sender, EventArgs e)
        {
            btn_No_mostrar_numtelf_secundario.BringToFront();
            lbl_numtelf_secund.Visible = true;
            lbl_num_secun_593.Visible = true;
            txt_numtelf_secun.Visible = true;
        }

        private void txt_Add_telf_User_TextChanged(object sender, EventArgs e) { }

        private void txt_Edit_Name_Profile_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txt_numtelf_secun_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}
