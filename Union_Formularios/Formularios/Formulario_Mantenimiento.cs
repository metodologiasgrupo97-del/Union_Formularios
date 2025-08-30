using Datos_Acceso.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Union_Formularios.Formularios;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    // Form que gestiona el registro de mantenimiento y la emisión de facturas
    public partial class Formulario_Mantenimiento : Form
    {
        // Catálogo simple de servicios con su precio base (para llenar la UI y calcular totales)
        Dictionary<string, decimal> preciosServicio = new Dictionary<string, decimal>
        {
            { "Cambio de aceite", 25 },
            { "Cambio de frenos", 40 },
            { "Alineación", 35 },
            { "Revisión general", 50 }
        };

        // Referencias a labels (si las usas desde el diseñador, permanecen)
        private Label lb_valor_servicio;
        private Label label10;

        // Tasa de IVA vigente (se intenta actualizar desde BD en el Load)
        private decimal _ivaRate = 0.15m;

        // Cadena de conexión a la BD
        private readonly string _connStr = "Server=DESKTOP-9TRMID2; DataBase=CAR_EFULL; Integrated Security=true; TrustServerCertificate=True";

        // Guarda el ID del mecánico seleccionado en el selector (puede ser null si no hay selección)
        private int? _selectedUserId;

        // Constructor del formulario
        public Formulario_Mantenimiento()
        {
            InitializeComponent();
        }

        // Evento Load: inicializa IVA, grilla, combos, y limpia los datos del visor de factura
        private void Formulario_Mantenimiento_Load(object sender, EventArgs e)
        {
            CargarIvaVigente();        // lee IVA vigente desde la tabla Impuestos
            CargarRepuestos();         // trae los repuestos y arma columnas editables

            // Configuración base de la grilla
            dgvRepuestos.DataSource = null;
            dgvRepuestos.Columns.Clear();
            dgvRepuestos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRepuestos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvRepuestos.AllowUserToResizeColumns = false;

            // El resumen de repuestos se muestra en varias líneas
            txt_Show_Repuestos.Multiline = true;

            // Combos de pago (método y forma)
            cmb_metodo_de_pago.Items.Clear();
            cmb_metodo_de_pago.Items.AddRange(new[] { "Efectivo", "Tarjeta", "Transferencia" });
            cmb_metodo_de_pago.SelectedIndex = -1;

            cmb_forma_de_pago.Items.Clear();
            cmb_forma_de_pago.Items.AddRange(new[] { "Contado", "Crédito" });
            cmb_forma_de_pago.SelectedIndex = -1;

            // Encabezado de la grilla: altura y estilo
            dgvRepuestos.ColumnHeadersVisible = true;
            dgvRepuestos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            if (dgvRepuestos.ColumnHeadersHeight < 28) dgvRepuestos.ColumnHeadersHeight = 28;

            // Modo de edición y validaciones varias
            dgvRepuestos.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvRepuestos.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvRepuestos.EditingControlShowing += dgvRepuestos_EditingControlShowing;     // restringe Cantidad a números
            dgvRepuestos.CurrentCellDirtyStateChanged += dgvRepuestos_CurrentCellDirtyStateChanged; // confirma checkbox
            dgvRepuestos.CellValidating += dgvRepuestos_CellValidating;                   // valida Cantidad y stock

            // Pone todas las columnas en solo lectura salvo "Cantidad" y "Seleccionar"
            foreach (DataGridViewColumn col in dgvRepuestos.Columns)
            {
                if (col.Name != "Cantidad" && col.Name != "Seleccionar")
                    col.ReadOnly = true;
            }
            // Oculta la columna de clave si existe
            if (dgvRepuestos.Columns.Contains("RepuestoID"))
                dgvRepuestos.Columns["RepuestoID"].Visible = false;

            // Limpia los campos del "preview" de factura
            txt_Show_Placa.Clear();
            txt_Show_Date.Clear();
            txt_Show_Service.Clear();
            txt_Show_Mecanico_Res.Clear();
            txt_Show_Repuestos.Clear();
            txt_Show_Obsv.Clear();

            // Resetea totales mostrados
            lbl_sub_total.Text = "$ 0,00";
            lbl_iva.Text = "$ 0,00";
            lbl_total_a_pagar.Text = "$ 0,00";

            // Alinea un label (si aplica en tu diseño)
            label1.TextAlign = ContentAlignment.MiddleRight;
            label1.AutoSize = false;
            label1.Width = 100;

            // Carga de placas al combo desde la tabla Vehiculos
            using (SqlConnection con = new SqlConnection(_connStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Placa FROM Vehiculos", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    Cmbox_Select_Plaque.Items.Add(Convert.ToString(reader["Placa"]));
                reader.Close();
            }

            // Carga el combo de tipos de servicio (Key/Value para precio automático)
            Cmbox_Tip_Service_Realizado.Items.Clear();
            foreach (var servicio in preciosServicio)
                Cmbox_Tip_Service_Realizado.Items.Add(new KeyValuePair<string, decimal>(servicio.Key, servicio.Value));
            Cmbox_Tip_Service_Realizado.DisplayMember = "Key";
            Cmbox_Tip_Service_Realizado.ValueMember = "Value";
        }

        // Callback que recibe el trabajador elegido en el selector y lo refleja en la UI
        public void EstablecerTrabajador(int id, string nombreCompleto)
        {
            _selectedUserId = id;                // guarda el ID para usarlo al facturar
            txt_Show_Mecanico_Res.Text = nombreCompleto;
            txt_preview_mecanico.Text = nombreCompleto;
        }

        // Carga los repuestos al DataGridView y agrega columnas "Cantidad" y "Seleccionar" si no existen
        private void CargarRepuestos()
        {
            Conexion_BD_Repuestos dao = new Conexion_BD_Repuestos();
            DataTable dt = dao.ObtenerRepuestos();  // consulta de la capa de datos

            dgvRepuestos.DataSource = dt;

            // Columna editable: Cantidad
            if (!dgvRepuestos.Columns.Contains("Cantidad"))
            {
                var colCantidad = new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    ValueType = typeof(int),
                    Width = 90
                };
                dgvRepuestos.Columns.Insert(2, colCantidad);
            }

            // Columna check: Seleccionar
            if (!dgvRepuestos.Columns.Contains("Seleccionar"))
            {
                var colCheck = new DataGridViewCheckBoxColumn
                {
                    Name = "Seleccionar",
                    HeaderText = "Seleccionar",
                    Width = 70
                };
                dgvRepuestos.Columns.Add(colCheck);
            }

            // Oculta la clave si está
            if (dgvRepuestos.Columns.Contains("RepuestoID"))
                dgvRepuestos.Columns["RepuestoID"].Visible = false;

            // Deja el resto en solo lectura
            foreach (DataGridViewColumn col in dgvRepuestos.Columns)
            {
                if (col.Name != "Cantidad" && col.Name != "Seleccionar")
                    col.ReadOnly = true;
            }
        }

        // Ajusta cantidad contra stock cuando termina de editarse la celda
        private void dgvRepuestos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRepuestos.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                var row = dgvRepuestos.Rows[e.RowIndex];
                if (row.Cells["Stock"].Value != null && row.Cells["Cantidad"].Value != null)
                {
                    int stock = Convert.ToInt32(row.Cells["Stock"].Value);
                    int cantidad = 0;
                    int.TryParse(Convert.ToString(row.Cells["Cantidad"].Value), out cantidad);

                    if (cantidad > stock)
                    {
                        MessageBox.Show("La cantidad sobrepasa el stock disponible.", "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.Cells["Cantidad"].Value = stock; // recorta al máximo permitido
                    }
                }
            }
            ActualizarListaRepuestos(); // refresca el resumen del lado derecho
        }

        // Al comenzar a editar "Cantidad" forzamos que solo acepte números
        private void dgvRepuestos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvRepuestos.CurrentCell == null) return;
            if (dgvRepuestos.Columns[dgvRepuestos.CurrentCell.ColumnIndex].Name == "Cantidad")
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress -= Cantidad_KeyPressOnlyNumbers;
                    tb.KeyPress += Cantidad_KeyPressOnlyNumbers;
                }
            }
        }

        // Validación para permitir solo dígitos en la columna Cantidad
        private void Cantidad_KeyPressOnlyNumbers(object sender, KeyPressEventArgs e)
        {
            // Solo dígitos y teclas de control (backspace, delete, etc.)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Confirma inmediatamente el cambio de un CheckBoxCell (Seleccionar)
        private void dgvRepuestos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvRepuestos.IsCurrentCellDirty && dgvRepuestos.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvRepuestos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                ActualizarListaRepuestos();
            }
        }

        // Validación de la columna "Cantidad": número entero >= 0 y no exceder stock
        private void dgvRepuestos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvRepuestos.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                var row = dgvRepuestos.Rows[e.RowIndex];
                if (row.IsNewRow) return;

                int cantidad;
                if (!int.TryParse(Convert.ToString(e.FormattedValue), out cantidad) || cantidad < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show("Ingrese una cantidad numérica válida (entera y >= 0).", "Cantidad inválida",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (row.Cells["Stock"]?.Value != null)
                {
                    int stock = 0;
                    int.TryParse(row.Cells["Stock"].Value.ToString(), out stock);
                    if (cantidad > stock)
                    {
                        e.Cancel = true;
                        MessageBox.Show("La cantidad sobrepasa el stock disponible.", "Stock insuficiente",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        // Lee la tasa de IVA vigente desde BD (si falla, usa 15% por defecto)
        private void CargarIvaVigente()
        {
            try
            {
                using (var con = new SqlConnection(_connStr))
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 TasaDecimal
                    FROM Impuestos
                    WHERE Activo = 1 AND Codigo = 'IVA'
                      AND GETDATE() >= VigenteDesde
                      AND (VigenteHasta IS NULL OR GETDATE() <= VigenteHasta)
                    ORDER BY VigenteDesde DESC;", con))
                {
                    con.Open();
                    var r = cmd.ExecuteScalar();
                    if (r != null && r != DBNull.Value)
                        _ivaRate = Convert.ToDecimal(r);
                }
            }
            catch
            {
                _ivaRate = 0.15m; // fallback
            }
        }

        // Cuando marcan/desmarcan "Seleccionar" en la grilla, actualiza el resumen
        private void dgvRepuestos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRepuestos.Columns[e.ColumnIndex].Name == "Seleccionar" && e.RowIndex >= 0)
            {
                dgvRepuestos.EndEdit();
                ActualizarListaRepuestos();
            }
        }

        // Reconstruye el texto con los repuestos seleccionados (Nombre x Cantidad)
        private void ActualizarListaRepuestos()
        {
            txt_Show_Repuestos.Clear();

            foreach (DataGridViewRow row in dgvRepuestos.Rows)
            {
                if (row.Cells["Seleccionar"].Value != null && (bool)row.Cells["Seleccionar"].Value == true)
                {
                    string nombre = row.Cells["Nombre"].Value?.ToString();
                    string cantidad = row.Cells["Cantidad"].Value?.ToString();
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        txt_Show_Repuestos.AppendText($"{nombre} x {cantidad}{Environment.NewLine}");
                    }
                }
            }
        }

        // Obtiene VehicleID e ID_Propietario por placa (ID_Propietario puede ser null)
        private (int vehicleId, int? idProp) GetVehiculoInfo(SqlConnection con, SqlTransaction tx, string placa)
        {
            using (var cmd = new SqlCommand(
                "SELECT VehicleID, ID_Propietario FROM Vehiculos WHERE Placa=@Placa", con, tx))
            {
                cmd.Parameters.Add("@Placa", SqlDbType.NVarChar, 10).Value = placa;
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        throw new InvalidOperationException("No se encontró el vehículo seleccionado.");

                    int vehicleId = r.GetInt32(0);
                    int? idProp = r.IsDBNull(1) ? (int?)null : r.GetInt32(1);
                    return (vehicleId, idProp);
                }
            }
        }

        // Devuelve DBNull para cadenas vacías (para parámetros opcionales)
        private static object DbOrNull(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s;
        }

        // Helper genérico para tipos valor nullable
        private static object DbOrNull<T>(T? value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        // Parsea textos tipo "$ 1.234,56" a decimal respetando la cultura del SO
        private static decimal ParseMoney(string s)
        {
            if (decimal.TryParse(s, NumberStyles.Currency, CultureInfo.CurrentCulture, out var d)) return d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return 0m;
        }

        // ==== Guardar Factura (cabecera + detalle) con validaciones y transacción ====
        private void btn_Grabar_Datos_factura_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones mínimas de UI antes de tocar la BD
                if (string.IsNullOrWhiteSpace(txt_Show_Placa.Text))
                { MessageBox.Show("Selecciona un vehículo y presiona 'Cargar Datos'."); return; }

                if (string.IsNullOrWhiteSpace(txt_Show_Service.Text))
                { MessageBox.Show("Selecciona el tipo de servicio y presiona 'Cargar Datos'."); return; }

                if (string.IsNullOrWhiteSpace(txt_Show_Mecanico_Res.Text))
                { MessageBox.Show("Selecciona el mecánico responsable."); return; }

                if (cmb_metodo_de_pago.SelectedIndex < 0)
                { MessageBox.Show("Selecciona el método de pago."); return; }

                if (cmb_forma_de_pago.SelectedIndex < 0)
                { MessageBox.Show("Selecciona la forma de pago."); return; }

                // Confirmación previa
                var rta = MessageBox.Show(this, "¿Está seguro de guardar la factura?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (rta != DialogResult.Yes) return;

                using (SqlConnection con = new SqlConnection(_connStr))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    try
                    {
                        // Parseo de totales desde labels
                        decimal total = ParseMoney(lbl_total_a_pagar.Text);
                        decimal subtotal = ParseMoney(lbl_sub_total.Text);
                        decimal iva = ParseMoney(lbl_iva.Text);

                        // Busca VehicleID + ID_Propietario (este último es NOT NULL en Facturas)
                        var info = GetVehiculoInfo(con, tran, txt_Show_Placa.Text);
                        if (!info.idProp.HasValue)
                        {
                            MessageBox.Show("La placa seleccionada no tiene propietario asignado. Asignar propietario antes de facturar.",
                                "Facturación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tran.Rollback();
                            return;
                        }

                        // ID de mecánico (puede ser null si no se eligió desde el selector)
                        int? userId = _selectedUserId;

                        // Convierte fecha del preview (si falla, usa hoy)
                        DateTime fechaMant = DateTime.Today;
                        DateTime.TryParseExact(txt_Show_Date.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                               DateTimeStyles.None, out fechaMant);

                        // Inserta cabecera de factura (usa DBNull para campos opcionales)
                        string insertFactura = 
                        @"INSERT INTO Facturas(CodigoFactura, Fecha, ID_Propietario, VehicleID, MetodoPago, FormaPago, Moneda, Subtotal, IVA, Total, FechaMantenimiento, TipoServicio, UserID, Observaciones)
                        VALUES (@CodigoFactura, @Fecha, @ID_Propietario, @VehicleID, @MetodoPago, @FormaPago, 'USD', @Subtotal, @IVA, @Total, @FechaMantenimiento, @TipoServicio, @UserID, @Observaciones);
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

                        SqlCommand cmdFactura = new SqlCommand(insertFactura, con, tran);

                        string codigoFactura = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                        cmdFactura.Parameters.AddWithValue("@CodigoFactura", codigoFactura);
                        cmdFactura.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        cmdFactura.Parameters.AddWithValue("@ID_Propietario", info.idProp.Value);
                        cmdFactura.Parameters.AddWithValue("@VehicleID", info.vehicleId);
                        cmdFactura.Parameters.AddWithValue("@MetodoPago", cmb_metodo_de_pago.Text);
                        cmdFactura.Parameters.AddWithValue("@FormaPago", cmb_forma_de_pago.Text);
                        cmdFactura.Parameters.AddWithValue("@Subtotal", subtotal);
                        cmdFactura.Parameters.AddWithValue("@IVA", iva);
                        cmdFactura.Parameters.AddWithValue("@Total", total);
                        cmdFactura.Parameters.AddWithValue("@FechaMantenimiento", fechaMant);
                        cmdFactura.Parameters.AddWithValue("@TipoServicio", DbOrNull(txt_Show_Service.Text));
                        cmdFactura.Parameters.AddWithValue("@UserID", DbOrNull(userId));
                        cmdFactura.Parameters.AddWithValue("@Observaciones",
                            DbOrNull(string.IsNullOrWhiteSpace(txt_Show_Obsv.Text) ? null : txt_Show_Obsv.Text));

                        int facturaID = Convert.ToInt32(cmdFactura.ExecuteScalar());

                        // Inserta detalle para cada repuesto marcado como "Seleccionar"
                        foreach (DataGridViewRow row in dgvRepuestos.Rows)
                        {
                            bool seleccionado = row.Cells["Seleccionar"].Value != null && (bool)row.Cells["Seleccionar"].Value;
                            if (!seleccionado) continue;

                            int repuestoID = Convert.ToInt32(row.Cells["RepuestoID"].Value);
                            int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                            decimal precioUnit = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);

                            string insertDetalle = @"INSERT INTO FacturaDetalle (FacturaID, RepuestoID, Cantidad, PrecioUnitario, IVA) 
                                                     VALUES (@FacturaID, @RepuestoID, @Cantidad, @PrecioUnitario, @IVA)";

                            SqlCommand cmdDetalle = new SqlCommand(insertDetalle, con, tran);
                            cmdDetalle.Parameters.AddWithValue("@FacturaID", facturaID);
                            cmdDetalle.Parameters.AddWithValue("@RepuestoID", repuestoID);
                            cmdDetalle.Parameters.AddWithValue("@Cantidad", cantidad);
                            cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", precioUnit);
                            cmdDetalle.Parameters.AddWithValue("@IVA", Math.Round(precioUnit * cantidad * _ivaRate, 2));
                            cmdDetalle.ExecuteNonQuery();
                        }

                        tran.Commit();
                        MessageBox.Show("Factura guardada con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Error al guardar factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón "Cargar Datos": copia la selección a la derecha (preview) y recalcula totales
        private void btn_Guardar_Datos_factura_Click(object sender, EventArgs e)
        {
            // Placa seleccionada
            txt_Show_Placa.Text = Cmbox_Select_Plaque.Text;

            // Fecha de mantenimiento (si no está marcado el check, usa fecha de hoy)
            DateTime fecha;
            try
            {
                if (DTP_Date_Mantenimiento.ShowCheckBox && !DTP_Date_Mantenimiento.Checked)
                    fecha = DateTime.Today;
                else
                    fecha = DTP_Date_Mantenimiento.Value.Date;
            }
            catch { fecha = DateTime.Today; }
            txt_Show_Date.Text = fecha.ToString("dd/MM/yyyy");

            // Servicio seleccionado (solo nombre)
            txt_Show_Service.Text = Cmbox_Tip_Service_Realizado.Text;

            // Cálculo del costo del servicio en base al catálogo
            decimal costoServicio = 0m;
            if (Cmbox_Tip_Service_Realizado.SelectedItem is KeyValuePair<string, decimal> kv)
                costoServicio = kv.Value;
            else if (!string.IsNullOrWhiteSpace(Cmbox_Tip_Service_Realizado.Text) &&
                     preciosServicio.TryGetValue(Cmbox_Tip_Service_Realizado.Text, out var val))
                costoServicio = val;
            lb_valor_servicio.Text = costoServicio.ToString("C");

            // Recolecta repuestos seleccionados y totaliza
            List<string> repuestosSeleccionados = new List<string>();
            decimal totalRepuestos = 0m;

            foreach (DataGridViewRow row in dgvRepuestos.Rows)
            {
                if (row.IsNewRow) continue;

                bool seleccionado = row.Cells["Seleccionar"].Value is bool b && b;
                if (!seleccionado) continue;

                string nombre = Convert.ToString(row.Cells["Nombre"]?.Value);

                int cantidad = 0;
                int.TryParse(Convert.ToString(row.Cells["Cantidad"]?.Value), out cantidad);

                decimal precio = 0m;
                var vPrecio = row.Cells["PrecioUnitario"]?.Value;
                if (vPrecio != null && vPrecio != DBNull.Value)
                {
                    if (vPrecio is decimal dec) precio = dec;
                    else decimal.TryParse(vPrecio.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out precio);
                }

                if (!string.IsNullOrWhiteSpace(nombre) && cantidad > 0 && precio > 0)
                {
                    repuestosSeleccionados.Add($"{nombre} x{cantidad}");
                    totalRepuestos += cantidad * precio;
                }
            }

            // Muestra lista de repuestos seleccionados o un placeholder si no hay
            txt_Show_Repuestos.Text = repuestosSeleccionados.Count > 0
                ? string.Join(", ", repuestosSeleccionados)
                : "(Ningún repuesto seleccionado)";

            // Observaciones
            txt_Show_Obsv.Text = string.IsNullOrWhiteSpace(txt_Obsv.Text)
                ? "(Ninguna observación)"
                : txt_Obsv.Text;

            // Totales finales (subtotal + IVA) según tasa vigente
            decimal totalFactura = totalRepuestos + costoServicio;
            decimal subtotal = Math.Round(totalFactura / (1 + _ivaRate), 2);
            decimal iva = Math.Round(totalFactura - subtotal, 2);

            lbl_sub_total.Text = subtotal.ToString("C");
            lbl_iva.Text = iva.ToString("C");
            lbl_total_a_pagar.Text = totalFactura.ToString("C");
        }

        // Botón reservado (si en el futuro se requiere recalcular sin tocar la UI :D)
        private void btnCalcularTotal_Click(object sender, EventArgs e)
        {
        }

        // Abre el diálogo para seleccionar el trabajador/mecánico y devuelve a este form
        private void btn_Seleccionar_Propietario_Click(object sender, EventArgs e)
        {
            Formulario_Seleccionar_Trabajador formTrabajador = new Formulario_Seleccionar_Trabajador(this);
            formTrabajador.ShowDialog();
        }

        // Radio “Sí, usar repuestos”: habilita y recarga la grilla
        private void rbtn_si_GM_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtn_si_GM.Checked)
            {
                dgvRepuestos.Enabled = true;

                dgvRepuestos.DataSource = null;
                dgvRepuestos.Columns.Clear();
                dgvRepuestos.Rows.Clear();
                CargarRepuestos();
            }
        }

        // Radio “No usar repuestos”: limpia la grilla
        private void rdb_NoUsarRepuestos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtn_no_GM.Checked)
            {
                dgvRepuestos.DataSource = null;
                dgvRepuestos.Columns.Clear();
                dgvRepuestos.Rows.Clear();
            }
        }
        private void Cmbox_Select_Plaque_SelectedIndexChanged(object sender, EventArgs e){}
        private void DTP_Date_Mantenimiento_ValueChanged(object sender, EventArgs e)
        {
            txt_Show_Date.Text = DTP_Date_Mantenimiento.Value.ToString("dd/MM/yyyy");
        }
        private void txt_Show_Repuestos_TextChanged(object sender, EventArgs e) { }
        private Label label1;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Select_Plaque;
        private Label label2;
        private Guna.UI2.WinForms.Guna2DateTimePicker DTP_Date_Mantenimiento;
        private Label label3;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Tip_Service_Realizado;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label8;
        private Guna.UI2.WinForms.Guna2RadioButton rbtn_no_GM;
        private Guna.UI2.WinForms.Guna2TextBox txt_Obsv;
        private Label label7;
        private Label lblTotal;
        private Guna.UI2.WinForms.Guna2Button btn_Guardar_Datos_factura;
        private Guna.UI2.WinForms.Guna2Button btn_Cargar_Datos;
        private Guna.UI2.WinForms.Guna2Button btn_Seleccionar_Propietario;
        private Guna.UI2.WinForms.Guna2DataGridView dgvRepuestos;
        private Guna.UI2.WinForms.Guna2TextBox txt_Show_Placa;
        private Guna.UI2.WinForms.Guna2TextBox txt_Show_Date;
        private Guna.UI2.WinForms.Guna2TextBox txt_Show_Service;
        public Guna.UI2.WinForms.Guna2TextBox txt_Show_Mecanico_Res;
        private Guna.UI2.WinForms.Guna2TextBox txt_Show_Repuestos;
        private Label label15;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator8;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Label label18;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_forma_de_pago;
        private Label label17;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_metodo_de_pago;
        private Label label16;
        private Label label20;
        private Label label19;
        private Label label22;
        private Label label21;
        private Label label23;
        private Label lbl_sub_total;
        private Label lbl_iva;
        private Label lbl_total_a_pagar;
        private Guna.UI2.WinForms.Guna2TextBox txt_Show_Obsv;
        private Label label25;
        private Label label24;
        public Guna.UI2.WinForms.Guna2TextBox txt_preview_mecanico;
        private Guna.UI2.WinForms.Guna2RadioButton rbtn_si_GM;
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.Cmbox_Select_Plaque = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DTP_Date_Mantenimiento = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.Cmbox_Tip_Service_Realizado = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.rbtn_si_GM = new Guna.UI2.WinForms.Guna2RadioButton();
            this.rbtn_no_GM = new Guna.UI2.WinForms.Guna2RadioButton();
            this.txt_Obsv = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Show_Placa = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Show_Date = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Show_Service = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Show_Mecanico_Res = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Show_Repuestos = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvRepuestos = new Guna.UI2.WinForms.Guna2DataGridView();
            this.btn_Seleccionar_Propietario = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Cargar_Datos = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Guardar_Datos_factura = new Guna.UI2.WinForms.Guna2Button();
            this.label15 = new System.Windows.Forms.Label();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.txt_preview_mecanico = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Separator8 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.lb_valor_servicio = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_sub_total = new System.Windows.Forms.Label();
            this.lbl_iva = new System.Windows.Forms.Label();
            this.lbl_total_a_pagar = new System.Windows.Forms.Label();
            this.txt_Show_Obsv = new Guna.UI2.WinForms.Guna2TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.cmb_forma_de_pago = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cmb_metodo_de_pago = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRepuestos)).BeginInit();
            this.guna2ShadowPanel1.SuspendLayout();
            this.guna2ShadowPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Seleccionar vehículo (por placa)";
            // 
            // Cmbox_Select_Plaque
            // 
            this.Cmbox_Select_Plaque.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Select_Plaque.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Select_Plaque.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Select_Plaque.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Select_Plaque.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Select_Plaque.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Select_Plaque.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Select_Plaque.ItemHeight = 30;
            this.Cmbox_Select_Plaque.Location = new System.Drawing.Point(16, 46);
            this.Cmbox_Select_Plaque.Name = "Cmbox_Select_Plaque";
            this.Cmbox_Select_Plaque.Size = new System.Drawing.Size(302, 36);
            this.Cmbox_Select_Plaque.TabIndex = 5;
            this.Cmbox_Select_Plaque.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(351, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Fecha del mantenimiento";
            // 
            // DTP_Date_Mantenimiento
            // 
            this.DTP_Date_Mantenimiento.Animated = true;
            this.DTP_Date_Mantenimiento.BackColor = System.Drawing.Color.Transparent;
            this.DTP_Date_Mantenimiento.Checked = true;
            this.DTP_Date_Mantenimiento.FillColor = System.Drawing.Color.White;
            this.DTP_Date_Mantenimiento.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.DTP_Date_Mantenimiento.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.DTP_Date_Mantenimiento.Location = new System.Drawing.Point(355, 46);
            this.DTP_Date_Mantenimiento.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.DTP_Date_Mantenimiento.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.DTP_Date_Mantenimiento.Name = "DTP_Date_Mantenimiento";
            this.DTP_Date_Mantenimiento.Size = new System.Drawing.Size(380, 36);
            this.DTP_Date_Mantenimiento.TabIndex = 7;
            this.DTP_Date_Mantenimiento.UseTransparentBackground = true;
            this.DTP_Date_Mantenimiento.Value = new System.DateTime(2025, 7, 31, 22, 43, 37, 433);
            this.DTP_Date_Mantenimiento.ValueChanged += new System.EventHandler(this.DTP_Date_Mantenimiento_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(17, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Tipo de servicio realizado";
            // 
            // Cmbox_Tip_Service_Realizado
            // 
            this.Cmbox_Tip_Service_Realizado.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Tip_Service_Realizado.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Tip_Service_Realizado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Tip_Service_Realizado.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Tip_Service_Realizado.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Tip_Service_Realizado.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.Cmbox_Tip_Service_Realizado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Tip_Service_Realizado.ItemHeight = 30;
            this.Cmbox_Tip_Service_Realizado.Location = new System.Drawing.Point(16, 122);
            this.Cmbox_Tip_Service_Realizado.Name = "Cmbox_Tip_Service_Realizado";
            this.Cmbox_Tip_Service_Realizado.Size = new System.Drawing.Size(302, 36);
            this.Cmbox_Tip_Service_Realizado.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(351, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Mecánico responsable";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(17, 497);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "Observaciones";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(17, 278);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Repuestos Utilizados";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(12, 216);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(158, 20);
            this.label8.TabIndex = 12;
            this.label8.Text = "¿Se utilizó repuestos?";
            // 
            // rbtn_si_GM
            // 
            this.rbtn_si_GM.AutoSize = true;
            this.rbtn_si_GM.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbtn_si_GM.CheckedState.BorderThickness = 0;
            this.rbtn_si_GM.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbtn_si_GM.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbtn_si_GM.CheckedState.InnerOffset = -4;
            this.rbtn_si_GM.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.rbtn_si_GM.Location = new System.Drawing.Point(32, 238);
            this.rbtn_si_GM.Name = "rbtn_si_GM";
            this.rbtn_si_GM.Size = new System.Drawing.Size(39, 24);
            this.rbtn_si_GM.TabIndex = 20;
            this.rbtn_si_GM.Text = "Si";
            this.rbtn_si_GM.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbtn_si_GM.UncheckedState.BorderThickness = 2;
            this.rbtn_si_GM.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbtn_si_GM.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbtn_si_GM.CheckedChanged += new System.EventHandler(this.rbtn_si_GM_CheckedChanged);
            // 
            // rbtn_no_GM
            // 
            this.rbtn_no_GM.AutoSize = true;
            this.rbtn_no_GM.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbtn_no_GM.CheckedState.BorderThickness = 0;
            this.rbtn_no_GM.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbtn_no_GM.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbtn_no_GM.CheckedState.InnerOffset = -4;
            this.rbtn_no_GM.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.rbtn_no_GM.Location = new System.Drawing.Point(95, 238);
            this.rbtn_no_GM.Name = "rbtn_no_GM";
            this.rbtn_no_GM.Size = new System.Drawing.Size(46, 24);
            this.rbtn_no_GM.TabIndex = 21;
            this.rbtn_no_GM.Text = "No";
            this.rbtn_no_GM.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbtn_no_GM.UncheckedState.BorderThickness = 2;
            this.rbtn_no_GM.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbtn_no_GM.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbtn_no_GM.CheckedChanged += new System.EventHandler(this.rdb_NoUsarRepuestos_CheckedChanged);
            // 
            // txt_Obsv
            // 
            this.txt_Obsv.Animated = true;
            this.txt_Obsv.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Obsv.DefaultText = "";
            this.txt_Obsv.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Obsv.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Obsv.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Obsv.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Obsv.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Obsv.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txt_Obsv.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Obsv.Location = new System.Drawing.Point(17, 522);
            this.txt_Obsv.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_Obsv.Multiline = true;
            this.txt_Obsv.Name = "txt_Obsv";
            this.txt_Obsv.PasswordChar = '\0';
            this.txt_Obsv.PlaceholderText = "El vehiculo presentó...";
            this.txt_Obsv.SelectedText = "";
            this.txt_Obsv.Size = new System.Drawing.Size(718, 151);
            this.txt_Obsv.TabIndex = 24;
            // 
            // txt_Show_Placa
            // 
            this.txt_Show_Placa.Animated = true;
            this.txt_Show_Placa.BackColor = System.Drawing.Color.White;
            this.txt_Show_Placa.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Placa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Placa.DefaultText = "";
            this.txt_Show_Placa.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Placa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Placa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Placa.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Placa.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Placa.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Placa.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Placa.Location = new System.Drawing.Point(25, 123);
            this.txt_Show_Placa.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Placa.Multiline = true;
            this.txt_Show_Placa.Name = "txt_Show_Placa";
            this.txt_Show_Placa.PasswordChar = '\0';
            this.txt_Show_Placa.PlaceholderText = "\r\n";
            this.txt_Show_Placa.ReadOnly = true;
            this.txt_Show_Placa.SelectedText = "";
            this.txt_Show_Placa.Size = new System.Drawing.Size(178, 36);
            this.txt_Show_Placa.TabIndex = 76;
            // 
            // txt_Show_Date
            // 
            this.txt_Show_Date.Animated = true;
            this.txt_Show_Date.BackColor = System.Drawing.Color.White;
            this.txt_Show_Date.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Date.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Date.DefaultText = "";
            this.txt_Show_Date.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Date.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Date.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Date.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Date.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Date.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Date.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Date.Location = new System.Drawing.Point(231, 123);
            this.txt_Show_Date.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Date.Multiline = true;
            this.txt_Show_Date.Name = "txt_Show_Date";
            this.txt_Show_Date.PasswordChar = '\0';
            this.txt_Show_Date.PlaceholderText = "\r\n";
            this.txt_Show_Date.ReadOnly = true;
            this.txt_Show_Date.SelectedText = "";
            this.txt_Show_Date.Size = new System.Drawing.Size(178, 36);
            this.txt_Show_Date.TabIndex = 75;
            // 
            // txt_Show_Service
            // 
            this.txt_Show_Service.Animated = true;
            this.txt_Show_Service.BackColor = System.Drawing.Color.White;
            this.txt_Show_Service.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Service.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Service.DefaultText = "";
            this.txt_Show_Service.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Service.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Service.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Service.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Service.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Service.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Service.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Service.Location = new System.Drawing.Point(25, 197);
            this.txt_Show_Service.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Service.Multiline = true;
            this.txt_Show_Service.Name = "txt_Show_Service";
            this.txt_Show_Service.PasswordChar = '\0';
            this.txt_Show_Service.PlaceholderText = "\r\n";
            this.txt_Show_Service.ReadOnly = true;
            this.txt_Show_Service.SelectedText = "";
            this.txt_Show_Service.Size = new System.Drawing.Size(178, 36);
            this.txt_Show_Service.TabIndex = 74;
            // 
            // txt_Show_Mecanico_Res
            // 
            this.txt_Show_Mecanico_Res.Animated = true;
            this.txt_Show_Mecanico_Res.BackColor = System.Drawing.Color.White;
            this.txt_Show_Mecanico_Res.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Mecanico_Res.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Mecanico_Res.DefaultText = "";
            this.txt_Show_Mecanico_Res.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Mecanico_Res.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Mecanico_Res.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Mecanico_Res.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Mecanico_Res.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Mecanico_Res.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Mecanico_Res.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Mecanico_Res.Location = new System.Drawing.Point(231, 197);
            this.txt_Show_Mecanico_Res.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Mecanico_Res.Multiline = true;
            this.txt_Show_Mecanico_Res.Name = "txt_Show_Mecanico_Res";
            this.txt_Show_Mecanico_Res.PasswordChar = '\0';
            this.txt_Show_Mecanico_Res.PlaceholderText = "\r\n";
            this.txt_Show_Mecanico_Res.ReadOnly = true;
            this.txt_Show_Mecanico_Res.SelectedText = "";
            this.txt_Show_Mecanico_Res.Size = new System.Drawing.Size(178, 36);
            this.txt_Show_Mecanico_Res.TabIndex = 73;
            // 
            // txt_Show_Repuestos
            // 
            this.txt_Show_Repuestos.Animated = true;
            this.txt_Show_Repuestos.BackColor = System.Drawing.Color.White;
            this.txt_Show_Repuestos.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Repuestos.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Repuestos.DefaultText = "";
            this.txt_Show_Repuestos.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Repuestos.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Repuestos.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Repuestos.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Repuestos.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Repuestos.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Repuestos.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Repuestos.Location = new System.Drawing.Point(25, 273);
            this.txt_Show_Repuestos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Repuestos.Multiline = true;
            this.txt_Show_Repuestos.Name = "txt_Show_Repuestos";
            this.txt_Show_Repuestos.PasswordChar = '\0';
            this.txt_Show_Repuestos.PlaceholderText = "";
            this.txt_Show_Repuestos.ReadOnly = true;
            this.txt_Show_Repuestos.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Show_Repuestos.SelectedText = "";
            this.txt_Show_Repuestos.Size = new System.Drawing.Size(384, 78);
            this.txt_Show_Repuestos.TabIndex = 72;
            this.txt_Show_Repuestos.TextChanged += new System.EventHandler(this.txt_Show_Repuestos_TextChanged);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.lblTotal.Location = new System.Drawing.Point(20, 579);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(127, 25);
            this.lblTotal.TabIndex = 38;
            this.lblTotal.Text = "Total a Pagar:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(779, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(148, 25);
            this.label7.TabIndex = 26;
            this.label7.Text = "Datos a Facturar";
            // 
            // dgvRepuestos
            // 
            this.dgvRepuestos.AllowUserToAddRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvRepuestos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvRepuestos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvRepuestos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRepuestos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvRepuestos.ColumnHeadersHeight = 4;
            this.dgvRepuestos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRepuestos.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvRepuestos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvRepuestos.Location = new System.Drawing.Point(21, 318);
            this.dgvRepuestos.Name = "dgvRepuestos";
            this.dgvRepuestos.RowHeadersVisible = false;
            this.dgvRepuestos.Size = new System.Drawing.Size(714, 176);
            this.dgvRepuestos.TabIndex = 70;
            this.dgvRepuestos.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvRepuestos.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvRepuestos.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvRepuestos.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvRepuestos.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvRepuestos.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvRepuestos.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvRepuestos.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvRepuestos.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvRepuestos.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvRepuestos.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvRepuestos.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvRepuestos.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvRepuestos.ThemeStyle.ReadOnly = false;
            this.dgvRepuestos.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvRepuestos.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvRepuestos.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvRepuestos.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvRepuestos.ThemeStyle.RowsStyle.Height = 22;
            this.dgvRepuestos.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvRepuestos.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvRepuestos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRepuestos_CellContentClick);
            this.dgvRepuestos.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRepuestos_CellEndEdit);
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
            this.btn_Seleccionar_Propietario.Font = new System.Drawing.Font("Montserrat", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Seleccionar_Propietario.ForeColor = System.Drawing.SystemColors.Window;
            this.btn_Seleccionar_Propietario.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Seleccionar_Propietario.ImageOffset = new System.Drawing.Point(5, -2);
            this.btn_Seleccionar_Propietario.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Seleccionar_Propietario.Location = new System.Drawing.Point(614, 123);
            this.btn_Seleccionar_Propietario.Name = "btn_Seleccionar_Propietario";
            this.btn_Seleccionar_Propietario.ShadowDecoration.BorderRadius = 14;
            this.btn_Seleccionar_Propietario.Size = new System.Drawing.Size(121, 36);
            this.btn_Seleccionar_Propietario.TabIndex = 67;
            this.btn_Seleccionar_Propietario.Text = "Seleccionar";
            this.btn_Seleccionar_Propietario.Click += new System.EventHandler(this.btn_Seleccionar_Propietario_Click);
            // 
            // btn_Cargar_Datos
            // 
            this.btn_Cargar_Datos.Animated = true;
            this.btn_Cargar_Datos.BorderRadius = 8;
            this.btn_Cargar_Datos.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Cargar_Datos.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Cargar_Datos.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Cargar_Datos.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Cargar_Datos.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(208)))), ((int)(((byte)(117)))));
            this.btn_Cargar_Datos.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Cargar_Datos.ForeColor = System.Drawing.Color.White;
            this.btn_Cargar_Datos.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Cargar_Datos.ImageOffset = new System.Drawing.Point(8, -2);
            this.btn_Cargar_Datos.ImageSize = new System.Drawing.Size(26, 26);
            this.btn_Cargar_Datos.Location = new System.Drawing.Point(113, 634);
            this.btn_Cargar_Datos.Name = "btn_Cargar_Datos";
            this.btn_Cargar_Datos.ShadowDecoration.BorderRadius = 14;
            this.btn_Cargar_Datos.Size = new System.Drawing.Size(147, 38);
            this.btn_Cargar_Datos.TabIndex = 36;
            this.btn_Cargar_Datos.Text = "Cargar Datos";
            this.btn_Cargar_Datos.Click += new System.EventHandler(this.btn_Guardar_Datos_factura_Click);
            // 
            // btn_Guardar_Datos_factura
            // 
            this.btn_Guardar_Datos_factura.Animated = true;
            this.btn_Guardar_Datos_factura.BorderRadius = 8;
            this.btn_Guardar_Datos_factura.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Guardar_Datos_factura.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Guardar_Datos_factura.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Guardar_Datos_factura.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Guardar_Datos_factura.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Guardar_Datos_factura.ForeColor = System.Drawing.Color.White;
            this.btn_Guardar_Datos_factura.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Guardar_Datos_factura.ImageOffset = new System.Drawing.Point(8, -1);
            this.btn_Guardar_Datos_factura.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Guardar_Datos_factura.Location = new System.Drawing.Point(266, 634);
            this.btn_Guardar_Datos_factura.Name = "btn_Guardar_Datos_factura";
            this.btn_Guardar_Datos_factura.ShadowDecoration.BorderRadius = 14;
            this.btn_Guardar_Datos_factura.Size = new System.Drawing.Size(143, 38);
            this.btn_Guardar_Datos_factura.TabIndex = 32;
            this.btn_Guardar_Datos_factura.Text = "Guardar";
            this.btn_Guardar_Datos_factura.Click += new System.EventHandler(this.btn_Grabar_Datos_factura_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(20, 31);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(220, 25);
            this.label15.TabIndex = 71;
            this.label15.Text = "Datos de mantenimiento";
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.txt_preview_mecanico);
            this.guna2ShadowPanel1.Controls.Add(this.guna2Separator8);
            this.guna2ShadowPanel1.Controls.Add(this.label1);
            this.guna2ShadowPanel1.Controls.Add(this.dgvRepuestos);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Select_Plaque);
            this.guna2ShadowPanel1.Controls.Add(this.label2);
            this.guna2ShadowPanel1.Controls.Add(this.txt_Obsv);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Seleccionar_Propietario);
            this.guna2ShadowPanel1.Controls.Add(this.label5);
            this.guna2ShadowPanel1.Controls.Add(this.DTP_Date_Mantenimiento);
            this.guna2ShadowPanel1.Controls.Add(this.label3);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Tip_Service_Realizado);
            this.guna2ShadowPanel1.Controls.Add(this.label6);
            this.guna2ShadowPanel1.Controls.Add(this.rbtn_no_GM);
            this.guna2ShadowPanel1.Controls.Add(this.rbtn_si_GM);
            this.guna2ShadowPanel1.Controls.Add(this.label4);
            this.guna2ShadowPanel1.Controls.Add(this.label8);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(17, 65);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(752, 694);
            this.guna2ShadowPanel1.TabIndex = 72;
            // 
            // txt_preview_mecanico
            // 
            this.txt_preview_mecanico.Animated = true;
            this.txt_preview_mecanico.BackColor = System.Drawing.Color.White;
            this.txt_preview_mecanico.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_preview_mecanico.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_preview_mecanico.DefaultText = "";
            this.txt_preview_mecanico.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_preview_mecanico.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_preview_mecanico.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_preview_mecanico.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_preview_mecanico.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_preview_mecanico.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_preview_mecanico.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_preview_mecanico.Location = new System.Drawing.Point(357, 123);
            this.txt_preview_mecanico.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_preview_mecanico.Multiline = true;
            this.txt_preview_mecanico.Name = "txt_preview_mecanico";
            this.txt_preview_mecanico.PasswordChar = '\0';
            this.txt_preview_mecanico.PlaceholderText = "\r\n";
            this.txt_preview_mecanico.ReadOnly = true;
            this.txt_preview_mecanico.SelectedText = "";
            this.txt_preview_mecanico.Size = new System.Drawing.Size(250, 36);
            this.txt_preview_mecanico.TabIndex = 88;
            // 
            // guna2Separator8
            // 
            this.guna2Separator8.Location = new System.Drawing.Point(15, 184);
            this.guna2Separator8.Name = "guna2Separator8";
            this.guna2Separator8.Size = new System.Drawing.Size(720, 20);
            this.guna2Separator8.TabIndex = 68;
            // 
            // guna2ShadowPanel2
            // 
            this.guna2ShadowPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel2.Controls.Add(this.lb_valor_servicio);
            this.guna2ShadowPanel2.Controls.Add(this.label10);
            this.guna2ShadowPanel2.Controls.Add(this.lbl_sub_total);
            this.guna2ShadowPanel2.Controls.Add(this.lbl_iva);
            this.guna2ShadowPanel2.Controls.Add(this.lbl_total_a_pagar);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Obsv);
            this.guna2ShadowPanel2.Controls.Add(this.btn_Cargar_Datos);
            this.guna2ShadowPanel2.Controls.Add(this.btn_Guardar_Datos_factura);
            this.guna2ShadowPanel2.Controls.Add(this.label25);
            this.guna2ShadowPanel2.Controls.Add(this.label24);
            this.guna2ShadowPanel2.Controls.Add(this.label23);
            this.guna2ShadowPanel2.Controls.Add(this.label22);
            this.guna2ShadowPanel2.Controls.Add(this.label21);
            this.guna2ShadowPanel2.Controls.Add(this.label20);
            this.guna2ShadowPanel2.Controls.Add(this.label19);
            this.guna2ShadowPanel2.Controls.Add(this.lblTotal);
            this.guna2ShadowPanel2.Controls.Add(this.label18);
            this.guna2ShadowPanel2.Controls.Add(this.cmb_forma_de_pago);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Repuestos);
            this.guna2ShadowPanel2.Controls.Add(this.label17);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Mecanico_Res);
            this.guna2ShadowPanel2.Controls.Add(this.cmb_metodo_de_pago);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Service);
            this.guna2ShadowPanel2.Controls.Add(this.label16);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Date);
            this.guna2ShadowPanel2.Controls.Add(this.txt_Show_Placa);
            this.guna2ShadowPanel2.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel2.Location = new System.Drawing.Point(775, 65);
            this.guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            this.guna2ShadowPanel2.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel2.Size = new System.Drawing.Size(427, 694);
            this.guna2ShadowPanel2.TabIndex = 73;
            // 
            // lb_valor_servicio
            // 
            this.lb_valor_servicio.AutoSize = true;
            this.lb_valor_servicio.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.lb_valor_servicio.Location = new System.Drawing.Point(358, 474);
            this.lb_valor_servicio.Name = "lb_valor_servicio";
            this.lb_valor_servicio.Size = new System.Drawing.Size(51, 20);
            this.lb_valor_servicio.TabIndex = 89;
            this.lb_valor_servicio.Text = "$ 0.00";
            this.lb_valor_servicio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(21, 474);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 20);
            this.label10.TabIndex = 88;
            this.label10.Text = "Servicio realizado";
            // 
            // lbl_sub_total
            // 
            this.lbl_sub_total.AutoSize = true;
            this.lbl_sub_total.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.lbl_sub_total.Location = new System.Drawing.Point(358, 507);
            this.lbl_sub_total.Name = "lbl_sub_total";
            this.lbl_sub_total.Size = new System.Drawing.Size(51, 20);
            this.lbl_sub_total.TabIndex = 87;
            this.lbl_sub_total.Text = "$ 0.00";
            this.lbl_sub_total.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_iva
            // 
            this.lbl_iva.AutoSize = true;
            this.lbl_iva.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.lbl_iva.Location = new System.Drawing.Point(358, 540);
            this.lbl_iva.Name = "lbl_iva";
            this.lbl_iva.Size = new System.Drawing.Size(51, 20);
            this.lbl_iva.TabIndex = 86;
            this.lbl_iva.Text = "$ 0.00";
            this.lbl_iva.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_total_a_pagar
            // 
            this.lbl_total_a_pagar.AutoSize = true;
            this.lbl_total_a_pagar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_total_a_pagar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.lbl_total_a_pagar.Location = new System.Drawing.Point(350, 581);
            this.lbl_total_a_pagar.Name = "lbl_total_a_pagar";
            this.lbl_total_a_pagar.Size = new System.Drawing.Size(59, 20);
            this.lbl_total_a_pagar.TabIndex = 85;
            this.lbl_total_a_pagar.Text = "$ 0.00";
            this.lbl_total_a_pagar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_Show_Obsv
            // 
            this.txt_Show_Obsv.Animated = true;
            this.txt_Show_Obsv.BackColor = System.Drawing.Color.White;
            this.txt_Show_Obsv.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_Show_Obsv.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Show_Obsv.DefaultText = "";
            this.txt_Show_Obsv.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Show_Obsv.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Show_Obsv.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Obsv.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Show_Obsv.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Obsv.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_Show_Obsv.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Show_Obsv.Location = new System.Drawing.Point(25, 381);
            this.txt_Show_Obsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Show_Obsv.Multiline = true;
            this.txt_Show_Obsv.Name = "txt_Show_Obsv";
            this.txt_Show_Obsv.PasswordChar = '\0';
            this.txt_Show_Obsv.PlaceholderText = "";
            this.txt_Show_Obsv.ReadOnly = true;
            this.txt_Show_Obsv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Show_Obsv.SelectedText = "";
            this.txt_Show_Obsv.Size = new System.Drawing.Size(384, 78);
            this.txt_Show_Obsv.TabIndex = 84;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(21, 540);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(60, 20);
            this.label25.TabIndex = 83;
            this.label25.Text = "IVA 15%";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label24.Location = new System.Drawing.Point(21, 507);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(126, 20);
            this.label24.TabIndex = 82;
            this.label24.Text = "Sub total a pagar";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(21, 356);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(109, 20);
            this.label23.TabIndex = 81;
            this.label23.Text = "Observaciones";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label22.Location = new System.Drawing.Point(21, 248);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(151, 20);
            this.label22.TabIndex = 80;
            this.label22.Text = "Repuestos utilizados";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(227, 172);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(160, 20);
            this.label21.TabIndex = 79;
            this.label21.Text = "Mecánico responsable";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(21, 172);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(115, 20);
            this.label20.TabIndex = 78;
            this.label20.Text = "Tipo de servicio";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(227, 98);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(182, 20);
            this.label19.TabIndex = 77;
            this.label19.Text = "Fecha de mantenimiento";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(21, 98);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 20);
            this.label18.TabIndex = 74;
            this.label18.Text = "Placa";
            // 
            // cmb_forma_de_pago
            // 
            this.cmb_forma_de_pago.BackColor = System.Drawing.Color.Transparent;
            this.cmb_forma_de_pago.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_forma_de_pago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_forma_de_pago.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_forma_de_pago.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_forma_de_pago.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_forma_de_pago.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_forma_de_pago.ItemHeight = 30;
            this.cmb_forma_de_pago.Location = new System.Drawing.Point(231, 46);
            this.cmb_forma_de_pago.Name = "cmb_forma_de_pago";
            this.cmb_forma_de_pago.Size = new System.Drawing.Size(178, 36);
            this.cmb_forma_de_pago.TabIndex = 73;
            this.cmb_forma_de_pago.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(227, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(111, 20);
            this.label17.TabIndex = 72;
            this.label17.Text = "Forma de pago";
            // 
            // cmb_metodo_de_pago
            // 
            this.cmb_metodo_de_pago.BackColor = System.Drawing.Color.Transparent;
            this.cmb_metodo_de_pago.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_metodo_de_pago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_metodo_de_pago.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_metodo_de_pago.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_metodo_de_pago.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_metodo_de_pago.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_metodo_de_pago.ItemHeight = 30;
            this.cmb_metodo_de_pago.Location = new System.Drawing.Point(25, 46);
            this.cmb_metodo_de_pago.Name = "cmb_metodo_de_pago";
            this.cmb_metodo_de_pago.Size = new System.Drawing.Size(178, 36);
            this.cmb_metodo_de_pago.TabIndex = 71;
            this.cmb_metodo_de_pago.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Montserrat SemiBold", 9.749999F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(21, 18);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(119, 20);
            this.label16.TabIndex = 27;
            this.label16.Text = "Método de pago";
            // 
            // Formulario_Mantenimiento
            // 
            this.ClientSize = new System.Drawing.Size(1212, 821);
            this.Controls.Add(this.guna2ShadowPanel2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.guna2ShadowPanel1);
            this.Controls.Add(this.label15);
            this.Name = "Formulario_Mantenimiento";
            this.Text = "Gestión de Mantenimiento";
            this.Load += new System.EventHandler(this.Formulario_Mantenimiento_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRepuestos)).EndInit();
            this.guna2ShadowPanel1.ResumeLayout(false);
            this.guna2ShadowPanel1.PerformLayout();
            this.guna2ShadowPanel2.ResumeLayout(false);
            this.guna2ShadowPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
