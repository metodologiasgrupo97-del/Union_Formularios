using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Datos_Acceso.SqlServer;

namespace Union_Formularios.Formularios_Adicionales
{
    public partial class UC_Repuestos : UserControl
    {
        private int? _editId = null;
        private bool _isSaving = false;
        private bool _initUI = true;

        public UC_Repuestos()
        {
            InitializeComponent();
            WireEvents();
            PrepararGrid();
            CargarCombosFijos();
            CargarTipos();
            ResetFormulario();
            CargarData();

            _initUI = false;         
            AplicarModoTipoRepuesto();
        }

        private void WireEvents()
        {
            // --- Cascadas de combos ---
            Cmbox_Tip_Vehi_Repuesto.SelectedIndexChanged += (s, e) =>
            {
                if (_initUI) return;                 
                CargarMarcasPorTipo();           
                AplicarModoTipoRepuesto();          
            };

            Cmbox_Marca_Repuesto.SelectedIndexChanged += (s, e) =>
            {
                if (_initUI) return;
                if (!EsGenerico())                   
                    CargarModelosPorMarca();
                AplicarModoTipoRepuesto();          
            };

            Cmbox_Tip_Repuesto.SelectedIndexChanged += (s, e) =>
            {
                if (_initUI) return;
                AplicarModoTipoRepuesto();           
            };

            // --- Precio ---
            txt_Precio_U.Enter += Txt_Precio_U_Enter;
            txt_Precio_U.Leave += FormatearMonedaLeave;
            txt_Precio_U.KeyPress += SoloNumerosDecimal;
            txt_Precio_U.TextChanged += NormalizaSeparadorDecimal;

            // --- Stock ---
            txt_Stock_min.KeyPress += SoloEnteros;

            // --- Acciones ---
            btn_Guardar_Repuesto.Click += btn_Guardar_Repuesto_Click;
            btn_Filtrar.Click += (s, e) => CargarData();

            // --- Grid: eliminar / editar ---
            dgv_Reg_Marcas.CellContentClick += dgv_Reg_Marcas_CellContentClick;
            dgv_Reg_Marcas.CellDoubleClick += dgv_Reg_Marcas_CellDoubleClick;
        }

        private void CargarCombosFijos()
        {
            Cmbox_Cat_Repuesto.Items.Clear();
            Cmbox_Cat_Repuesto.Items.AddRange(RepuestosDAO.CategoriasFijas);

            Cmbox_Tip_Repuesto.Items.Clear();
            Cmbox_Tip_Repuesto.Items.Add("Repuestos originales");
            Cmbox_Tip_Repuesto.Items.Add("Repuestos genéricos");

            Cmbox_Estado_Repuesto.Items.Clear();
            Cmbox_Estado_Repuesto.Items.Add("Activo");
            Cmbox_Estado_Repuesto.Items.Add("Inactivo");
        }

        // ----------------- Tipos/Marcas/Modelos desde BD -----------------
        private void CargarTipos()
        {
            var dt = RepuestosDAO.GetTiposVehiculo();
            Cmbox_Tip_Vehi_Repuesto.DisplayMember = "Nombre";
            Cmbox_Tip_Vehi_Repuesto.ValueMember = "TipoID";
            Cmbox_Tip_Vehi_Repuesto.DataSource = dt;
            Cmbox_Tip_Vehi_Repuesto.SelectedIndex = -1;

            Cmbox_Marca_Repuesto.DataSource = null;
            Cmbox_Modelo_Repuesto.DataSource = null;
            Cmbox_Marca_Repuesto.Enabled = false;
            Cmbox_Modelo_Repuesto.Enabled = false;
        }

