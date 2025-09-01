using Datos_Acceso.SqlServer;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Add_Propietario : Form
    {
        // null = alta; valor = edición
        private int? _propietarioId = null;
        private bool IsEditMode => _propietarioId.HasValue;

        // === Constructor: Modo ALTA ===
        public Formulario_Add_Propietario()
        {
            InitializeComponent();
            btnGuardar.Text = "Guardar";
        }

        // === Constructor: Modo EDICIÓN ===
        public Formulario_Add_Propietario(int propietarioId) : this()
        {
            ConfigurarComoEdicion(propietarioId);
        }

        // Alternativa si quieres abrir con ctor vacío y luego configurar
        public void ConfigurarComoEdicion(int propietarioId)
        {
            _propietarioId = propietarioId;
            btnGuardar.Text = "Actualizar";

            CargarPropietario(propietarioId);

            // La cédula NO se edita en modo actualización
            txtCedula.ReadOnly = true;
            txtCedula.TabStop = false;
        }

        private void CargarPropietario(int id)
        {
            try
            {
                using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                {
                    cn.Open();
                    string sql = @"SELECT ID_Propietario, Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro
                                   FROM Propietarios
                                   WHERE ID_Propietario = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("No se encontró el propietario.", "Aviso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Close();
                                return;
                            }

                            txtCedula.Text = rd["Cedula"]?.ToString();
                            txtNombre.Text = rd["Nombre"]?.ToString();
                            txtApellido.Text = rd["Apellido"]?.ToString();
                            txtTelefono.Text = rd["Telefono"]?.ToString();
                            txtCorreo.Text = rd["Correo"]?.ToString();
                            txtDireccion.Text = rd["Direccion"]?.ToString();

                            var estado = rd["Estado"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(estado))
                            {
                                int idx = cmbEstado.FindStringExact(estado);
                                cmbEstado.SelectedIndex = idx >= 0 ? idx : -1;
                            }
                            else
                            {
                                cmbEstado.SelectedIndex = -1;
                            }

                            if (DateTime.TryParse(rd["FechaRegistro"]?.ToString(), out var fr))
                                dtpFechaRegistro.Value = fr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar propietario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!CamposCompletos())
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Faltan datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Para depurar si hace falta:
            System.Diagnostics.Debug.WriteLine($"[Formulario_Add_Propietario] IsEditMode={IsEditMode}, Id={_propietarioId}");

            if (!IsEditMode)
            {
                // ============== INSERT (ALTA) ==============
                // Verificar cédula duplicada SOLO en alta
                if (CedulaExiste(txtCedula.Text.Trim()))
                {
                    MessageBox.Show("La cédula ingresada ya existe. No se puede duplicar.", "Duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                    {
                        cn.Open();
                        string sql = @"INSERT INTO Propietarios 
                                       (Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro)
                                       VALUES 
                                       (@Cedula, @Nombre, @Apellido, @Telefono, @Correo, @Direccion, @Estado, @FechaRegistro)";

                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@Cedula", txtCedula.Text.Trim());
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                            cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtCorreo.Text) ? (object)DBNull.Value : txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Estado", cmbEstado.Text);
                            cmd.Parameters.AddWithValue("@FechaRegistro", dtpFechaRegistro.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Propietario registrado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al registrar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // ============== UPDATE (EDICIÓN) ==============
                if (!_propietarioId.HasValue)
                {
                    MessageBox.Show("ID de propietario no válido para actualización.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Importante: NO se actualiza la cédula ni la fecha de registro
                try
                {
                    using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                    {
                        cn.Open();
                        string sql = @"UPDATE Propietarios
                                       SET Nombre    = @Nombre,
                                           Apellido  = @Apellido,
                                           Telefono  = @Telefono,
                                           Correo    = @Correo,
                                           Direccion = @Direccion,
                                           Estado    = @Estado
                                       WHERE ID_Propietario = @Id";

                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                            cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                            cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(txtCorreo.Text) ? (object)DBNull.Value : txtCorreo.Text.Trim());
                            cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : txtDireccion.Text.Trim());
                            cmd.Parameters.AddWithValue("@Estado", cmbEstado.Text);
                            cmd.Parameters.AddWithValue("@Id", _propietarioId.Value);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                            {
                                MessageBox.Show("Propietario actualizado correctamente.", "Éxito",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar (verifique el ID).", "Aviso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool CamposCompletos()
        {
            // En edición, la cédula está readonly pero debe estar cargada.
            return !string.IsNullOrWhiteSpace(txtCedula.Text) &&
                   !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                   !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                   !string.IsNullOrWhiteSpace(txtTelefono.Text) &&
                   !string.IsNullOrWhiteSpace(txtCorreo.Text) &&
                   !string.IsNullOrWhiteSpace(txtDireccion.Text) &&
                   cmbEstado.SelectedIndex != -1;
        }

        private bool CedulaExiste(string cedula)
        {
            try
            {
                using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                {
                    cn.Open();
                    string sql = "SELECT COUNT(1) FROM Propietarios WHERE Cedula = @Cedula";
                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@Cedula", cedula);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return true;
            }
        }
    }
}
