using Dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Add_Users : Form
    {
        // ===== 1) CAMPOS =====
        private Panel panelEscritorio;   // Panel contenedor donde se insertan subformularios
        private Form subFormulario;      // Referencia al subformulario actual
        private byte[] imagenEnBytes;    // Imagen del usuario convertida a arreglo de bytes

        // ===== 2) CONSTRUCTOR =====
        // Recibe el panel de escritorio para poder abrir subformularios dentro de él.
        public Formulario_Add_Users(Panel panel)
        {
            InitializeComponent();
            this.panelEscritorio = panel;
        }

        // ===== 3) HANDLERS PRINCIPALES =====

        // Botón para subir foto: abre un diálogo de archivos, carga imagen en PictureBox
        // y la convierte a arreglo de bytes para guardarla en BD.
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

        // Botón Registrar: valida campos, prepara datos y llama al modelo para registrar usuario.
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string usuario = txt_Edit_Nom_Usu.Text;
            string contraseña = txt_Edit_Pass_Profile.Text;
            string nombre = txt_Edit_Name_Profile.Text;
            string apellido = txt_Edit_LastName_Profile.Text;
            string correo = txt_Cor_Edit.Text;
            string puesto = Cmbox_Select_Plaque.SelectedItem?.ToString();
            string telefono = txt_Add_telf_User.Text.Trim();

            // === Validaciones ===
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

            // === Registro ===
            Modelo_Registro_Usuario modelo = new Modelo_Registro_Usuario();
            string resultado = modelo.RegistrarTrabajador(usuario, contraseña, nombre, apellido, puesto, correo, telefono, imagenEnBytes);
            MessageBox.Show(resultado, "Resultado del registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== 4) NAVEGACIÓN ENTRE FORMULARIOS =====
        // Abre un subformulario dentro del panel de escritorio (usado en botones de navegación).
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

        // Botón para abrir formulario de dashboard.
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Abrir_Sub_Formulario(new Formulario_EdtDash(panelEscritorio));
            this.Close();
        }

        // Botón para abrir formulario de configuración.
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Abrir_Sub_Formulario(new Formulario_Configuracion(panelEscritorio));
            this.Close();
        }

        // ===== 5) VALIDACIONES DE ENTRADA =====
        // Restringe campo de teléfono a solo dígitos.
        private void txt_Add_telf_User_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // ===== 6) MOSTRAR / OCULTAR TELÉFONO SECUNDARIO =====
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

        // ===== 7) EVENTOS VACÍOS (generados por diseñador) =====
        private void txt_Add_telf_User_TextChanged(object sender, EventArgs e)
        {
            // Actualmente vacío: puede eliminarse si no está enlazado en el diseñador.
        }
    }
}