        private void CargarMarcasPorTipo()
        {
            // si arranque o genérico -> mantener deshabilitado
            if (_initUI || EsGenerico() || Cmbox_Tip_Vehi_Repuesto.SelectedValue == null)
            {
                Cmbox_Marca_Repuesto.DataSource = null;
                Cmbox_Marca_Repuesto.Enabled = false;
                return;
            }

            if (!int.TryParse(Cmbox_Tip_Vehi_Repuesto.SelectedValue.ToString(), out int tipoId))
            { Cmbox_Marca_Repuesto.DataSource = null; Cmbox_Marca_Repuesto.Enabled = false; return; }

            var dt = RepuestosDAO.GetMarcasPorTipo(tipoId);
            Cmbox_Marca_Repuesto.DisplayMember = "Nombre";
            Cmbox_Marca_Repuesto.ValueMember = "MarcaID";
            Cmbox_Marca_Repuesto.DataSource = dt;
            Cmbox_Marca_Repuesto.SelectedIndex = -1;
            Cmbox_Marca_Repuesto.Enabled = !EsGenerico() && dt.Rows.Count > 0;
        }

        private void CargarModelosPorMarca()
        {
            if (_initUI || EsGenerico() || Cmbox_Marca_Repuesto.SelectedValue == null)
            {
                Cmbox_Modelo_Repuesto.DataSource = null;
                Cmbox_Modelo_Repuesto.Enabled = false;
                return;
            }

            if (!int.TryParse(Cmbox_Marca_Repuesto.SelectedValue.ToString(), out int marcaId))
            { Cmbox_Modelo_Repuesto.DataSource = null; Cmbox_Modelo_Repuesto.Enabled = false; return; }

            var dt = RepuestosDAO.GetModelosPorMarca(marcaId);
            Cmbox_Modelo_Repuesto.DisplayMember = "Nombre";
            Cmbox_Modelo_Repuesto.ValueMember = "ModeloID";
            Cmbox_Modelo_Repuesto.DataSource = dt;
            Cmbox_Modelo_Repuesto.SelectedIndex = -1;
            Cmbox_Modelo_Repuesto.Enabled = !EsGenerico() && dt.Rows.Count > 0;
        }

