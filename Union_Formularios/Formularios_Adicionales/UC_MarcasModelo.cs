using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Union_Formularios.Formularios_Adicionales
{
    public partial class UC_MarcasModelos : UserControl
    {
        private int? _editMarcaId = null;
        private int? _editModeloId = null;
        private bool _isEditingMarca = false;
        private bool _isEditingModelo = false;

        public UC_MarcasModelos()
        {
            InitializeComponent();

            this.Load += UC_MarcasModelos_Load;

            Cmbox_Tip_Vehic.SelectedIndexChanged += (s, e) =>
            {
                if (_isEditingMarca) return;
                CargarGridMarcas();
            };
            Cmbox_Estado_Config_marc.SelectedIndexChanged += (s, e) =>
            {
                if (_isEditingMarca) return;
                CargarGridMarcas();
            };

            btn_Guardar_Marca.Click += btn_Guardar_Marca_Click;

            Cmbox_Tip_Vehi_Modelo.SelectedIndexChanged += (s, e) =>
            {
                if (_isEditingModelo) return;
                CargarMarcasPorTipo_ParaModelos();
                CargarGridModelos();
            };
            Cmbox_Marca_Config.SelectedIndexChanged += (s, e) =>
            {
                if (_isEditingModelo) return;
                CargarGridModelos();
            };
            Cmbox_Estado_Config_model.SelectedIndexChanged += (s, e) =>
            {
                if (_isEditingModelo) return;
                CargarGridModelos();
            };

            var btnModelo = this.Controls.Find("btn_Guardar_Modelo", true).FirstOrDefault() as Button;
            if (btnModelo != null) btnModelo.Click += btn_Guardar_Modelo_Click;

            dgv_Reg_Marcas.CellDoubleClick += dgv_Reg_Marcas_CellDoubleClick;
            dgv_Reg_Modelo.CellDoubleClick += dgv_Reg_Modelo_CellDoubleClick;

            dgv_Reg_Marcas.CellContentClick += dgv_Reg_Marcas_CellContentClick;
            dgv_Reg_Modelo.CellContentClick += dgv_Reg_Modelo_CellContentClick;

            Cmbox_Marca_Config.DropDown += (s, e) => { if (!_isEditingModelo) CargarMarcasPorTipo_ParaModelos(); };
        }

        private void UC_MarcasModelos_Load(object sender, EventArgs e)
        {
            try
            {
                InitEstadosCombo(Cmbox_Estado_Config_marc);
                InitEstadosCombo(Cmbox_Estado_Config_model);

                CargarTiposVehiculo();             
                CargarMarcasPorTipo_ParaModelos();  
                CargarAnios();                     

                CargarGridMarcas();
                CargarGridModelos();

                InicializarVacios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar maestro de marcas/modelos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            AsegurarColumnaEliminar(dgv_Reg_Marcas, false);
            AsegurarColumnaEliminar(dgv_Reg_Modelo, false);
        }

        private void InitEstadosCombo(ComboBox cb)
        {
            cb.Items.Clear();
            cb.Items.AddRange(new[] { "Activo", "Inactivo" });
        }

        private void InicializarVacios()
        {
            txt_Nom_Marc.Clear();
            txt_Nom_Model.Clear();

            Cmbox_Tip_Vehic.SelectedIndex = -1;
            Cmbox_Tip_Vehi_Modelo.SelectedIndex = -1;
            Cmbox_Marca_Config.SelectedIndex = -1;
            Cmbox_Estado_Config_marc.SelectedIndex = -1;
            Cmbox_Estado_Config_model.SelectedIndex = -1;
            Cmbox_anio_desde.SelectedIndex = -1;
            Cmbox_anio_hasta.SelectedIndex = -1;

            btn_Guardar_Marca.Text = "Guardar marca";
            var btnModelo = this.Controls.Find("btn_Guardar_Modelo", true).FirstOrDefault() as Button;
            if (btnModelo != null) btnModelo.Text = "Guardar modelo";

            _editMarcaId = null; _isEditingMarca = false;
            _editModeloId = null; _isEditingModelo = false;
        }

        private static string NormalizaNombre(string s) => (s ?? "").Trim();
        private static string NormalizaClave(string s) => (s ?? "").Trim().ToUpperInvariant();

        private void CargarTiposVehiculo()
        {
            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter("SELECT TipoID, Nombre FROM TipoVehiculo ORDER BY Nombre", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);

                // Panel MARCAS
                Cmbox_Tip_Vehic.DisplayMember = "Nombre";
                Cmbox_Tip_Vehic.ValueMember = "TipoID";
                Cmbox_Tip_Vehic.DataSource = dt.Copy(); 

                Cmbox_Tip_Vehi_Modelo.DisplayMember = "Nombre";
                Cmbox_Tip_Vehi_Modelo.ValueMember = "TipoID";
                Cmbox_Tip_Vehi_Modelo.DataSource = dt;  
            }
        }

        private void CargarMarcasPorTipo_ParaModelos()
        {
            int? tipoId = null;
            if (Cmbox_Tip_Vehi_Modelo.SelectedValue != null &&
                int.TryParse(Cmbox_Tip_Vehi_Modelo.SelectedValue.ToString(), out int t))
                tipoId = t;

            if (tipoId == null)
            {
                Cmbox_Marca_Config.DataSource = null;
                return;
            }

            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(
                "SELECT MarcaID, Nombre FROM MarcaVehiculo WHERE TipoID=@t AND Estado='Activo' ORDER BY Nombre", cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@t", tipoId.Value);
                var dt = new DataTable();
                da.Fill(dt);
                Cmbox_Marca_Config.DisplayMember = "Nombre";
                Cmbox_Marca_Config.ValueMember = "MarcaID";
                Cmbox_Marca_Config.DataSource = dt;
            }
        }

        private int? TipoSeleccionado_Marcas()
        {
            if (Cmbox_Tip_Vehic.SelectedValue == null) return null;
            if (int.TryParse(Cmbox_Tip_Vehic.SelectedValue.ToString(), out int id)) return id;
            return null;
        }
        private int? TipoSeleccionado_Modelos()
        {
            if (Cmbox_Tip_Vehi_Modelo.SelectedValue == null) return null;
            if (int.TryParse(Cmbox_Tip_Vehi_Modelo.SelectedValue.ToString(), out int id)) return id;
            return null;
        }

        private void CargarAnios()
        {
            int anioMin = 1980;
            int anioMax = DateTime.Now.Year + 1;

            Cmbox_anio_desde.Items.Clear();
            Cmbox_anio_hasta.Items.Clear();

            for (int a = anioMin; a <= anioMax; a++)
            {
                Cmbox_anio_desde.Items.Add(a);
                Cmbox_anio_hasta.Items.Add(a);
            }
        }

        private void btn_Guardar_Marca_Click(object sender, EventArgs e)
        {
            var tipoId = TipoSeleccionado_Marcas();
            string nombre = NormalizaNombre(txt_Nom_Marc.Text);
            string estado = Cmbox_Estado_Config_marc.SelectedItem?.ToString() ?? "Activo";

            // Validación de campos requeridos
            if (tipoId == null)
            {
                MessageBox.Show("Seleccione el tipo de vehículo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre de la marca.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (Cmbox_Estado_Config_marc.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el estado de la marca.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    if (ExisteMarcaDuplicada(cn, tipoId.Value, nombre, _editMarcaId))
                    {
                        MessageBox.Show("La marca ya existe para ese tipo (duplicado).",
                            "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (_editMarcaId.HasValue)
                    {
                        using (var cmd = new SqlCommand(
                            @"UPDATE MarcaVehiculo
                              SET TipoID=@t, Nombre=@n, Estado=@e
                              WHERE MarcaID=@id;", cn))
                        {
                            cmd.Parameters.AddWithValue("@id", _editMarcaId.Value);
                            cmd.Parameters.AddWithValue("@t", tipoId.Value);
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@e", estado);
                            cmd.ExecuteNonQuery();
                        }
                        _editMarcaId = null; _isEditingMarca = false;
                        btn_Guardar_Marca.Text = "Guardar marca";
                    }
                    else
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO MarcaVehiculo(TipoID, Nombre, Estado)
                              VALUES(@t, @n, @e);", cn))
                        {
                            cmd.Parameters.AddWithValue("@t", tipoId.Value);
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@e", estado);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                txt_Nom_Marc.Clear();
                CargarGridMarcas();

                MessageBox.Show("Operación de marca realizada correctamente.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sx) when (sx.Number == 2627 || sx.Number == 2601)
            {
                MessageBox.Show("Duplicado por restricción única.",
                    "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar/actualizar la marca: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ExisteMarcaDuplicada(SqlConnection cn, int tipoId, string nombre, int? excluirMarcaId)
        {
            string nUp = NormalizaClave(nombre);
            using (var cmd = new SqlCommand(@"
                SELECT CASE WHEN EXISTS 
                (
                 SELECT 1
                    FROM MarcaVehiculo
                    WHERE TipoID = @t AND UPPER(LTRIM(RTRIM(Nombre))) = @nUp AND (@id IS NULL OR MarcaID <> @id)
                ) 
                THEN 1 ELSE 0 END;", cn))
            {
                cmd.Parameters.AddWithValue("@t", tipoId);
                cmd.Parameters.AddWithValue("@nUp", nUp);
                cmd.Parameters.AddWithValue("@id", (object)excluirMarcaId ?? DBNull.Value);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        private void btn_Guardar_Modelo_Click(object sender, EventArgs e)
        {
            if (Cmbox_Marca_Config.SelectedValue == null)
            {
                MessageBox.Show("Seleccione la marca (panel modelos).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            int marcaId = Convert.ToInt32(Cmbox_Marca_Config.SelectedValue);
            string nombre = NormalizaNombre(txt_Nom_Model.Text);
            string estado = Cmbox_Estado_Config_model.SelectedItem?.ToString() ?? "Activo";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre del modelo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (Cmbox_Estado_Config_model.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el estado del modelo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (Cmbox_anio_desde.SelectedIndex == -1 || Cmbox_anio_hasta.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el rango de años.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            int anioDesde = Convert.ToInt32(Cmbox_anio_desde.SelectedItem);
            int anioHasta = Convert.ToInt32(Cmbox_anio_hasta.SelectedItem);
            if (anioDesde > anioHasta)
            {
                MessageBox.Show("El año 'desde' no puede ser mayor que 'hasta'.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    if (ExisteModeloDuplicado(cn, tx, marcaId, nombre, _editModeloId))
                    {
                        MessageBox.Show("El modelo ya existe para esa marca (duplicado).",
                            "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx.Rollback();
                        return;
                    }

                    int modeloId;

                    if (_editModeloId.HasValue)
                    {
                        using (var cmd = new SqlCommand(
                            @"UPDATE ModeloVehiculo
                              SET MarcaID=@m, Nombre=@n, Estado=@e
                              WHERE ModeloID=@id;", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", _editModeloId.Value);
                            cmd.Parameters.AddWithValue("@m", marcaId);
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@e", estado);
                            cmd.ExecuteNonQuery();
                        }
                        modeloId = _editModeloId.Value;
                    }
                    else
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO ModeloVehiculo(MarcaID, Nombre, Estado)
                              OUTPUT INSERTED.ModeloID
                              VALUES(@m, @n, @e);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@m", marcaId);
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@e", estado);
                            modeloId = (int)cmd.ExecuteScalar();
                        }
                    }

                    using (var cmd = new SqlCommand(
                        @"INSERT INTO ModeloAnio(ModeloID, Anio)
                          SELECT @ModeloID, @Anio
                          WHERE NOT EXISTS(SELECT 1 FROM ModeloAnio WHERE ModeloID=@ModeloID AND Anio=@Anio);", cn, tx))
                    {
                        cmd.Parameters.Add("@ModeloID", SqlDbType.Int).Value = modeloId;
                        var pAnio = cmd.Parameters.Add("@Anio", SqlDbType.Int);

                        for (int a = anioDesde; a <= anioHasta; a++)
                        {
                            pAnio.Value = a;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }

                _editModeloId = null; _isEditingModelo = false;
                var btnModelo = this.Controls.Find("btn_Guardar_Modelo", true).FirstOrDefault() as Button;
                if (btnModelo != null) btnModelo.Text = "Guardar modelo";
                txt_Nom_Model.Clear();

                CargarGridModelos();

                MessageBox.Show("Operación de modelo realizada correctamente.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException sx) when (sx.Number == 2627 || sx.Number == 2601)
            {
                MessageBox.Show("Duplicado por restricción única.",
                    "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar/actualizar el modelo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ExisteModeloDuplicado(SqlConnection cn, SqlTransaction tx, int marcaId, string nombre, int? excluirModeloId)
        {
            string nUp = NormalizaClave(nombre);
            using (var cmd = new SqlCommand(@"
                SELECT CASE WHEN EXISTS 
                (
                 SELECT 1
                 FROM ModeloVehiculo
                 WHERE MarcaID = @m AND UPPER(LTRIM(RTRIM(Nombre))) = @nUp AND (@id IS NULL OR ModeloID <> @id)
                ) 
                THEN 1 ELSE 0 END;", cn, tx))
            {
                cmd.Parameters.AddWithValue("@m", marcaId);
                cmd.Parameters.AddWithValue("@nUp", nUp);
                cmd.Parameters.AddWithValue("@id", (object)excluirModeloId ?? DBNull.Value);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        private void CargarGridMarcas()
        {
            var tipoId = TipoSeleccionado_Marcas();
            string estadoSel = Cmbox_Estado_Config_marc.SelectedItem?.ToString() ?? "Activo";

            string sql = @"
            SELECT 
                M.MarcaID,
                M.TipoID,
                T.Nombre AS Tipo,
                M.Nombre AS Marca,
                M.Estado
            FROM MarcaVehiculo M
            JOIN TipoVehiculo T ON T.TipoID = M.TipoID
            WHERE (@TipoID IS NULL OR M.TipoID=@TipoID) AND M.Estado = @Estado
            ORDER BY T.Nombre, M.Nombre;";

            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@TipoID", (object)tipoId ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@Estado", estadoSel);

                var dt = new DataTable();
                da.Fill(dt);

                dgv_Reg_Marcas.AutoGenerateColumns = true;
                dgv_Reg_Marcas.DataSource = dt;

                if (dgv_Reg_Marcas.Columns["MarcaID"] != null)
                {
                    var c = dgv_Reg_Marcas.Columns["MarcaID"];
                    c.Visible = true;
                    c.HeaderText = "ID Marca";
                    c.ReadOnly = true;
                    c.Width = 85;
                    c.DisplayIndex = 0;
                }
                if (dgv_Reg_Marcas.Columns["TipoID"] != null)
                    dgv_Reg_Marcas.Columns["TipoID"].Visible = false;

                dgv_Reg_Marcas.Columns["MarcaID"].SortMode = DataGridViewColumnSortMode.Automatic;

                AsegurarColumnaEliminar(dgv_Reg_Marcas, false);
            }
        }

        private void CargarGridModelos()
        {
            int? tipoId = TipoSeleccionado_Modelos(); 
            int? marcaId = null;
            if (Cmbox_Marca_Config.SelectedValue != null &&
                int.TryParse(Cmbox_Marca_Config.SelectedValue.ToString(), out int m))
                marcaId = m;

            string estadoSel = Cmbox_Estado_Config_model.SelectedItem?.ToString() ?? "Activo";

            string sql = @"
            SELECT 
                MO.ModeloID,
                MO.MarcaID,
                MA.TipoID,
                T.Nombre  AS Tipo,
                MA.Nombre AS Marca,
                MO.Nombre AS Modelo,
                MO.Estado,
                MIN(A.Anio) AS AnioDesde,
                MAX(A.Anio) AS AnioHasta,
                COUNT(A.Anio) AS CantAnios
            FROM ModeloVehiculo MO
            JOIN MarcaVehiculo MA ON MA.MarcaID = MO.MarcaID
            JOIN TipoVehiculo  T ON T.TipoID   = MA.TipoID
            LEFT JOIN ModeloAnio A ON A.ModeloID = MO.ModeloID
            WHERE (@TipoID  IS NULL OR T.TipoID   = @TipoID) AND (@MarcaID IS NULL OR MO.MarcaID = @MarcaID) AND MO.Estado = @Estado
            GROUP BY MO.ModeloID, MO.MarcaID, MA.TipoID, T.Nombre, MA.Nombre, MO.Nombre, MO.Estado
            ORDER BY T.Nombre, MA.Nombre, MO.Nombre;";

            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(sql, cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@TipoID", (object)tipoId ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@MarcaID", (object)marcaId ?? DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@Estado", estadoSel);

                var dt = new DataTable();
                da.Fill(dt);

                dgv_Reg_Modelo.AutoGenerateColumns = true;
                dgv_Reg_Modelo.DataSource = dt;

                if (dgv_Reg_Modelo.Columns["ModeloID"] != null)
                    dgv_Reg_Modelo.Columns["ModeloID"].Visible = false;

                if (dgv_Reg_Modelo.Columns["TipoID"] != null)
                    dgv_Reg_Modelo.Columns["TipoID"].Visible = false;

                if (dgv_Reg_Modelo.Columns["MarcaID"] != null)
                {
                    var c = dgv_Reg_Modelo.Columns["MarcaID"];
                    c.Visible = true;
                    c.HeaderText = "ID Marca";
                    c.ReadOnly = true;
                    c.Width = 85;
                    c.DisplayIndex = 0;
                }

                AsegurarColumnaEliminar(dgv_Reg_Modelo, false);
            }
        }

        private void AsegurarColumnaEliminar(DataGridView dgv, bool alInicio = false)
        {
            const string colName = "Eliminar";
            var col = dgv.Columns[colName] as DataGridViewButtonColumn;
            if (col == null)
            {
                col = new DataGridViewButtonColumn
                {
                    Name = colName,
                    HeaderText = "",
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    FlatStyle = FlatStyle.Standard   
                };
                dgv.Columns.Add(col);
            }

            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            col.DisplayIndex = alInicio ? 0 : dgv.Columns.Count - 1;
        }

        private void dgv_Reg_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = (DataGridView)sender;
            if (dgv.Columns[e.ColumnIndex].Name == "Eliminar") return;
            if (dgv_Reg_Marcas.Columns[e.ColumnIndex].Name != "Eliminar") return;
            if (dgv_Reg_Modelo.Columns[e.ColumnIndex].Name != "Eliminar") return;

        }

        private void dgv_Reg_Marcas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rowView = dgv_Reg_Marcas.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;
            var row = rowView.Row;

            _editMarcaId = Convert.ToInt32(row["MarcaID"]);
            _isEditingMarca = true;

            int tipoId = Convert.ToInt32(row["TipoID"]);
            string nombre = row["Marca"].ToString();
            string estado = row["Estado"].ToString();

            Cmbox_Tip_Vehic.SelectedValue = tipoId;
            txt_Nom_Marc.Text = nombre;
            Cmbox_Estado_Config_marc.SelectedItem = (estado == "Inactivo") ? "Inactivo" : "Activo";
            btn_Guardar_Marca.Text = "Actualizar marca";
        }

        private void dgv_Reg_Modelo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rowView = dgv_Reg_Modelo.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;
            var row = rowView.Row;

            _editModeloId = Convert.ToInt32(row["ModeloID"]);
            _isEditingModelo = true;

            int tipoId = Convert.ToInt32(row["TipoID"]);
            int marcaId = Convert.ToInt32(row["MarcaID"]);
            string nombre = row["Modelo"].ToString();
            string estado = row["Estado"].ToString();

            Cmbox_Tip_Vehi_Modelo.SelectedValue = tipoId;
            CargarMarcasPorTipo_ParaModelos();
            Cmbox_Marca_Config.SelectedValue = marcaId;

            txt_Nom_Model.Text = nombre;
            Cmbox_Estado_Config_model.SelectedItem = (estado == "Inactivo") ? "Inactivo" : "Activo";

            if (row["AnioDesde"] != DBNull.Value) Cmbox_anio_desde.SelectedItem = Convert.ToInt32(row["AnioDesde"]);
            if (row["AnioHasta"] != DBNull.Value) Cmbox_anio_hasta.SelectedItem = Convert.ToInt32(row["AnioHasta"]);

            var btnModelo = this.Controls.Find("btn_Guardar_Modelo", true).FirstOrDefault() as Button;
            if (btnModelo != null) btnModelo.Text = "Actualizar modelo";
        }

        private void dgv_Reg_Marcas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv_Reg_Marcas.Columns[e.ColumnIndex].Name != "Eliminar") return;

            var rowView = dgv_Reg_Marcas.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;
            var row = rowView.Row;

            int marcaId = Convert.ToInt32(row["MarcaID"]);
            string marca = row["Marca"].ToString();

            var ok = MessageBox.Show(
                $"¿Desactivar la marca '{marca}' y todos sus modelos?",
                "Confirmar desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ok != DialogResult.Yes) return;

            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(
                        @"UPDATE ModeloVehiculo SET Estado='Inactivo' WHERE MarcaID=@id;", cn, tx))
                    { cmd.Parameters.AddWithValue("@id", marcaId); cmd.ExecuteNonQuery(); }

                    using (var cmd = new SqlCommand(
                        @"UPDATE MarcaVehiculo SET Estado='Inactivo' WHERE MarcaID=@id;", cn, tx))
                    { cmd.Parameters.AddWithValue("@id", marcaId); cmd.ExecuteNonQuery(); }

                    tx.Commit();
                }

                if (_editMarcaId == marcaId) { _editMarcaId = null; _isEditingMarca = false; btn_Guardar_Marca.Text = "Guardar marca"; }

                CargarGridMarcas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar la marca: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgv_Reg_Modelo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv_Reg_Modelo.Columns[e.ColumnIndex].Name != "Eliminar") return;

            var rowView = dgv_Reg_Modelo.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;
            var row = rowView.Row;

            int modeloId = Convert.ToInt32(row["ModeloID"]);
            string modelo = row["Modelo"].ToString();

            var ok = MessageBox.Show(
                $"¿Desactivar el modelo '{modelo}'?",
                "Confirmar desactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ok != DialogResult.Yes) return;

            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                using (var cmd = new SqlCommand(
                    @"UPDATE ModeloVehiculo SET Estado='Inactivo' WHERE ModeloID=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", modeloId);
                    cmd.ExecuteNonQuery();
                }

                if (_editModeloId == modeloId)
                {
                    _editModeloId = null; _isEditingModelo = false;
                    var btnModelo = this.Controls.Find("btn_Guardar_Modelo", true).FirstOrDefault() as Button;
                    if (btnModelo != null) btnModelo.Text = "Guardar modelo";
                    txt_Nom_Model.Clear();
                }

                CargarGridModelos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar el modelo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
