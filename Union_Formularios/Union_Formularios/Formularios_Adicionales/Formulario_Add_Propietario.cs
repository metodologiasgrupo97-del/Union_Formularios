using Datos_Acceso.SqlServer;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Add_Propietario : Form
    {
        private int? _propietarioId = null;
        private bool IsEditMode => _propietarioId.HasValue;

        public Formulario_Add_Propietario()
        {
            InitializeComponent();
            btnGuardar.Text = "Guardar";
            ConfigurarDatePicker();
            PrepararComboEstado();
        }

        public Formulario_Add_Propietario(int propietarioId) : this()
        {
            ConfigurarComoEdicion(propietarioId);
        }

        private void PrepararComboEstado()
        {
            if (cmbEstado.Items.Count == 0)
            {
                cmbEstado.Items.Add("Activo");
                cmbEstado.Items.Add("Inactivo");
            }
        }

        public void ConfigurarComoEdicion(int propietarioId)
        {
            _propietarioId = propietarioId;
            btnGuardar.Text = "Actualizar";
            CargarPropietario(propietarioId);

            txtCedula.ReadOnly = false;
            txtCedula.TabStop = true;
            dtpFechaRegistro.Enabled = true;
        }

        private void CargarPropietario(int id)
        {
            try
            {
                using (SqlConnection cn = Conexion_SQL.OpenConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ID_Propietario, Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro
                    FROM dbo.Propietarios
                    WHERE ID_Propietario = @Id;", cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("No se encontró el propietario.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            DialogResult = DialogResult.Cancel;
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
                        cmbEstado.SelectedIndex = (estado == "Activo") ? 0 :
                                                  (estado == "Inactivo") ? 1 : -1;

                        if (DateTime.TryParse(rd["FechaRegistro"]?.ToString(), out var fr))
                            dtpFechaRegistro.Value = fr;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar propietario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validar y normalizar datos
            if (!ValidarYNormalizar(out var cedula, out var nombre, out var apellido,
                                     out var telefono, out var correo, out var direccion,
                                     out var estado, out var fechaReg))
                return;

            if (!IsEditMode)
            {
                // INSERT
                if (CedulaExiste(cedula))
                {
                    MessageBox.Show("La cédula ingresada ya existe. No se puede duplicar.", "Duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection cn = Conexion_SQL.OpenConnection())
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO dbo.Propietarios
                            (Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro)
                        VALUES
                            (@Cedula, @Nombre, @Apellido, @Telefono, @Correo, @Direccion, @Estado, @FechaRegistro);", cn))
                    {
                        cmd.Parameters.AddWithValue("@Cedula", cedula);
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Apellido", apellido);
                        cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Direccion", (object)direccion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@FechaRegistro", fechaReg);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Propietario registrado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Ya existe un propietario con esa cédula.", "Duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al registrar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // UPDATE (permitiendo cambiar Cédula y FechaRegistro)
                if (!_propietarioId.HasValue)
                {
                    MessageBox.Show("ID de propietario no válido para actualización.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (CedulaExisteExceptoId(cedula, _propietarioId.Value))
                {
                    MessageBox.Show("La cédula ingresada ya existe en otro propietario.", "Duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection cn = Conexion_SQL.OpenConnection())
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE dbo.Propietarios
                           SET Cedula       = @Cedula,
                               Nombre       = @Nombre,
                               Apellido     = @Apellido,
                               Telefono     = @Telefono,
                               Correo       = @Correo,
                               Direccion    = @Direccion,
                               Estado       = @Estado,
                               FechaRegistro= @FechaRegistro
                         WHERE ID_Propietario = @Id;", cn))
                    {
                        cmd.Parameters.AddWithValue("@Cedula", cedula);
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Apellido", apellido);
                        cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Correo", (object)correo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Direccion", (object)direccion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", estado);
                        cmd.Parameters.AddWithValue("@FechaRegistro", fechaReg);
                        cmd.Parameters.AddWithValue("@Id", _propietarioId.Value);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Propietario actualizado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar (verifique el ID).", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Ya existe un propietario con esa cédula.", "Duplicado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidarYNormalizar(
            out string cedula, out string nombre, out string apellido,
            out string telefono, out string correo, out string direccion,
            out string estado, out DateTime fechaRegistro)
        {
            cedula = txtCedula.Text?.Trim();
            nombre = txtNombre.Text?.Trim();
            apellido = txtApellido.Text?.Trim();
            telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : LimpiarTelefono(txtTelefono.Text);
            correo = string.IsNullOrWhiteSpace(txtCorreo.Text) ? null : txtCorreo.Text.Trim();
            direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim();
            estado = cmbEstado.Text?.Trim();
            fechaRegistro = dtpFechaRegistro.Value.Date;

            if (string.IsNullOrWhiteSpace(cedula) || !Regex.IsMatch(cedula, @"^\d{10,13}$"))
            {
                MessageBox.Show("Ingrese una cédula válida (solo dígitos, 10–13).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCedula.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Ingrese el apellido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtApellido.Focus();
                return false;
            }

            if (telefono != null && !Regex.IsMatch(telefono, @"^\+?\d{7,15}$"))
            {
                MessageBox.Show("El teléfono debe tener solo dígitos (y opcional +), 7–15 caracteres.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTelefono.Focus();
                return false;
            }

            if (correo != null && !Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Ingrese un correo válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtCorreo.Focus();
                return false;
            }

            if (estado != "Activo" && estado != "Inactivo")
            {
                MessageBox.Show("Seleccione un estado válido (Activo/Inactivo).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbEstado.Focus();
                return false;
            }

            if (fechaRegistro.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de registro no puede ser futura.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dtpFechaRegistro.Focus();
                return false;
            }

            return true;
        }

        private string LimpiarTelefono(string raw)
        {
            // Permite + solo al inicio, luego dígitos
            var s = raw.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (s.StartsWith("+"))
                return "+" + Regex.Replace(s.Substring(1), @"\D", "");
            return Regex.Replace(s, @"\D", "");
        }

        private void ConfigurarDatePicker()
        {
            dtpFechaRegistro.Format = DateTimePickerFormat.Short;
            dtpFechaRegistro.MaxDate = DateTime.Today;
            dtpFechaRegistro.Value = DateTime.Today;
        }

        private bool CedulaExiste(string cedula)
        {
            try
            {
                using (SqlConnection cn = Conexion_SQL.OpenConnection())
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT 1 FROM dbo.Propietarios WHERE Cedula = @Cedula;", cn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);
                    using (var rd = cmd.ExecuteReader())
                        return rd.Read();
                }
            }
            catch
            {
                return true;
            }
        }

        private bool CedulaExisteExceptoId(string cedula, int id)
        {
            try
            {
                using (SqlConnection cn = Conexion_SQL.OpenConnection())
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT 1 
                      FROM dbo.Propietarios 
                      WHERE Cedula = @Cedula AND ID_Propietario <> @Id;", cn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var rd = cmd.ExecuteReader())
                        return rd.Read();
                }
            }
            catch
            {
                return true;
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}