        // ----------------- Grid -----------------
        private void PrepararGrid()
        {
            var g = dgv_Reg_Marcas;
            g.AutoGenerateColumns = false;
            g.Columns.Clear();

            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Codigo", HeaderText = "Código", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Categoria", HeaderText = "Categoría", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TipoRepuesto", HeaderText = "Tipo repuesto", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TipoVehiculo", HeaderText = "Tipo vehículo", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", HeaderText = "Marca", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", HeaderText = "Modelo", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrecioUnitario", HeaderText = "Precio", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Stock", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            g.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Activo", HeaderText = "Activo", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            var btn = new DataGridViewButtonColumn { Name = "colEliminar", HeaderText = "", Text = "Eliminar", UseColumnTextForButtonValue = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            g.Columns.Add(btn);

            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RepuestoID", Name = "RepuestoID", Visible = false });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TipoID", Name = "TipoID", Visible = false });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MarcaID", Name = "MarcaID", Visible = false });
            g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ModeloID", Name = "ModeloID", Visible = false });

            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.AllowUserToAddRows = false;
            g.ReadOnly = true;
        }

        private void CargarData()
        {
            int? tipoId = (Cmbox_Tip_Vehi_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Tip_Vehi_Repuesto.SelectedValue.ToString(), out var t)) ? (int?)t : null;
            int? marcaId = (Cmbox_Marca_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Marca_Repuesto.SelectedValue.ToString(), out var m)) ? (int?)m : null;
            int? modeloId = (Cmbox_Modelo_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Modelo_Repuesto.SelectedValue.ToString(), out var mo)) ? (int?)mo : null;

            string categoria = Cmbox_Cat_Repuesto.SelectedItem == null ? null : Cmbox_Cat_Repuesto.SelectedItem.ToString();
            string tipoRep = Cmbox_Tip_Repuesto.SelectedItem == null ? null : Cmbox_Tip_Repuesto.SelectedItem.ToString();

            var dt = RepuestosDAO.Listar(tipoId, marcaId, modeloId, categoria, tipoRep);
            dgv_Reg_Marcas.DataSource = dt;
        }

        // ----------------- Helpers UI -----------------
        private void ResetFormulario()
        {
            _editId = null;

            txt_Cod_Repuesto.ForeColor = System.Drawing.Color.Black;
            using (var cn = Conexion_SQL.OpenConnection())
                txt_Cod_Repuesto.Text = RepuestosDAO.ObtenerSiguienteCodigo(cn);

            txt_Descripcion_Repuesto.Text = "";
            Cmbox_Tip_Vehi_Repuesto.SelectedIndex = -1;
            Cmbox_Cat_Repuesto.SelectedIndex = -1;
            Cmbox_Tip_Repuesto.SelectedIndex = -1;

            Cmbox_Marca_Repuesto.DataSource = null; Cmbox_Marca_Repuesto.Enabled = false;
            Cmbox_Modelo_Repuesto.DataSource = null; Cmbox_Modelo_Repuesto.Enabled = false;

            txt_Precio_U.Text = "$0.00";
            txt_Stock_min.Text = "0";
            Cmbox_Estado_Repuesto.SelectedItem = "Activo";

            btn_Guardar_Repuesto.Text = "Guardar repuesto";
            btn_Filtrar.Enabled = true;

            AplicarModoTipoRepuesto();
        }

        private void SoloEnteros(object sender, KeyPressEventArgs e)
        { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; }

        private void SoloNumerosDecimal(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                var tb = (Guna2TextBox)sender;
                if (tb.Text.Contains(".") || tb.Text.Contains(",")) e.Handled = true;
                return;
            }
            e.Handled = true;
        }

        private bool EsGenerico() => string.Equals(Cmbox_Tip_Repuesto.SelectedItem?.ToString(), "Repuestos genéricos", StringComparison.OrdinalIgnoreCase);
        private void AplicarModoTipoRepuesto()
        {
            bool gen = EsGenerico();
            Cmbox_Marca_Repuesto.Enabled = !gen && Cmbox_Tip_Vehi_Repuesto.SelectedIndex >= 0;
            Cmbox_Modelo_Repuesto.Enabled = !gen && Cmbox_Marca_Repuesto.SelectedIndex >= 0;

            if (gen)
            {
                Cmbox_Marca_Repuesto.DataSource = null;
                Cmbox_Modelo_Repuesto.DataSource = null;
                Cmbox_Marca_Repuesto.SelectedIndex = -1;
                Cmbox_Modelo_Repuesto.SelectedIndex = -1;
            }
        }


        // -------------- TextChanged del precio (suave, sin romper separadores) --------------
        private void NormalizaSeparadorDecimal(object sender, EventArgs e)
        {
            var tb = (Guna2TextBox)sender;
            if (string.IsNullOrEmpty(tb.Text)) return;

            // asegura el prefijo $
            var t = tb.Text.Trim();
            if (!t.StartsWith("$")) t = "$" + t.Replace("$", "");
            tb.Text = t;
            tb.SelectionStart = tb.Text.Length;
        }

        private void Txt_Precio_U_Enter(object sender, EventArgs e)
        {
            var t = txt_Precio_U.Text == null ? "" : txt_Precio_U.Text.Trim();
            if (t == "" || t == "$0.00" || t == "$0,00") txt_Precio_U.Text = "$";
            txt_Precio_U.SelectionStart = txt_Precio_U.Text.Length;
        }

        // -------------- Leave del precio (formato final) --------------
        private void FormatearMonedaLeave(object sender, EventArgs e)
        {
            var tb = (Guna2TextBox)sender;
            if (!TryParsePrecio(tb.Text, out decimal val))
            {
                tb.Text = "$0.00";
                return;
            }

            if (val > 99999999.99m) val = 99999999.99m;
            if (val < 0m) val = 0m;

            tb.Text = string.Format(CultureInfo.CurrentCulture, "{0:C2}", val);
        }

        private bool ValidarObligatorios()
        {
            if (string.IsNullOrWhiteSpace(txt_Descripcion_Repuesto.Text))
            { MessageBox.Show("La descripción es obligatoria."); return false; }

            if (Cmbox_Tip_Vehi_Repuesto.SelectedIndex < 0)
            { MessageBox.Show("Seleccione el tipo de vehículo."); return false; }

            if (Cmbox_Cat_Repuesto.SelectedIndex < 0)
            { MessageBox.Show("Seleccione una categoría."); return false; }

            if (Cmbox_Tip_Repuesto.SelectedIndex < 0)
            { MessageBox.Show("Seleccione el tipo de repuesto."); return false; }

            if (!EsGenerico())
            {
                if (Cmbox_Marca_Repuesto.SelectedIndex < 0)
                { MessageBox.Show("Seleccione la marca (para repuestos originales)."); return false; }

                if (Cmbox_Modelo_Repuesto.SelectedIndex < 0)
                { MessageBox.Show("Seleccione el modelo (para repuestos originales)."); return false; }
            }

            return true;
        }

        // -------------- Botón Guardar --------------
        private void btn_Guardar_Repuesto_Click(object sender, EventArgs e)
        {
            if (_isSaving) return;
            _isSaving = true;

            try
            {
                if (!ValidarObligatorios()) return;

                if (!TryParsePrecio(txt_Precio_U.Text, out decimal precio))
                { MessageBox.Show("Precio inválido."); return; }

                if (!int.TryParse(txt_Stock_min.Text, out int stock))
                { MessageBox.Show("Stock mínimo inválido."); return; }

                bool activo = (Cmbox_Estado_Repuesto.SelectedItem?.ToString() == "Activo");
                if (stock <= 0) activo = false;

                int? tipoId = (Cmbox_Tip_Vehi_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Tip_Vehi_Repuesto.SelectedValue.ToString(), out var t)) ? (int?)t : null;

                // === CAMBIO: si es genérico, forzar Marca/Modelo a NULL
                int? marcaId = null;
                int? modeloId = null;
                if (!EsGenerico())
                {
                    marcaId = (Cmbox_Marca_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Marca_Repuesto.SelectedValue.ToString(), out var m)) ? (int?)m : null;
                    modeloId = (Cmbox_Modelo_Repuesto.SelectedValue != null && int.TryParse(Cmbox_Modelo_Repuesto.SelectedValue.ToString(), out var mo)) ? (int?)mo : null;
                }

                string codigo = txt_Cod_Repuesto.Text.Trim();
                string nombre = txt_Descripcion_Repuesto.Text.Trim();
                string categoria = Cmbox_Cat_Repuesto.SelectedItem?.ToString();
                string tipoRep = Cmbox_Tip_Repuesto.SelectedItem?.ToString();
                int? impDefault = null;

                if (_editId == null)
                {
                    RepuestosDAO.Insertar(codigo, nombre, categoria, tipoRep,
                                           tipoId, marcaId, modeloId,
                                           precio, stock, activo, impDefault);
                    MessageBox.Show("Repuesto guardado.");
                }
                else
                {
                    RepuestosDAO.Actualizar(_editId.Value, codigo, nombre, categoria, tipoRep,
                                            tipoId, marcaId, modeloId,
                                            precio, stock, activo, impDefault);
                    MessageBox.Show("Repuesto actualizado.");
                }

                CargarData();
                ResetFormulario();
                CargarData();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("El código ya existe. Se generará el siguiente disponible.", "Código duplicado");
                    using (var cn = Conexion_SQL.OpenConnection())
                        txt_Cod_Repuesto.Text = RepuestosDAO.ObtenerSiguienteCodigo(cn);
                }
                else { MessageBox.Show("Error SQL: " + ex.Message); }
            }
            catch (Exception ex)
            { MessageBox.Show("Error: " + ex.Message); }
            finally
            {
                _isSaving = false;
            }
        }

        // ----------------- Grid: eliminar / editar -----------------
        private void dgv_Reg_Marcas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv_Reg_Marcas.Columns[e.ColumnIndex].Name != "colEliminar") return;

            var rowView = dgv_Reg_Marcas.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null) return;
            int id = Convert.ToInt32(rowView["RepuestoID"]);

            if (MessageBox.Show("¿Desea marcar como Inactivo este repuesto?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RepuestosDAO.Desactivar(id);
                CargarData();
            }
        }

        private void dgv_Reg_Marcas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var rv = dgv_Reg_Marcas.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rv == null) return;
            var row = rv.Row;

            _editId = row.Field<int>("RepuestoID");
            txt_Cod_Repuesto.Text = row["Codigo"].ToString();
            txt_Descripcion_Repuesto.Text = row["Nombre"].ToString();

            // Tipo/Categoría/TipoRepuesto
            if (row.Table.Columns.Contains("TipoID") && row["TipoID"] != DBNull.Value)
            {
                Cmbox_Tip_Vehi_Repuesto.SelectedValue = Convert.ToInt32(row["TipoID"]);
                CargarMarcasPorTipo();
            }

            string cat = row["Categoria"] == DBNull.Value ? null : row["Categoria"].ToString();
            if (!string.IsNullOrEmpty(cat)) Cmbox_Cat_Repuesto.SelectedItem = cat;

            string trep = row["TipoRepuesto"] == DBNull.Value ? null : row["TipoRepuesto"].ToString();
            if (!string.IsNullOrEmpty(trep)) Cmbox_Tip_Repuesto.SelectedItem = trep;

            // Aplica modo (esto deshabilita marca/modelo si es genérico)
            AplicarModoTipoRepuesto();

            // Marca/Modelo solo si NO es genérico
            if (!EsGenerico())
            {
                if (row.Table.Columns.Contains("MarcaID") && row["MarcaID"] != DBNull.Value)
                {
                    Cmbox_Marca_Repuesto.SelectedValue = Convert.ToInt32(row["MarcaID"]);
                    CargarModelosPorMarca();
                }
                if (row.Table.Columns.Contains("ModeloID") && row["ModeloID"] != DBNull.Value)
                {
                    Cmbox_Modelo_Repuesto.SelectedValue = Convert.ToInt32(row["ModeloID"]);
                }
            }

            decimal precio = row["PrecioUnitario"] == DBNull.Value ? 0m : Convert.ToDecimal(row["PrecioUnitario"]);
            txt_Precio_U.Text = string.Format(CultureInfo.CurrentCulture, "{0:C2}", precio);

            txt_Stock_min.Text = row["Stock"] == DBNull.Value ? "0" : row["Stock"].ToString();
            Cmbox_Estado_Repuesto.SelectedItem = (row["Activo"] != DBNull.Value && (bool)row["Activo"]) ? "Activo" : "Inactivo";

            btn_Guardar_Repuesto.Text = "Actualizar repuesto";
            btn_Filtrar.Enabled = false;
        }

        // -------------- Parseo de precio robusto --------------
        private static bool TryParsePrecio(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            // quita símbolo de moneda y espacios
            var raw = input.Trim().Replace(" ", "").Replace("$", "");

            // 1) intenta con la cultura actual (maneja 0,50 o 0.50 según SO)
            if (decimal.TryParse(raw, NumberStyles.Number | NumberStyles.AllowCurrencySymbol,
                                 CultureInfo.CurrentCulture, out value))
                return true;

            // 2) intenta con cultura ecuatoriana (coma decimal)
            var esEC = CultureInfo.GetCultureInfo("es-EC");
            if (decimal.TryParse(raw, NumberStyles.Number | NumberStyles.AllowCurrencySymbol,
                                 esEC, out value))
                return true;

            // 3) plan C: reemplaza coma por punto SOLO para decimal y parsea Invariant
            raw = raw.Replace(",", ".");
            // no elimines puntos: pueden ser el separador decimal real
            return decimal.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
        }


    }
}
