using System;
using Dominio;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Datos_Acceso.SqlServer; 
using Union_Formularios.Formularios;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_Vehiculos : Form
    {
        // ===== 1) CAMPOS =====
        private int? _tipoIdSel, _marcaIdSel, _modeloIdSel;
        private int? _vehicleId = null;
        private bool IsEditMode => _vehicleId.HasValue;

        public int idPropietarioSeleccionado = 0;
        private bool _suspendEvents = false;

        // ===== 2) CONSTRUCTOR =====
        public Formulario_Vehiculos()
        {
            InitializeComponent();
        }

        // ===== 3) LOAD =====
        private void FormVehiculo_Load(object sender, EventArgs e)
        {
            // Combos: solo lista
            txt_Prop_Asig.ReadOnly = true;
            txt_Placa.CharacterCasing = CharacterCasing.Upper;
            txt_Num_Motor.CharacterCasing = CharacterCasing.Upper;
            Cmbox_Tip_Vehic.DropDownStyle = ComboBoxStyle.DropDownList;
            Cmbox_Marca.DropDownStyle = ComboBoxStyle.DropDownList;
            Cmbox_Modelo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_Año.DropDownStyle = ComboBoxStyle.DropDownList;

            // Datos fijos
            Cmbox_Combustible.Items.Clear();
            Cmbox_Combustible.Items.AddRange(new[] { "Gasolina", "Diésel", "Eléctrico", "Híbrido" });
            Cmbox_Estado.Items.Clear();
            Cmbox_Estado.Items.AddRange(new[] { "Activo", "Inactivo" });
            txt_Prop_Asig.ReadOnly = true;

            SetInitialState();
            CargarTiposDesdeBD();

            // Si no están conectados en el diseñador:
            txt_Placa.TextChanged += txt_Placa_TextChanged;
            Cmbox_Tip_Vehic.SelectedIndexChanged += Cmbox_Tip_Vehic_SelectedIndexChanged;
            Cmbox_Marca.SelectedIndexChanged += Cmbox_Marca_SelectedIndexChanged;
            Cmbox_Modelo.SelectedIndexChanged += Cmbox_Modelo_SelectedIndexChanged;
            cmb_Año.SelectedIndexChanged += cmb_Año_SelectedIndexChanged;
        }

        // ===== 4) ESTADO INICIAL =====
        private void SetInitialState()
        {
            _suspendEvents = true;

            // Placa limpia y editable
            txt_Placa.Clear();
            txt_Placa.Enabled = true;
            txt_Placa.ReadOnly = false;

            // Campos de texto
            txt_Num_Motor.Clear();
            txt_Num_Chasis.Clear();
            txt_Color.Clear();
            txt_Kilometraje.Clear();
            txt_Prop_Asig.Clear();

            // Combos fijos (solo deselección)
            Cmbox_Combustible.SelectedIndex = -1;
            Cmbox_Estado.SelectedIndex = -1;

            // Combos dependientes: vacíos y deshabilitados
            Cmbox_Tip_Vehic.SelectedIndex = -1;
            ResetCombo(Cmbox_Marca);
            ResetCombo(Cmbox_Modelo);
            ResetCombo(cmb_Año);
            Cmbox_Tip_Vehic.Enabled = false;
            Cmbox_Marca.Enabled = false;
            Cmbox_Modelo.Enabled = false;
            cmb_Año.Enabled = false;

            // Campos finales deshabilitados
            SetFinalFieldsEnabled(false);

            // Estado interno
            idPropietarioSeleccionado = 0;
            _tipoIdSel = _marcaIdSel = _modeloIdSel = null;
            _vehicleId = null;

            // Botones
            btn_Guardar_Vehiculo.Text = "Guardar";
            btn_Editar_Vehiculo.Enabled = true;

            _suspendEvents = false;
        }

        private void SetFinalFieldsEnabled(bool enabled)
        {
            txt_Num_Motor.Enabled = enabled;
            txt_Num_Chasis.Enabled = enabled;
            txt_Color.Enabled = enabled;
            Cmbox_Combustible.Enabled = enabled;
            txt_Kilometraje.Enabled = enabled;
            Cmbox_Estado.Enabled = enabled;
        }
        private void ResetCombo(ComboBox cmb)
        {
            _suspendEvents = true;
            cmb.BeginUpdate();
            cmb.DataSource = null;
            cmb.Items.Clear();
            cmb.SelectedIndex = -1;
            cmb.Text = string.Empty;
            cmb.EndUpdate();
            _suspendEvents = false;
        }

        // ===== 5) CARGA DE CATÁLOGOS =====
        private void CargarTiposDesdeBD()
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var da = new SqlDataAdapter(
                    "SELECT TipoID, Nombre FROM dbo.TipoVehiculo ORDER BY Nombre", cn))
                {
                    var t = new DataTable();
                    da.Fill(t);

                    _suspendEvents = true;
                    Cmbox_Tip_Vehic.DisplayMember = "Nombre";
                    Cmbox_Tip_Vehic.ValueMember = "TipoID";
                    Cmbox_Tip_Vehic.DataSource = t;
                    Cmbox_Tip_Vehic.SelectedIndex = -1;
                    _suspendEvents = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Tipos: " + ex.Message);
            }
        }

        private void CargarMarcasDesdeBD(int tipoId)
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var da = new SqlDataAdapter(
                    "SELECT MarcaID, Nombre FROM dbo.MarcaVehiculo WHERE TipoID=@t ORDER BY Nombre", cn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@t", tipoId);
                    var t = new DataTable();
                    da.Fill(t);

                    _suspendEvents = true;
                    Cmbox_Marca.DisplayMember = "Nombre";
                    Cmbox_Marca.ValueMember = "MarcaID";
                    Cmbox_Marca.DataSource = t;
                    Cmbox_Marca.SelectedIndex = -1;
                    _suspendEvents = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Marcas: " + ex.Message);
            }
        }

        private void CargarModelosDesdeBD(int marcaId)
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var da = new SqlDataAdapter(
                    "SELECT ModeloID, Nombre FROM dbo.ModeloVehiculo WHERE MarcaID=@m ORDER BY Nombre", cn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@m", marcaId);
                    var t = new DataTable();
                    da.Fill(t);

                    _suspendEvents = true;
                    Cmbox_Modelo.DisplayMember = "Nombre";
                    Cmbox_Modelo.ValueMember = "ModeloID";
                    Cmbox_Modelo.DataSource = t;
                    Cmbox_Modelo.SelectedIndex = -1;
                    _suspendEvents = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Modelos: " + ex.Message);
            }
        }

        private void CargarAniosDesdeBD(int modeloId)
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var da = new SqlDataAdapter(
                    "SELECT ModeloAnioID, Anio FROM dbo.ModeloAnio WHERE ModeloID=@mod ORDER BY Anio DESC", cn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@mod", modeloId);
                    var t = new DataTable();
                    da.Fill(t);

                    _suspendEvents = true;
                    cmb_Año.DisplayMember = "Anio";
                    cmb_Año.ValueMember = "ModeloAnioID";
                    cmb_Año.DataSource = t;
                    cmb_Año.SelectedIndex = -1;
                    _suspendEvents = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Años: " + ex.Message);
            }
        }

        // ===== 6) HELPERS: Resolver IDs por NOMBRE =====
        private int? GetTipoIdByNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            using (var cn = Conexion_SQL.GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT TipoID FROM dbo.TipoVehiculo WHERE Nombre=@n", cn))
                {
                    cmd.Parameters.AddWithValue("@n", nombre.Trim());
                    var r = cmd.ExecuteScalar();
                    return r == null ? (int?)null : Convert.ToInt32(r);
                }
            }
        }

        private int? GetMarcaIdByNombre(string nombre, int tipoId)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            using (var cn = Conexion_SQL.GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT MarcaID FROM dbo.MarcaVehiculo WHERE Nombre=@n AND TipoID=@t", cn))
                {
                    cmd.Parameters.AddWithValue("@n", nombre.Trim());
                    cmd.Parameters.AddWithValue("@t", tipoId);
                    var r = cmd.ExecuteScalar();
                    return r == null ? (int?)null : Convert.ToInt32(r);
                }
            }
        }

        private int? GetModeloIdByNombre(string nombre, int marcaId)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            using (var cn = Conexion_SQL.GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT ModeloID FROM dbo.ModeloVehiculo WHERE Nombre=@n AND MarcaID=@m", cn))
                {
                    cmd.Parameters.AddWithValue("@n", nombre.Trim());
                    cmd.Parameters.AddWithValue("@m", marcaId);
                    var r = cmd.ExecuteScalar();
                    return r == null ? (int?)null : Convert.ToInt32(r);
                }
            }
        }

        private int? GetModeloAnioIdByAnio(int anio, int modeloId)
        {
            using (var cn = Conexion_SQL.GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT ModeloAnioID FROM dbo.ModeloAnio WHERE Anio=@a AND ModeloID=@mo", cn))
                {
                    cmd.Parameters.AddWithValue("@a", anio);
                    cmd.Parameters.AddWithValue("@mo", modeloId);
                    var r = cmd.ExecuteScalar();
                    return r == null ? (int?)null : Convert.ToInt32(r);
                }
            }
        }

        private void SelectByTextIfNeeded(ComboBox cmb, string displayText)
        {
            if (string.IsNullOrWhiteSpace(displayText)) { cmb.SelectedIndex = -1; return; }
            int idx = cmb.FindStringExact(displayText.Trim());
            if (idx >= 0) cmb.SelectedIndex = idx;
        }

        // ===== 7) UI HANDLERS =====
        private void txt_Placa_TextChanged(object sender, EventArgs e)
        {
            if (_suspendEvents || IsEditMode) return;

            bool ok = Regex.IsMatch(txt_Placa.Text.Trim(), @"^[A-Z]{3}-\d{4}$");

            if (ok)
            {
                if (!Cmbox_Tip_Vehic.Enabled) Cmbox_Tip_Vehic.Enabled = true;
            }
            else
            {
                if (Cmbox_Tip_Vehic.Enabled) Cmbox_Tip_Vehic.Enabled = false;

                _suspendEvents = true;
                Cmbox_Tip_Vehic.SelectedIndex = -1;

                ResetCombo(Cmbox_Marca); Cmbox_Marca.Enabled = false;
                ResetCombo(Cmbox_Modelo); Cmbox_Modelo.Enabled = false;
                ResetCombo(cmb_Año); cmb_Año.Enabled = false;

                SetFinalFieldsEnabled(false);
                _suspendEvents = false;
            }
        }

        private void Cmbox_Tip_Vehic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspendEvents) return;

            if (Cmbox_Tip_Vehic.SelectedItem == null)
            {
                _tipoIdSel = null;
                ResetCombo(Cmbox_Marca); Cmbox_Marca.Enabled = false;
                ResetCombo(Cmbox_Modelo); Cmbox_Modelo.Enabled = false;
                ResetCombo(cmb_Año); cmb_Año.Enabled = false;
                SetFinalFieldsEnabled(false);
                return;
            }

            if (Cmbox_Tip_Vehic.SelectedItem is DataRowView drvT && drvT.Row.Table.Columns.Contains("TipoID"))
                _tipoIdSel = Convert.ToInt32(drvT["TipoID"]);
            else
                _tipoIdSel = null;

            if (_tipoIdSel.HasValue)
            {
                CargarMarcasDesdeBD(_tipoIdSel.Value);
                Cmbox_Marca.Enabled = true;
            }
            else
            {
                ResetCombo(Cmbox_Marca);
                Cmbox_Marca.Enabled = false;
            }

            ResetCombo(Cmbox_Modelo); Cmbox_Modelo.Enabled = false;
            ResetCombo(cmb_Año); cmb_Año.Enabled = false;
            SetFinalFieldsEnabled(false);
        }

        private void Cmbox_Marca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspendEvents) return;

            if (Cmbox_Marca.SelectedItem == null)
            {
                _marcaIdSel = null;
                ResetCombo(Cmbox_Modelo); Cmbox_Modelo.Enabled = false;
                ResetCombo(cmb_Año); cmb_Año.Enabled = false;
                SetFinalFieldsEnabled(false);
                return;
            }

            if (Cmbox_Marca.SelectedItem is DataRowView drvM && drvM.Row.Table.Columns.Contains("MarcaID"))
                _marcaIdSel = Convert.ToInt32(drvM["MarcaID"]);
            else
                _marcaIdSel = null;

            if (_marcaIdSel.HasValue)
            {
                CargarModelosDesdeBD(_marcaIdSel.Value);
                Cmbox_Modelo.Enabled = true;
            }
            else
            {
                ResetCombo(Cmbox_Modelo);
                Cmbox_Modelo.Enabled = false;
            }

            ResetCombo(cmb_Año); cmb_Año.Enabled = false;
            SetFinalFieldsEnabled(false);
        }

        private void Cmbox_Modelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspendEvents) return;

            if (Cmbox_Modelo.SelectedItem == null)
            {
                _modeloIdSel = null;
                ResetCombo(cmb_Año); cmb_Año.Enabled = false;
                SetFinalFieldsEnabled(false);
                return;
            }

            if (Cmbox_Modelo.SelectedItem is DataRowView drvMo && drvMo.Row.Table.Columns.Contains("ModeloID"))
                _modeloIdSel = Convert.ToInt32(drvMo["ModeloID"]);
            else
                _modeloIdSel = null;

            if (_modeloIdSel.HasValue)
            {
                CargarAniosDesdeBD(_modeloIdSel.Value);
                cmb_Año.Enabled = true;
            }
            else
            {
                ResetCombo(cmb_Año);
                cmb_Año.Enabled = false;
            }

            SetFinalFieldsEnabled(false);
        }

        private void cmb_Año_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspendEvents) return;
            SetFinalFieldsEnabled(cmb_Año.SelectedItem != null);
        }

        // ===== 8) GUARDAR (INSERT / UPDATE) =====
        private void btn_Guardar_Vehiculo_Click(object sender, EventArgs e)
        {
            try
            {
                string placa = (txt_Placa.Text ?? "").Trim().ToUpper().Replace(" ", "").Replace("_", "");
                if (Regex.IsMatch(placa, @"^[A-Z]{3}\d{4}$"))
                    placa = placa.Substring(0, 3) + "-" + placa.Substring(3);

                if (!Regex.IsMatch(placa, @"^[A-Z]{3}-\d{4}$")) { MessageBox.Show("Placa inválida (AAA-1234)."); return; }

                if (string.IsNullOrWhiteSpace(Cmbox_Tip_Vehic.Text)) { MessageBox.Show("Seleccione Tipo."); return; }
                if (string.IsNullOrWhiteSpace(Cmbox_Marca.Text)) { MessageBox.Show("Seleccione Marca."); return; }
                if (string.IsNullOrWhiteSpace(Cmbox_Modelo.Text)) { MessageBox.Show("Seleccione Modelo."); return; }
                if (string.IsNullOrWhiteSpace(cmb_Año.Text)) { MessageBox.Show("Seleccione Año."); return; }
                if (idPropietarioSeleccionado <= 0) { MessageBox.Show("Seleccione un propietario."); return; }
                if (string.IsNullOrWhiteSpace(txt_Num_Motor.Text)) { MessageBox.Show("Ingrese N° Motor."); return; }
                if (string.IsNullOrWhiteSpace(txt_Num_Chasis.Text)) { MessageBox.Show("Ingrese N° Chasis."); return; }
                if (string.IsNullOrWhiteSpace(txt_Color.Text)) { MessageBox.Show("Ingrese Color."); return; }
                if (string.IsNullOrWhiteSpace(Cmbox_Combustible.Text)) { MessageBox.Show("Seleccione Combustible."); return; }
                if (!int.TryParse(cmb_Año.Text.Trim(), out int anio)) { MessageBox.Show("Año inválido."); return; }
                if (!int.TryParse(txt_Kilometraje.Text.Trim(), out int km) || km < 0) { MessageBox.Show("Kilometraje inválido."); return; }
                if (string.IsNullOrWhiteSpace(Cmbox_Estado.Text)) { MessageBox.Show("Seleccione Estado."); return; }

                string tipo = Cmbox_Tip_Vehic.Text.Trim();
                string marca = Cmbox_Marca.Text.Trim();
                string modelo = Cmbox_Modelo.Text.Trim();
                string motor = txt_Num_Motor.Text.Trim().ToUpper();
                string chasis = txt_Num_Chasis.Text.Trim().ToUpper();
                string color = txt_Color.Text.Trim();
                string combus = Cmbox_Combustible.Text.Trim();
                string estado = Cmbox_Estado.Text.Trim();

                using (var cn = Conexion_SQL.GetConnection())
                {
                    cn.Open();

                    if (!IsEditMode)
                    {
                        using (var ch = new SqlCommand("SELECT COUNT(1) FROM dbo.Vehiculos WHERE Placa=@p;", cn))
                        {
                            ch.Parameters.AddWithValue("@p", placa);
                            if (Convert.ToInt32(ch.ExecuteScalar()) > 0)
                            { MessageBox.Show("Ya existe un vehículo con esa placa."); return; }
                        }

                        const string sqlIns = @"
                    INSERT INTO dbo.Vehiculos
                    (Placa, Tipo, Marca, Modelo, Anio, NumeroMotor, NumeroChasis,
                     Color, Combustible, Kilometraje, Estado, ID_Propietario)
                    VALUES
                    (@Placa, @Tipo, @Marca, @Modelo, @Anio, @Motor, @Chasis,
                     @Color, @Combustible, @Kilometraje, @Estado, @Prop);";

                        using (var cmd = new SqlCommand(sqlIns, cn))
                        {
                            cmd.Parameters.AddWithValue("@Placa", placa);
                            cmd.Parameters.AddWithValue("@Tipo", tipo);
                            cmd.Parameters.AddWithValue("@Marca", marca);
                            cmd.Parameters.AddWithValue("@Modelo", modelo);
                            cmd.Parameters.AddWithValue("@Anio", anio);
                            cmd.Parameters.AddWithValue("@Motor", motor);
                            cmd.Parameters.AddWithValue("@Chasis", chasis);
                            cmd.Parameters.AddWithValue("@Color", color);
                            cmd.Parameters.AddWithValue("@Combustible", combus);
                            cmd.Parameters.AddWithValue("@Kilometraje", km);
                            cmd.Parameters.AddWithValue("@Estado", estado);
                            cmd.Parameters.AddWithValue("@Prop", idPropietarioSeleccionado);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Vehículo registrado correctamente.", "Vehículos",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Limpiar_Campos();
                    }
                    else
                    {
                        const string sqlUpd = @"
                    UPDATE dbo.Vehiculos SET 
                        Tipo=@Tipo,
                        Marca=@Marca,
                        Modelo=@Modelo,
                        Anio=@Anio,
                        NumeroMotor=@Motor,
                        NumeroChasis=@Chasis,
                        Color=@Color,
                        Combustible=@Combustible,
                        Kilometraje=@Kilometraje,
                        Estado=@Estado,
                        ID_Propietario=@Prop
                    WHERE VehicleID=@ID;";

                        using (var cmd = new SqlCommand(sqlUpd, cn))
                        {
                            cmd.Parameters.AddWithValue("@ID", _vehicleId.Value);
                            cmd.Parameters.AddWithValue("@Tipo", tipo);
                            cmd.Parameters.AddWithValue("@Marca", marca);
                            cmd.Parameters.AddWithValue("@Modelo", modelo);
                            cmd.Parameters.AddWithValue("@Anio", anio);
                            cmd.Parameters.AddWithValue("@Motor", motor);
                            cmd.Parameters.AddWithValue("@Chasis", chasis);
                            cmd.Parameters.AddWithValue("@Color", color);
                            cmd.Parameters.AddWithValue("@Combustible", combus);
                            cmd.Parameters.AddWithValue("@Kilometraje", km);
                            cmd.Parameters.AddWithValue("@Estado", estado);
                            cmd.Parameters.AddWithValue("@Prop", idPropietarioSeleccionado);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Vehículo actualizado correctamente.", "Vehículos",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Limpiar_Campos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar/actualizar vehículo:\n" + ex.Message,
                    "BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== 9) BOTONES =====
        private void btnSeleccionarPropietario_Click(object sender, EventArgs e)
        {
            var frm = new Formulario_dgv_De_Propietarios { formPadre = this };
            frm.ShowDialog();
        }

        private void btn_Editar_Vehiculo_Click(object sender, EventArgs e)
        {
            using (var sel = new Formulario_Seleccionar_Vehiculo())
            {
                sel.VehiculoSeleccionado += v => ConfigurarComoEdicion(v.VehicleID);
                sel.ShowDialog(this);
            }
        }

        // ===== 10) EDICIÓN =====
        public void ConfigurarComoEdicion(int vehicleId)
        {
            _vehicleId = vehicleId;
            btn_Guardar_Vehiculo.Text = "Actualizar";
            btn_Editar_Vehiculo.Enabled = false;

            CargarVehiculo(vehicleId);

            txt_Placa.ReadOnly = true;
            txt_Placa.Enabled = false;
        }

        private void Limpiar_Campos()
        {
            _suspendEvents = true;
            SetInitialState();     // limpia todo
            CargarTiposDesdeBD();  // repuebla solo “Tipo”
            _suspendEvents = false;

            txt_Placa.Focus();
        }

        private void CargarVehiculo(int id)
        {
            try
            {
                string tipoN = null, marcaN = null, modeloN = null;
                int anio = 0;

                using (var cn = Conexion_SQL.GetConnection())
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(@"
                SELECT V.VehicleID, V.Placa,
                       V.Tipo, V.Marca, V.Modelo, V.Anio,
                       V.NumeroMotor, V.NumeroChasis, V.Color, V.Combustible,
                       V.Kilometraje, V.Estado AS EstadoVehiculo,
                       V.ID_Propietario AS ID_PropietarioVehiculo,
                       (P.Nombre + ' ' + ISNULL(P.Apellido,'')) AS Propietario
                FROM dbo.Vehiculos V
                LEFT JOIN dbo.Propietarios P ON P.ID_Propietario = V.ID_Propietario
                WHERE V.VehicleID = @id;", cn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("No se encontró el vehículo.", "Aviso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            _vehicleId = id;

                            txt_Placa.Text = (rd["Placa"]?.ToString() ?? "").ToUpper();
                            txt_Placa.ReadOnly = true;
                            txt_Placa.Enabled = false;

                            tipoN = rd["Tipo"]?.ToString();
                            marcaN = rd["Marca"]?.ToString();
                            modeloN = rd["Modelo"]?.ToString();
                            int.TryParse(rd["Anio"]?.ToString(), out anio);

                            txt_Num_Motor.Text = rd["NumeroMotor"]?.ToString();
                            txt_Num_Chasis.Text = rd["NumeroChasis"]?.ToString();
                            txt_Color.Text = rd["Color"]?.ToString();
                            Cmbox_Combustible.Text = rd["Combustible"]?.ToString();
                            txt_Kilometraje.Text = (rd["Kilometraje"] ?? 0).ToString();
                            Cmbox_Estado.Text = rd["EstadoVehiculo"]?.ToString();

                            txt_Prop_Asig.Text = rd["Propietario"]?.ToString();
                            idPropietarioSeleccionado =
                                rd["ID_PropietarioVehiculo"] == DBNull.Value ? 0
                                : Convert.ToInt32(rd["ID_PropietarioVehiculo"]);
                        }
                    }
                }

                // ==== poblar combos según catálogos, resolviendo IDs por nombre (helpers) ====
                _suspendEvents = true;

                // Tipo
                CargarTiposDesdeBD();
                int? tipoId = GetTipoIdByNombre(tipoN);
                if (tipoId.HasValue) Cmbox_Tip_Vehic.SelectedValue = tipoId.Value;
                else
                {
                    // si el catálogo no tiene ese tipo, lo agregamos temporalmente
                    if (Cmbox_Tip_Vehic.FindStringExact(tipoN ?? "") < 0 && !string.IsNullOrWhiteSpace(tipoN))
                        Cmbox_Tip_Vehic.Items.Add(tipoN);
                    Cmbox_Tip_Vehic.SelectedItem = tipoN;
                }
                Cmbox_Tip_Vehic.Enabled = true;

                // Marca (por Tipo)
                if (tipoId.HasValue) CargarMarcasDesdeBD(tipoId.Value); else ResetCombo(Cmbox_Marca);
                int? marcaId = tipoId.HasValue ? GetMarcaIdByNombre(marcaN, tipoId.Value) : null;
                if (marcaId.HasValue) Cmbox_Marca.SelectedValue = marcaId.Value;
                else
                {
                    if (Cmbox_Marca.FindStringExact(marcaN ?? "") < 0 && !string.IsNullOrWhiteSpace(marcaN))
                        Cmbox_Marca.Items.Add(marcaN);
                    Cmbox_Marca.SelectedItem = marcaN;
                }
                Cmbox_Marca.Enabled = true;

                // Modelo (por Marca)
                if (marcaId.HasValue) CargarModelosDesdeBD(marcaId.Value); else ResetCombo(Cmbox_Modelo);
                int? modeloId = marcaId.HasValue ? GetModeloIdByNombre(modeloN, marcaId.Value) : null;
                if (modeloId.HasValue) Cmbox_Modelo.SelectedValue = modeloId.Value;
                else
                {
                    if (Cmbox_Modelo.FindStringExact(modeloN ?? "") < 0 && !string.IsNullOrWhiteSpace(modeloN))
                        Cmbox_Modelo.Items.Add(modeloN);
                    Cmbox_Modelo.SelectedItem = modeloN;
                }
                Cmbox_Modelo.Enabled = true;

                // Año (por Modelo)
                if (modeloId.HasValue) CargarAniosDesdeBD(modeloId.Value); else ResetCombo(cmb_Año);
                int? modeloAnioId = modeloId.HasValue ? GetModeloAnioIdByAnio(anio, modeloId.Value) : null;
                if (modeloAnioId.HasValue) cmb_Año.SelectedValue = modeloAnioId.Value;
                else
                {
                    string anioStr = anio == 0 ? "" : anio.ToString();
                    if (cmb_Año.FindStringExact(anioStr) < 0 && !string.IsNullOrWhiteSpace(anioStr))
                        cmb_Año.Items.Add(anioStr);
                    cmb_Año.SelectedItem = anioStr;
                }
                cmb_Año.Enabled = true;

                _suspendEvents = false;

                SetFinalFieldsEnabled(true);
                btn_Guardar_Vehiculo.Text = "Actualizar";
                btn_Editar_Vehiculo.Enabled = false;
            }
            catch (Exception ex)
            {
                _suspendEvents = false;
                MessageBox.Show("Error al cargar vehículo: " + ex.Message,
                    "Vehículos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ===== 11) VALIDACIONES DE TECLADO =====
        private void txt_Kilometraje_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txt_Num_Motor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != ' ')
                e.Handled = true;
        }

        private void txt_Num_Chasis_TextChanged(object sender, EventArgs e)
        {
            txt_Num_Chasis.CharacterCasing = CharacterCasing.Upper;
        }

        private void txt_Color_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        public void EstablecerPropietario(int id, string nombreCompleto)
        {
            idPropietarioSeleccionado = id;
            txt_Prop_Asig.Text = nombreCompleto ?? string.Empty;
        }

        private void txt_Color_Leave(object sender, EventArgs e)
        {
            if (txt_Color.Text.Length > 0)
            {
                int cursorPos = txt_Color.SelectionStart;
                string texto = txt_Color.Text.Trim().ToLower();
                txt_Color.Text = char.ToUpper(texto[0]) + texto.Substring(1);
                txt_Color.SelectionStart = cursorPos;
            }
        }

        // ---- Eventos autogenerados vacíos (si existen en el diseñador) ----
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void Formulario_Reportes_Load(object sender, EventArgs e) { }
        private void guna2Button3_Click(object sender, EventArgs e) { }

        /// ////////////////


        private Panel panel1;
        private Guna.UI2.WinForms.Guna2Button btn_Guardar_Vehiculo;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Guna.UI2.WinForms.Guna2TextBox txt_Kilometraje;
        private Label label11;
        private Label label12;
        private Guna.UI2.WinForms.Guna2TextBox txt_Color;
        private Label label5;
        private Guna.UI2.WinForms.Guna2TextBox txt_Num_Chasis;
        private Label label6;
        private Label label3;
        private Label label4;
        private Guna.UI2.WinForms.Guna2TextBox txt_Num_Motor;
        private Label label2;
        private Label label1;
        private Label label13;
        private Guna.UI2.WinForms.Guna2Button btn_Editar_Vehiculo;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Marca;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Estado;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Tip_Vehic;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Modelo;
        private Guna.UI2.WinForms.Guna2Button btn_Seleccionar_Propietario;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Combustible;
        private Guna.UI2.WinForms.Guna2TextBox txt_Prop_Asig;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_Año;
        private Guna.UI2.WinForms.Guna2TextBox txt_Placa;
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.btn_Editar_Vehiculo = new Guna.UI2.WinForms.Guna2Button();
            this.label13 = new System.Windows.Forms.Label();
            this.cmb_Año = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Prop_Asig = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Placa = new Guna.UI2.WinForms.Guna2TextBox();
            this.Cmbox_Combustible = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Seleccionar_Propietario = new Guna.UI2.WinForms.Guna2Button();
            this.Cmbox_Estado = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txt_Num_Motor = new Guna.UI2.WinForms.Guna2TextBox();
            this.Cmbox_Tip_Vehic = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Cmbox_Modelo = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Cmbox_Marca = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txt_Num_Chasis = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Guardar_Vehiculo = new Guna.UI2.WinForms.Guna2Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_Color = new Guna.UI2.WinForms.Guna2TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_Kilometraje = new Guna.UI2.WinForms.Guna2TextBox();
            this.panel1.SuspendLayout();
            this.guna2ShadowPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.guna2ShadowPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1212, 753);
            this.panel1.TabIndex = 34;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.btn_Editar_Vehiculo);
            this.guna2ShadowPanel1.Controls.Add(this.label13);
            this.guna2ShadowPanel1.Controls.Add(this.cmb_Año);
            this.guna2ShadowPanel1.Controls.Add(this.label1);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Prop_Asig);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Placa);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Combustible);
            this.guna2ShadowPanel1.Controls.Add(this.label2);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Seleccionar_Propietario);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Estado);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Num_Motor);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Tip_Vehic);
            this.guna2ShadowPanel1.Controls.Add(this.label4);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Modelo);
            this.guna2ShadowPanel1.Controls.Add(this.label3);
            this.guna2ShadowPanel1.Controls.Add(this.label6);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Marca);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Num_Chasis);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Guardar_Vehiculo);
            this.guna2ShadowPanel1.Controls.Add(this.label5);
            this.guna2ShadowPanel1.Controls.Add(this.label7);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Color);
            this.guna2ShadowPanel1.Controls.Add(this.label8);
            this.guna2ShadowPanel1.Controls.Add(this.label12);
            this.guna2ShadowPanel1.Controls.Add(this.label9);
            this.guna2ShadowPanel1.Controls.Add(this.label11);
            this.guna2ShadowPanel1.Controls.Add(this.label10);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Kilometraje);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(40, 42);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(1134, 676);
            this.guna2ShadowPanel1.TabIndex = 71;
            // 
            // btn_Editar_Vehiculo
            // 
            this.btn_Editar_Vehiculo.Animated = true;
            this.btn_Editar_Vehiculo.BorderRadius = 8;
            this.btn_Editar_Vehiculo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Editar_Vehiculo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Editar_Vehiculo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Editar_Vehiculo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Editar_Vehiculo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(208)))), ((int)(((byte)(117)))));
            this.btn_Editar_Vehiculo.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Editar_Vehiculo.ForeColor = System.Drawing.Color.White;
            this.btn_Editar_Vehiculo.Image = global::Union_Formularios.Properties.Resources.edit_icon1;
            this.btn_Editar_Vehiculo.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Editar_Vehiculo.ImageOffset = new System.Drawing.Point(8, 0);
            this.btn_Editar_Vehiculo.ImageSize = new System.Drawing.Size(28, 28);
            this.btn_Editar_Vehiculo.Location = new System.Drawing.Point(374, 570);
            this.btn_Editar_Vehiculo.Name = "btn_Editar_Vehiculo";
            this.btn_Editar_Vehiculo.ShadowDecoration.BorderRadius = 14;
            this.btn_Editar_Vehiculo.Size = new System.Drawing.Size(180, 45);
            this.btn_Editar_Vehiculo.TabIndex = 73;
            this.btn_Editar_Vehiculo.Text = "Editar";
            this.btn_Editar_Vehiculo.Click += new System.EventHandler(this.btn_Editar_Vehiculo_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(377, 35);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(356, 38);
            this.label13.TabIndex = 72;
            this.label13.Text = "Ingresar datos de vehículos";
            // 
            // cmb_Año
            // 
            this.cmb_Año.BackColor = System.Drawing.Color.Transparent;
            this.cmb_Año.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_Año.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Año.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Año.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Año.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_Año.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_Año.IntegralHeight = false;
            this.cmb_Año.ItemHeight = 30;
            this.cmb_Año.Location = new System.Drawing.Point(160, 248);
            this.cmb_Año.MaxDropDownItems = 5;
            this.cmb_Año.Name = "cmb_Año";
            this.cmb_Año.Size = new System.Drawing.Size(202, 36);
            this.cmb_Año.TabIndex = 71;
            this.cmb_Año.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(157, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 25);
            this.label1.TabIndex = 36;
            this.label1.Text = "*Placa";
            // 
            // txt_Prop_Asig
            // 
            this.txt_Prop_Asig.Animated = true;
            this.txt_Prop_Asig.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Prop_Asig.DefaultText = "";
            this.txt_Prop_Asig.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Prop_Asig.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Prop_Asig.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Prop_Asig.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Prop_Asig.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Prop_Asig.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Prop_Asig.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Prop_Asig.Location = new System.Drawing.Point(760, 443);
            this.txt_Prop_Asig.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Prop_Asig.Name = "txt_Prop_Asig";
            this.txt_Prop_Asig.PasswordChar = '\0';
            this.txt_Prop_Asig.PlaceholderText = "";
            this.txt_Prop_Asig.SelectedText = "";
            this.txt_Prop_Asig.Size = new System.Drawing.Size(200, 36);
            this.txt_Prop_Asig.TabIndex = 70;
            // 
            // txt_Placa
            // 
            this.txt_Placa.Animated = true;
            this.txt_Placa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Placa.DefaultText = "";
            this.txt_Placa.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Placa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Placa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Placa.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Placa.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Placa.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Placa.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Placa.Location = new System.Drawing.Point(162, 153);
            this.txt_Placa.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Placa.Name = "txt_Placa";
            this.txt_Placa.PasswordChar = '\0';
            this.txt_Placa.PlaceholderText = "ABC-1234";
            this.txt_Placa.SelectedText = "";
            this.txt_Placa.Size = new System.Drawing.Size(200, 36);
            this.txt_Placa.TabIndex = 34;
            this.txt_Placa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt_Placa.TextChanged += new System.EventHandler(this.txt_Placa_TextChanged);
            // 
            // Cmbox_Combustible
            // 
            this.Cmbox_Combustible.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Combustible.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Combustible.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Combustible.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Combustible.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Combustible.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Combustible.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Combustible.ItemHeight = 30;
            this.Cmbox_Combustible.Location = new System.Drawing.Point(762, 343);
            this.Cmbox_Combustible.Name = "Cmbox_Combustible";
            this.Cmbox_Combustible.Size = new System.Drawing.Size(202, 36);
            this.Cmbox_Combustible.TabIndex = 67;
            this.Cmbox_Combustible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cmbox_Combustible.SelectedIndexChanged += new System.EventHandler(this.Cmbox_Combustible_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(453, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 25);
            this.label2.TabIndex = 37;
            this.label2.Text = "*Marca";
            // 
            // btn_Seleccionar_Propietario
            // 
            this.btn_Seleccionar_Propietario.Animated = true;
            this.btn_Seleccionar_Propietario.BorderRadius = 8;
            this.btn_Seleccionar_Propietario.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Seleccionar_Propietario.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Seleccionar_Propietario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Seleccionar_Propietario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Seleccionar_Propietario.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(82)))), ((int)(((byte)(186)))));
            this.btn_Seleccionar_Propietario.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Seleccionar_Propietario.ForeColor = System.Drawing.Color.White;
            this.btn_Seleccionar_Propietario.Image = global::Union_Formularios.Properties.Resources.icon_Add_Usu1;
            this.btn_Seleccionar_Propietario.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Seleccionar_Propietario.ImageOffset = new System.Drawing.Point(5, -2);
            this.btn_Seleccionar_Propietario.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Seleccionar_Propietario.Location = new System.Drawing.Point(761, 490);
            this.btn_Seleccionar_Propietario.Name = "btn_Seleccionar_Propietario";
            this.btn_Seleccionar_Propietario.ShadowDecoration.BorderRadius = 14;
            this.btn_Seleccionar_Propietario.Size = new System.Drawing.Size(199, 36);
            this.btn_Seleccionar_Propietario.TabIndex = 66;
            this.btn_Seleccionar_Propietario.Text = "    Seleccionar";
            this.btn_Seleccionar_Propietario.Click += new System.EventHandler(this.btnSeleccionarPropietario_Click);
            // 
            // Cmbox_Estado
            // 
            this.Cmbox_Estado.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Estado.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Estado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Estado.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Estado.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Estado.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Estado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Estado.ItemHeight = 30;
            this.Cmbox_Estado.Location = new System.Drawing.Point(456, 443);
            this.Cmbox_Estado.Name = "Cmbox_Estado";
            this.Cmbox_Estado.Size = new System.Drawing.Size(202, 36);
            this.Cmbox_Estado.TabIndex = 65;
            this.Cmbox_Estado.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt_Num_Motor
            // 
            this.txt_Num_Motor.Animated = true;
            this.txt_Num_Motor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Num_Motor.DefaultText = "";
            this.txt_Num_Motor.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Num_Motor.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Num_Motor.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Num_Motor.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Num_Motor.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Num_Motor.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Num_Motor.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Num_Motor.Location = new System.Drawing.Point(458, 248);
            this.txt_Num_Motor.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Num_Motor.Name = "txt_Num_Motor";
            this.txt_Num_Motor.PasswordChar = '\0';
            this.txt_Num_Motor.PlaceholderText = "";
            this.txt_Num_Motor.SelectedText = "";
            this.txt_Num_Motor.Size = new System.Drawing.Size(200, 36);
            this.txt_Num_Motor.TabIndex = 39;
            this.txt_Num_Motor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt_Num_Motor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Num_Motor_KeyPress);
            // 
            // Cmbox_Tip_Vehic
            // 
            this.Cmbox_Tip_Vehic.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Tip_Vehic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Tip_Vehic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Tip_Vehic.Enabled = false;
            this.Cmbox_Tip_Vehic.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Tip_Vehic.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Tip_Vehic.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Tip_Vehic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Tip_Vehic.ItemHeight = 30;
            this.Cmbox_Tip_Vehic.Location = new System.Drawing.Point(162, 343);
            this.Cmbox_Tip_Vehic.Name = "Cmbox_Tip_Vehic";
            this.Cmbox_Tip_Vehic.Size = new System.Drawing.Size(202, 36);
            this.Cmbox_Tip_Vehic.TabIndex = 64;
            this.Cmbox_Tip_Vehic.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cmbox_Tip_Vehic.SelectedIndexChanged += new System.EventHandler(this.Cmbox_Tip_Vehic_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(157, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 25);
            this.label4.TabIndex = 40;
            this.label4.Text = "*Año de Fabricación";
            // 
            // Cmbox_Modelo
            // 
            this.Cmbox_Modelo.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Modelo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Modelo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Modelo.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Modelo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Modelo.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Modelo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Modelo.ItemHeight = 30;
            this.Cmbox_Modelo.Location = new System.Drawing.Point(761, 153);
            this.Cmbox_Modelo.Name = "Cmbox_Modelo";
            this.Cmbox_Modelo.Size = new System.Drawing.Size(202, 36);
            this.Cmbox_Modelo.TabIndex = 63;
            this.Cmbox_Modelo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cmbox_Modelo.SelectedIndexChanged += new System.EventHandler(this.Cmbox_Modelo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(453, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 25);
            this.label3.TabIndex = 41;
            this.label3.Text = "*Número de motor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(756, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 25);
            this.label6.TabIndex = 43;
            this.label6.Text = "*Modelo";
            // 
            // Cmbox_Marca
            // 
            this.Cmbox_Marca.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Marca.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Marca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Marca.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Marca.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Marca.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Marca.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Marca.ItemHeight = 30;
            this.Cmbox_Marca.Location = new System.Drawing.Point(458, 153);
            this.Cmbox_Marca.Name = "Cmbox_Marca";
            this.Cmbox_Marca.Size = new System.Drawing.Size(202, 36);
            this.Cmbox_Marca.TabIndex = 61;
            this.Cmbox_Marca.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cmbox_Marca.SelectedIndexChanged += new System.EventHandler(this.Cmbox_Marca_SelectedIndexChanged);
            // 
            // txt_Num_Chasis
            // 
            this.txt_Num_Chasis.Animated = true;
            this.txt_Num_Chasis.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Num_Chasis.DefaultText = "";
            this.txt_Num_Chasis.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Num_Chasis.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Num_Chasis.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Num_Chasis.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Num_Chasis.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Num_Chasis.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Num_Chasis.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Num_Chasis.Location = new System.Drawing.Point(761, 248);
            this.txt_Num_Chasis.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Num_Chasis.MaxLength = 17;
            this.txt_Num_Chasis.Name = "txt_Num_Chasis";
            this.txt_Num_Chasis.PasswordChar = '\0';
            this.txt_Num_Chasis.PlaceholderText = "";
            this.txt_Num_Chasis.SelectedText = "";
            this.txt_Num_Chasis.Size = new System.Drawing.Size(200, 36);
            this.txt_Num_Chasis.TabIndex = 44;
            this.txt_Num_Chasis.TextChanged += new System.EventHandler(this.txt_Num_Chasis_TextChanged);
            // 
            // btn_Guardar_Vehiculo
            // 
            this.btn_Guardar_Vehiculo.Animated = true;
            this.btn_Guardar_Vehiculo.BorderRadius = 8;
            this.btn_Guardar_Vehiculo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Guardar_Vehiculo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Guardar_Vehiculo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Guardar_Vehiculo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Guardar_Vehiculo.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Guardar_Vehiculo.ForeColor = System.Drawing.Color.White;
            this.btn_Guardar_Vehiculo.Image = global::Union_Formularios.Properties.Resources.icon_download;
            this.btn_Guardar_Vehiculo.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Guardar_Vehiculo.ImageOffset = new System.Drawing.Point(8, -1);
            this.btn_Guardar_Vehiculo.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Guardar_Vehiculo.Location = new System.Drawing.Point(580, 570);
            this.btn_Guardar_Vehiculo.Name = "btn_Guardar_Vehiculo";
            this.btn_Guardar_Vehiculo.ShadowDecoration.BorderRadius = 14;
            this.btn_Guardar_Vehiculo.Size = new System.Drawing.Size(180, 45);
            this.btn_Guardar_Vehiculo.TabIndex = 60;
            this.btn_Guardar_Vehiculo.Text = "   Guardar";
            this.btn_Guardar_Vehiculo.Click += new System.EventHandler(this.btn_Guardar_Vehiculo_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(756, 220);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(213, 25);
            this.label5.TabIndex = 45;
            this.label5.Text = "*Número de chasis (VIN)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(756, 415);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(193, 25);
            this.label7.TabIndex = 57;
            this.label7.Text = "*Propietario asignado";
            // 
            // txt_Color
            // 
            this.txt_Color.Animated = true;
            this.txt_Color.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Color.DefaultText = "";
            this.txt_Color.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Color.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Color.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Color.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Color.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Color.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Color.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Color.Location = new System.Drawing.Point(458, 343);
            this.txt_Color.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Color.Name = "txt_Color";
            this.txt_Color.PasswordChar = '\0';
            this.txt_Color.PlaceholderText = "";
            this.txt_Color.SelectedText = "";
            this.txt_Color.Size = new System.Drawing.Size(200, 36);
            this.txt_Color.TabIndex = 47;
            this.txt_Color.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Color_KeyPress);
            this.txt_Color.Leave += new System.EventHandler(this.txt_Color_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(756, 315);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 25);
            this.label8.TabIndex = 55;
            this.label8.Text = "*Combustible";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(157, 315);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(154, 25);
            this.label12.TabIndex = 48;
            this.label12.Text = "*Tipo de vehículo";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(453, 415);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(214, 25);
            this.label9.TabIndex = 53;
            this.label9.Text = "*Estado (activo/inactivo)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(453, 315);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 25);
            this.label11.TabIndex = 49;
            this.label11.Text = "*Color";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(157, 415);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(172, 25);
            this.label10.TabIndex = 52;
            this.label10.Text = "*Kilometraje actual";
            // 
            // txt_Kilometraje
            // 
            this.txt_Kilometraje.Animated = true;
            this.txt_Kilometraje.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Kilometraje.DefaultText = "";
            this.txt_Kilometraje.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Kilometraje.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Kilometraje.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Kilometraje.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Kilometraje.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Kilometraje.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Kilometraje.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Kilometraje.Location = new System.Drawing.Point(162, 443);
            this.txt_Kilometraje.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Kilometraje.Name = "txt_Kilometraje";
            this.txt_Kilometraje.PasswordChar = '\0';
            this.txt_Kilometraje.PlaceholderText = "0 KM";
            this.txt_Kilometraje.SelectedText = "";
            this.txt_Kilometraje.Size = new System.Drawing.Size(200, 36);
            this.txt_Kilometraje.TabIndex = 50;
            this.txt_Kilometraje.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Kilometraje_KeyPress);
            // 
            // Formulario_Vehiculos
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1212, 753);
            this.Controls.Add(this.panel1);
            this.Name = "Formulario_Vehiculos";
            this.Text = "Gestión de Vehículos";
            this.Load += new System.EventHandler(this.FormVehiculo_Load);
            this.panel1.ResumeLayout(false);
            this.guna2ShadowPanel1.ResumeLayout(false);
            this.guna2ShadowPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void Cmbox_Combustible_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
