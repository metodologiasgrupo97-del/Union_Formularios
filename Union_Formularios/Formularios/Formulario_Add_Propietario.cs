using Datos_Acceso.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Add_Propietario : Form
    {
        public Formulario_Add_Propietario()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!CamposCompletos())
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var registro = new Registro_Datos_Propietario();

            if (btnGuardar.Text == "Actualizar")
            {
                // Lógica de actualización (si deseas implementarla después)
                MessageBox.Show("Actualización aún no implementada.");
                return;
            }

            if (registro.CedulaExiste(txtCedula.Text))
            {
                MessageBox.Show("La cédula ingresada ya existe. No se puede duplicar.");
                return;
            }

            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                string query = @"INSERT INTO Propietarios 
                (Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro)
                VALUES 
                (@Cedula, @Nombre, @Apellido, @Telefono, @Correo, @Direccion, @Estado, @FechaRegistro)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Cedula", txtCedula.Text);
                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                cmd.Parameters.AddWithValue("@Estado", cmbEstado.Text);
                cmd.Parameters.AddWithValue("@FechaRegistro", dtpFechaRegistro.Value);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Propietario registrado exitosamente.");
                this.Close();
            }
        }


        private bool CamposCompletos()
        {
            return !string.IsNullOrWhiteSpace(txtCedula.Text) &&
                   !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                   !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                   !string.IsNullOrWhiteSpace(txtTelefono.Text) &&
                   !string.IsNullOrWhiteSpace(txtCorreo.Text) &&
                   !string.IsNullOrWhiteSpace(txtDireccion.Text) &&
                   cmbEstado.SelectedIndex != -1;
        }

    }
}
