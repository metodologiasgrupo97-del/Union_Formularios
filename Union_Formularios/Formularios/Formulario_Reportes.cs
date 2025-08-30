using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using ClosedXML.Excel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_Reportes : Form
    {
        private int? _selectedUserId;
       
        public Formulario_Reportes()
        {
            InitializeComponent();
            ConfigurarDtpNullable(dtp_Fecha_Inicial);
            ConfigurarDtpNullable(dtp_Fecha_Final);
            ConfigurarGrid();
            if (this.Controls.Find("cmb_Comprobante_Reporte", true).FirstOrDefault() is ComboBox cmbComp)
            {
                cmbComp.Items.Clear();
                cmbComp.Items.AddRange(new object[] { "Tarjeta", "Efectivo", "Transferencia" });
                cmbComp.SelectedIndex = -1;
            }
            txt_select_mecanico.ReadOnly = true;
            
            this.Load += Reportes_Load;
            btn_Aplicar_Filtros.Click += btn_Aplicar_Filtros_Click;
            btn_Limpiar_filtros.Click += btn_Limpiar_filtros_Click;
            btn_Export_xlsx.Click += btn_Export_xlsx_Click;

            btn_Seleccionar_Trabajador.Click += btn_Seleccionar_Trabajador_Click;
            if (this.Controls.Find("btn_Seleccionar_Propietario_Reporte", true).FirstOrDefault() is Button bAlt)
                bAlt.Click += btn_Seleccionar_Trabajador_Click;
        }

        private readonly string _connStr = "Data Source=.;Initial Catalog=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True";
        private readonly BindingSource _bs = new BindingSource();
        private DataTable _dt = new DataTable();

        private void ConfigurarDtpNullable(Guna.UI2.WinForms.Guna2DateTimePicker dtp)
        {
            dtp.Checked = false;
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = " ";
            dtp.ValueChanged += (s, e) =>
            {
                dtp.CustomFormat = "dd/MM/yyyy"; 
                dtp.Checked = true;
            };
        }
        private void Reportes_Load(object sender, EventArgs e)
        {
            try
            {
                CargarCombos();
                CargarReportes();   
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar el módulo de reportes:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarGrid()
        {
            dgvReportes.ReadOnly = true;
            dgvReportes.MultiSelect = false;
            dgvReportes.AllowUserToAddRows = false;
            dgvReportes.AllowUserToDeleteRows = false;
            dgvReportes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReportes.AutoGenerateColumns = true; // usaremos alias desde el SELECT
            dgvReportes.DataSource = _bs;
            dgvReportes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            dgvReportes.EnableHeadersVisualStyles = false;
            dgvReportes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReportes.ColumnHeadersHeight = 32;
            dgvReportes.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvReportes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // =================== Carga de filtros ===================
        private void CargarCombos()
        {
            try
            {
                using (var cn = new SqlConnection(_connStr))
                {
                    cn.Open();

                    using (var da = new SqlDataAdapter(@"SELECT DISTINCT v.Placa FROM Vehiculos v INNER JOIN Facturas f ON f.VehicleID = v.VehicleID ORDER BY v.Placa;", cn))
                    {
                        var t = new DataTable();
                        da.Fill(t);
                        Cmbox_Select_Placa.DataSource = t;
                        Cmbox_Select_Placa.DisplayMember = "Placa";
                        Cmbox_Select_Placa.ValueMember = "Placa";
                        Cmbox_Select_Placa.SelectedIndex = -1; // vacío
                        Cmbox_Select_Placa.Text = string.Empty;
                    }


                    // Placas
                    using (var da = new SqlDataAdapter(@"SELECT DISTINCT v.Placa FROM Vehiculos v INNER JOIN Facturas f ON f.VehicleID = v.VehicleID ORDER BY v.Placa;", cn))
                    {
                        var t = new DataTable();
                        da.Fill(t);
                        Cmbox_Select_Placa.DataSource = t;
                        Cmbox_Select_Placa.DisplayMember = "Placa";
                        Cmbox_Select_Placa.ValueMember = "Placa";
                        Cmbox_Select_Placa.SelectedIndex = -1; 
                    }

                    // Códigos de factura
                    using (var da = new SqlDataAdapter(
                        "SELECT CodigoFactura FROM Facturas ORDER BY CodigoFactura DESC", cn))
                    {
                        var t = new DataTable();
                        da.Fill(t);
                        cmb_Codigo_Factura_Reporte.DataSource = t;
                        cmb_Codigo_Factura_Reporte.DisplayMember = "CodigoFactura";
                        cmb_Codigo_Factura_Reporte.ValueMember = "CodigoFactura";
                        cmb_Codigo_Factura_Reporte.SelectedIndex = -1;
                    }

                    // Servicios registrados (desde Facturas.TipoServicio)
                    using (var da = new SqlDataAdapter(
                        "SELECT DISTINCT TipoServicio FROM Facturas WHERE ISNULL(TipoServicio,'')<>'' ORDER BY TipoServicio", cn))
                    {
                        var t = new DataTable();
                        da.Fill(t);
                        cmb_Servicio_realizado_Reporte.DataSource = t;
                        cmb_Servicio_realizado_Reporte.DisplayMember = "TipoServicio";
                        cmb_Servicio_realizado_Reporte.ValueMember = "TipoServicio";
                        cmb_Servicio_realizado_Reporte.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los filtros:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =================== Query principal ===================
        private void CargarReportes()
        {
            try
            {
                var sql = new StringBuilder(@"
                SELECT
                f.CodigoFactura                                       AS [CodigoFactura],
                f.FechaMantenimiento                                  AS [Fecha del mantenimiento],
                v.Placa                                               AS [Placa],
                (p.Nombre + ' ' + p.Apellido)                         AS [Propietario],
                ISNULL(NULLIF(f.TipoServicio,''),'(Sin registro)')    AS [Servicio],
                ISNULL(u.FirstName + ' ' + u.LastName,'(Sin técnico)')AS [Tecnico],
                    f.Total                                               AS [Total]
                FROM Facturas f
                JOIN Vehiculos v     ON v.VehicleID = f.VehicleID
                JOIN Propietarios p  ON p.ID_Propietario = f.ID_Propietario
                LEFT JOIN Users u    ON u.UserID = f.UserID
                WHERE 1=1");

                var cmdParams = new System.Collections.Generic.List<SqlParameter>();

                DateTime? ini = dtp_Fecha_Inicial.Checked ? dtp_Fecha_Inicial.Value.Date : (DateTime?)null;
                DateTime? fin = dtp_Fecha_Final.Checked ? dtp_Fecha_Final.Value.Date : (DateTime?)null;

                if (fin.HasValue && !ini.HasValue)
                {
                    MessageBox.Show("Para usar 'Fecha hasta', primero selecciona 'Fecha desde'.",
                        "Filtros", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (ini.HasValue && !fin.HasValue)
                {
                    fin = DateTime.Today; 
                }
                if (ini.HasValue && fin.HasValue)
                {
                    if (fin.Value < ini.Value)
                    {
                        MessageBox.Show("La fecha final no puede ser menor a la inicial.",
                            "Filtros", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    sql.AppendLine("  AND f.FechaMantenimiento BETWEEN @FIni AND @FFin");
                    cmdParams.Add(new SqlParameter("@FIni", ini.Value));
                    cmdParams.Add(new SqlParameter("@FFin", fin.Value));
                }

                if (Cmbox_Select_Placa.SelectedIndex >= 0)
                {
                    sql.AppendLine("  AND v.Placa = @Placa");
                    cmdParams.Add(new SqlParameter("@Placa", Cmbox_Select_Placa.SelectedValue.ToString()));
                }

                if (_selectedUserId.HasValue)
                {
                    sql.AppendLine("  AND f.UserID = @UserID");
                    cmdParams.Add(new SqlParameter("@UserID", _selectedUserId.Value));
                }
                else if (!string.IsNullOrWhiteSpace(txt_select_mecanico.Text))
                {
                    sql.AppendLine("  AND ISNULL(u.FirstName + ' ' + u.LastName,'') LIKE @Mec");
                    cmdParams.Add(new SqlParameter("@Mec", "%" + txt_select_mecanico.Text.Trim() + "%"));
                }


                if (!string.IsNullOrWhiteSpace(txt_select_mecanico.Text))
                {
                    sql.AppendLine("  AND ISNULL(u.FirstName + ' ' + u.LastName,'') LIKE @Mec");
                    cmdParams.Add(new SqlParameter("@Mec", "%" + txt_select_mecanico.Text.Trim() + "%"));
                }

                ComboBox cmbComp = this.Controls.Find("cmb_Comprobante_Reporte", true).FirstOrDefault() as ComboBox;
                if (cmbComp != null && cmbComp.SelectedIndex >= 0)
                {
                    sql.AppendLine("  AND f.MetodoPago = @MetodoPago");
                    cmdParams.Add(new SqlParameter("@MetodoPago", cmbComp.SelectedItem.ToString()));
                }

                if (cmb_Codigo_Factura_Reporte.SelectedIndex >= 0)
                {
                    sql.AppendLine("  AND f.CodigoFactura = @CodFac");
                    cmdParams.Add(new SqlParameter("@CodFac", cmb_Codigo_Factura_Reporte.SelectedValue.ToString()));
                }

                // Servicio realizado
                if (cmb_Servicio_realizado_Reporte.SelectedIndex >= 0)
                {
                    sql.AppendLine("  AND f.TipoServicio = @Srv");
                    cmdParams.Add(new SqlParameter("@Srv", cmb_Servicio_realizado_Reporte.SelectedValue.ToString()));
                }

                sql.AppendLine("ORDER BY f.FechaMantenimiento DESC, f.CodigoFactura DESC");

                using (var cn = new SqlConnection(_connStr))
                using (var da = new SqlDataAdapter(sql.ToString(), cn))
                {
                    foreach (var p in cmdParams) da.SelectCommand.Parameters.Add(p);
                    var t = new DataTable();
                    da.Fill(t);
                    _dt = t;
                    _bs.DataSource = _dt;

                    // Formato de moneda para la columna Total
                    if (dgvReportes.Columns.Contains("Total"))
                        dgvReportes.Columns["Total"].DefaultCellStyle.Format = "C2";

                    // KPI
                    ActualizarKpis();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarKpis()
        {
            try
            {
                var filas = _dt?.Rows?.Count ?? 0;
                lbl_Ordenes.Text = filas.ToString();

                decimal total = 0m;
                if (_dt != null && _dt.Columns.Contains("Total"))
                {
                    total = _dt.AsEnumerable()
                               .Where(r => r["Total"] != DBNull.Value)
                               .Sum(r => r.Field<decimal>("Total"));
                }
                lbl_Ingresos_totales.Text = total.ToString("C2");

                var topServicio = _dt?.AsEnumerable()
                    .Select(r => Convert.ToString(r["Servicio"]))
                    .Where(s => !string.IsNullOrWhiteSpace(s) && s != "(Sin registro)")
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count())
                    .Select(g => $"{g.Key}")
                    .FirstOrDefault();

                Nom_Serv_moda.Text = string.IsNullOrEmpty(topServicio) ? "—" : topServicio;
            }
            catch (Exception ex)
            {
                // No caemos si hay problema de conversión
                Nom_Serv_moda.Text = "—";
                MessageBox.Show("Error al calcular KPIs:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =================== Handlers de botones ===================
        private void btn_Aplicar_Filtros_Click(object sender, EventArgs e)
        {
            CargarReportes();
            if (_dt != null && _dt.Rows.Count == 0
        && Cmbox_Select_Placa.SelectedIndex >= 0
        && cmb_Codigo_Factura_Reporte.SelectedIndex >= 0)
    {
        Cmbox_Select_Placa.SelectedIndex = -1;
        Cmbox_Select_Placa.Text = string.Empty;
        CargarReportes();
        MessageBox.Show("El código de factura no pertenece a la placa seleccionada. Se limpió la placa.",
            "Filtros", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
        }

        private void btn_Limpiar_filtros_Click(object sender, EventArgs e)
        {
            try
            {
                // Vaciar todos
                dtp_Fecha_Inicial.Checked = false; dtp_Fecha_Inicial.CustomFormat = " ";
                dtp_Fecha_Final.Checked = false; dtp_Fecha_Final.CustomFormat = " ";
                Cmbox_Select_Placa.SelectedIndex = -1;
                txt_select_mecanico.Clear();
                var cmbComp = this.Controls.Find("cmb_Comprobante_Reporte", true).FirstOrDefault() as ComboBox;
                if (cmbComp != null) cmbComp.SelectedIndex = -1;
                cmb_Codigo_Factura_Reporte.SelectedIndex = -1;
                cmb_Servicio_realizado_Reporte.SelectedIndex = -1;

                CargarReportes(); // todo

                if (_dt != null && _dt.Rows.Count == 0 && Cmbox_Select_Placa.SelectedIndex >= 0 && cmb_Codigo_Factura_Reporte.SelectedIndex >= 0)
                {
                    Cmbox_Select_Placa.SelectedIndex = -1;
                    CargarReportes();
                    MessageBox.Show(
                        "El código de factura no pertenece a la placa seleccionada. Se limpió la placa.",
                        "Filtros", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron limpiar los filtros:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_Seleccionar_Trabajador_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = new Union_Formularios.Formularios.Formulario_Seleccionar_Trabajador();
                sel.TrabajadorSeleccionado += (id, nombre) =>
                {
                    _selectedUserId = id;                
                    txt_select_mecanico.Text = nombre;  
                };
                sel.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el selector:\n" + ex.Message,
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SetTrabajadorSeleccionado(string nombreCompleto)
        {
            txt_select_mecanico.Text = nombreCompleto;
        }

        private bool _isExporting;

        private void btn_Export_xlsx_Click(object sender, EventArgs e)
        {
            if (_isExporting) return;
            _isExporting = true;
            try
            {
                if (_dt == null || _dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var sfd = new SaveFileDialog()
                {
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = $"Reporte_Car-EFULL_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.AddWorksheet("Reportes");

                        // ===== Encabezado =====
                        ws.Cell("C1").Value = "Reporte — Car-EFULL";
                        ws.Cell("C1").Style.Font.Bold = true;
                        ws.Cell("C1").Style.Font.FontSize = 16;

                        ws.Cell("C2").Value = "Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        ws.Cell("C3").Value = "Filtros: " + ResumenFiltros();

                        // ===== Tabla de datos desde A5 =====
                        var tabla = ws.Cell(5, 1).InsertTable(_dt, "TablaReportes", true);
                        tabla.Theme = XLTableTheme.TableStyleMedium6;

                        // Formato moneda a la columna Total (si existe)
                        int colTotal = -1;
                        for (int i = 0; i < _dt.Columns.Count; i++)
                            if (string.Equals(_dt.Columns[i].ColumnName, "Total", StringComparison.OrdinalIgnoreCase))
                            { colTotal = i + 1; break; }

                        if (colTotal > 0)
                            ws.Column(colTotal).Style.NumberFormat.Format = "$#,##0.00";

                        // Autofit
                        ws.Columns().AdjustToContents();

                        // Guardar archivo
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Exportación exitosa.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel (ClosedXML):\n" + ex.Message,
                    "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _isExporting = false; }
        }


        private string ResumenFiltros()
        {
            string fn(object o) => string.IsNullOrWhiteSpace(Convert.ToString(o)) ? "—" : Convert.ToString(o);

            var cmbComp = this.Controls.Find("cmb_Comprobante_Reporte", true).FirstOrDefault() as ComboBox;
            string comp = (cmbComp != null && cmbComp.SelectedIndex >= 0) ? cmbComp.SelectedItem.ToString() : "Todos";

            string fIni = dtp_Fecha_Inicial.Checked ? dtp_Fecha_Inicial.Value.ToString("dd/MM/yyyy") : "—";
            string fFin = dtp_Fecha_Final.Checked ? dtp_Fecha_Final.Value.ToString("dd/MM/yyyy") : (dtp_Fecha_Inicial.Checked ? DateTime.Today.ToString("dd/MM/yyyy") : "—");

            return $"Desde: {fIni} | Hasta: {fFin} | Placa: {fn(Cmbox_Select_Placa.Text)} | Mecánico: {fn(txt_select_mecanico.Text)} | Comprobante: {comp} | " +
                   $"Código Factura: {fn(cmb_Codigo_Factura_Reporte.Text)} | Servicio: {fn(cmb_Servicio_realizado_Reporte.Text)}";
        }








        private Label label1;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Label label5;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Fecha_Final;
        private Label label4;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Fecha_Inicial;
        private Label label3;
        private Label label2;
        private Guna.UI2.WinForms.Guna2Button btn_Export_xlsx;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private Label lbl_Cont_Trab;
        private Label lbl_Ordenes;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel3;
        private Label label10;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel3;
        private Label label12;
        private Label label8;
        private Label Nom_Serv_moda;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel2;
        private Label label11;
        private Label label6;
        private Label lbl_Ingresos_totales;
        private Guna.UI2.WinForms.Guna2ComboBox Cmbox_Select_Placa;
        private Label label13;
        private Label label15;
        private Label label14;
        public Guna.UI2.WinForms.Guna2TextBox txt_select_mecanico;
        private Guna.UI2.WinForms.Guna2Button btn_Seleccionar_Trabajador;
        private Label label16;
        private Guna.UI2.WinForms.Guna2Button btn_Limpiar_filtros;
        private Guna.UI2.WinForms.Guna2Button btn_Aplicar_Filtros;
        private Guna.UI2.WinForms.Guna2DataGridView dgvReportes;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_Comprobante;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_Servicio_realizado_Reporte;
        private Label label9;
        private Guna.UI2.WinForms.Guna2ComboBox cmb_Codigo_Factura_Reporte;
        private Label label7;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.cmb_Servicio_realizado_Reporte = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmb_Codigo_Factura_Reporte = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_Limpiar_filtros = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Aplicar_Filtros = new Guna.UI2.WinForms.Guna2Button();
            this.cmb_Comprobante = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btn_Seleccionar_Trabajador = new Guna.UI2.WinForms.Guna2Button();
            this.txt_select_mecanico = new Guna.UI2.WinForms.Guna2TextBox();
            this.Cmbox_Select_Placa = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtp_Fecha_Final = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtp_Fecha_Inicial = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.dgvReportes = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_Cont_Trab = new System.Windows.Forms.Label();
            this.lbl_Ordenes = new System.Windows.Forms.Label();
            this.guna2ShadowPanel3 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.guna2GradientPanel3 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Nom_Serv_moda = new System.Windows.Forms.Label();
            this.guna2GradientPanel2 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbl_Ingresos_totales = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.btn_Export_xlsx = new Guna.UI2.WinForms.Guna2Button();
            this.guna2ShadowPanel1.SuspendLayout();
            this.guna2ShadowPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportes)).BeginInit();
            this.guna2GradientPanel1.SuspendLayout();
            this.guna2ShadowPanel3.SuspendLayout();
            this.guna2GradientPanel3.SuspendLayout();
            this.guna2GradientPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 25);
            this.label1.TabIndex = 44;
            this.label1.Text = "Panel de reportes";
            // 
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.cmb_Servicio_realizado_Reporte);
            this.guna2ShadowPanel1.Controls.Add(this.label9);
            this.guna2ShadowPanel1.Controls.Add(this.cmb_Codigo_Factura_Reporte);
            this.guna2ShadowPanel1.Controls.Add(this.label7);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Limpiar_filtros);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Aplicar_Filtros);
            this.guna2ShadowPanel1.Controls.Add(this.cmb_Comprobante);
            this.guna2ShadowPanel1.Controls.Add(this.label16);
            this.guna2ShadowPanel1.Controls.Add(this.btn_Seleccionar_Trabajador);
            this.guna2ShadowPanel1.Controls.Add(this.txt_select_mecanico);
            this.guna2ShadowPanel1.Controls.Add(this.Cmbox_Select_Placa);
            this.guna2ShadowPanel1.Controls.Add(this.label13);
            this.guna2ShadowPanel1.Controls.Add(this.label14);
            this.guna2ShadowPanel1.Controls.Add(this.label5);
            this.guna2ShadowPanel1.Controls.Add(this.dtp_Fecha_Final);
            this.guna2ShadowPanel1.Controls.Add(this.label4);
            this.guna2ShadowPanel1.Controls.Add(this.dtp_Fecha_Inicial);
            this.guna2ShadowPanel1.Controls.Add(this.label3);
            this.guna2ShadowPanel1.Controls.Add(this.label2);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(17, 59);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(295, 692);
            this.guna2ShadowPanel1.TabIndex = 45;
            // 
            // cmb_Servicio_realizado_Reporte
            // 
            this.cmb_Servicio_realizado_Reporte.BackColor = System.Drawing.Color.Transparent;
            this.cmb_Servicio_realizado_Reporte.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_Servicio_realizado_Reporte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Servicio_realizado_Reporte.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Servicio_realizado_Reporte.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Servicio_realizado_Reporte.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_Servicio_realizado_Reporte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_Servicio_realizado_Reporte.ItemHeight = 30;
            this.cmb_Servicio_realizado_Reporte.Location = new System.Drawing.Point(20, 506);
            this.cmb_Servicio_realizado_Reporte.Name = "cmb_Servicio_realizado_Reporte";
            this.cmb_Servicio_realizado_Reporte.Size = new System.Drawing.Size(258, 36);
            this.cmb_Servicio_realizado_Reporte.TabIndex = 97;
            this.cmb_Servicio_realizado_Reporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(17, 485);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 18);
            this.label9.TabIndex = 96;
            this.label9.Text = "Servicio realizado";
            // 
            // cmb_Codigo_Factura_Reporte
            // 
            this.cmb_Codigo_Factura_Reporte.BackColor = System.Drawing.Color.Transparent;
            this.cmb_Codigo_Factura_Reporte.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_Codigo_Factura_Reporte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Codigo_Factura_Reporte.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Codigo_Factura_Reporte.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Codigo_Factura_Reporte.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_Codigo_Factura_Reporte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_Codigo_Factura_Reporte.ItemHeight = 30;
            this.cmb_Codigo_Factura_Reporte.Location = new System.Drawing.Point(20, 434);
            this.cmb_Codigo_Factura_Reporte.Name = "cmb_Codigo_Factura_Reporte";
            this.cmb_Codigo_Factura_Reporte.Size = new System.Drawing.Size(258, 36);
            this.cmb_Codigo_Factura_Reporte.TabIndex = 95;
            this.cmb_Codigo_Factura_Reporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(17, 413);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 18);
            this.label7.TabIndex = 94;
            this.label7.Text = "Codigo factura";
            // 
            // btn_Limpiar_filtros
            // 
            this.btn_Limpiar_filtros.Animated = true;
            this.btn_Limpiar_filtros.BorderRadius = 8;
            this.btn_Limpiar_filtros.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Limpiar_filtros.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Limpiar_filtros.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Limpiar_filtros.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Limpiar_filtros.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Limpiar_filtros.ForeColor = System.Drawing.Color.White;
            this.btn_Limpiar_filtros.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Limpiar_filtros.ImageOffset = new System.Drawing.Point(8, -1);
            this.btn_Limpiar_filtros.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Limpiar_filtros.Location = new System.Drawing.Point(20, 625);
            this.btn_Limpiar_filtros.Name = "btn_Limpiar_filtros";
            this.btn_Limpiar_filtros.ShadowDecoration.BorderRadius = 14;
            this.btn_Limpiar_filtros.Size = new System.Drawing.Size(258, 38);
            this.btn_Limpiar_filtros.TabIndex = 37;
            this.btn_Limpiar_filtros.Text = "Limpiar";
            this.btn_Limpiar_filtros.Click += new System.EventHandler(this.btn_Limpiar_filtros_Click);
            // 
            // btn_Aplicar_Filtros
            // 
            this.btn_Aplicar_Filtros.Animated = true;
            this.btn_Aplicar_Filtros.BorderRadius = 8;
            this.btn_Aplicar_Filtros.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Aplicar_Filtros.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Aplicar_Filtros.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Aplicar_Filtros.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Aplicar_Filtros.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(208)))), ((int)(((byte)(117)))));
            this.btn_Aplicar_Filtros.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Aplicar_Filtros.ForeColor = System.Drawing.Color.White;
            this.btn_Aplicar_Filtros.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Aplicar_Filtros.ImageOffset = new System.Drawing.Point(8, -2);
            this.btn_Aplicar_Filtros.ImageSize = new System.Drawing.Size(26, 26);
            this.btn_Aplicar_Filtros.Location = new System.Drawing.Point(20, 581);
            this.btn_Aplicar_Filtros.Name = "btn_Aplicar_Filtros";
            this.btn_Aplicar_Filtros.ShadowDecoration.BorderRadius = 14;
            this.btn_Aplicar_Filtros.Size = new System.Drawing.Size(258, 38);
            this.btn_Aplicar_Filtros.TabIndex = 38;
            this.btn_Aplicar_Filtros.Text = "Aplicar filtros";
            this.btn_Aplicar_Filtros.Click += new System.EventHandler(this.btn_Aplicar_Filtros_Click);
            // 
            // cmb_Comprobante
            // 
            this.cmb_Comprobante.BackColor = System.Drawing.Color.Transparent;
            this.cmb_Comprobante.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_Comprobante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Comprobante.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Comprobante.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmb_Comprobante.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.cmb_Comprobante.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmb_Comprobante.ItemHeight = 30;
            this.cmb_Comprobante.Location = new System.Drawing.Point(20, 363);
            this.cmb_Comprobante.Name = "cmb_Comprobante";
            this.cmb_Comprobante.Size = new System.Drawing.Size(258, 36);
            this.cmb_Comprobante.TabIndex = 93;
            this.cmb_Comprobante.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(17, 342);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 18);
            this.label16.TabIndex = 92;
            this.label16.Text = "Comprobante";
            // 
            // btn_Seleccionar_Trabajador
            // 
            this.btn_Seleccionar_Trabajador.Animated = true;
            this.btn_Seleccionar_Trabajador.BorderRadius = 8;
            this.btn_Seleccionar_Trabajador.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Seleccionar_Trabajador.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Seleccionar_Trabajador.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Seleccionar_Trabajador.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Seleccionar_Trabajador.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(82)))), ((int)(((byte)(186)))));
            this.btn_Seleccionar_Trabajador.Font = new System.Drawing.Font("Montserrat", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Seleccionar_Trabajador.ForeColor = System.Drawing.SystemColors.Window;
            this.btn_Seleccionar_Trabajador.Image = global::Union_Formularios.Properties.Resources.icon_search;
            this.btn_Seleccionar_Trabajador.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Seleccionar_Trabajador.ImageOffset = new System.Drawing.Point(-6, -2);
            this.btn_Seleccionar_Trabajador.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Seleccionar_Trabajador.Location = new System.Drawing.Point(245, 292);
            this.btn_Seleccionar_Trabajador.Name = "btn_Seleccionar_Trabajador";
            this.btn_Seleccionar_Trabajador.ShadowDecoration.BorderRadius = 14;
            this.btn_Seleccionar_Trabajador.Size = new System.Drawing.Size(33, 36);
            this.btn_Seleccionar_Trabajador.TabIndex = 90;
            // 
            // txt_select_mecanico
            // 
            this.txt_select_mecanico.Animated = true;
            this.txt_select_mecanico.BackColor = System.Drawing.Color.White;
            this.txt_select_mecanico.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.txt_select_mecanico.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_select_mecanico.DefaultText = "";
            this.txt_select_mecanico.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_select_mecanico.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_select_mecanico.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_select_mecanico.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_select_mecanico.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_select_mecanico.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.txt_select_mecanico.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_select_mecanico.Location = new System.Drawing.Point(20, 292);
            this.txt_select_mecanico.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_select_mecanico.Multiline = true;
            this.txt_select_mecanico.Name = "txt_select_mecanico";
            this.txt_select_mecanico.PasswordChar = '\0';
            this.txt_select_mecanico.PlaceholderText = "\r\n";
            this.txt_select_mecanico.ReadOnly = true;
            this.txt_select_mecanico.SelectedText = "";
            this.txt_select_mecanico.Size = new System.Drawing.Size(218, 36);
            this.txt_select_mecanico.TabIndex = 91;
            // 
            // Cmbox_Select_Placa
            // 
            this.Cmbox_Select_Placa.BackColor = System.Drawing.Color.Transparent;
            this.Cmbox_Select_Placa.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Cmbox_Select_Placa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbox_Select_Placa.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Select_Placa.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Cmbox_Select_Placa.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.Cmbox_Select_Placa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.Cmbox_Select_Placa.ItemHeight = 30;
            this.Cmbox_Select_Placa.Location = new System.Drawing.Point(20, 222);
            this.Cmbox_Select_Placa.Name = "Cmbox_Select_Placa";
            this.Cmbox_Select_Placa.Size = new System.Drawing.Size(258, 36);
            this.Cmbox_Select_Placa.TabIndex = 67;
            this.Cmbox_Select_Placa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(17, 201);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(199, 18);
            this.label13.TabIndex = 66;
            this.label13.Text = "Seleccionar vehículo (por placa)";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(17, 269);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(142, 18);
            this.label14.TabIndex = 89;
            this.label14.Text = "Mecánico responsable";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat SemiBold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 30);
            this.label5.TabIndex = 51;
            this.label5.Text = "Rango de fechas";
            // 
            // dtp_Fecha_Final
            // 
            this.dtp_Fecha_Final.Checked = true;
            this.dtp_Fecha_Final.FillColor = System.Drawing.Color.White;
            this.dtp_Fecha_Final.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtp_Fecha_Final.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Fecha_Final.Location = new System.Drawing.Point(20, 158);
            this.dtp_Fecha_Final.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Fecha_Final.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Fecha_Final.Name = "dtp_Fecha_Final";
            this.dtp_Fecha_Final.Size = new System.Drawing.Size(258, 31);
            this.dtp_Fecha_Final.TabIndex = 50;
            this.dtp_Fecha_Final.Value = new System.DateTime(2025, 8, 21, 23, 30, 26, 774);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 18);
            this.label4.TabIndex = 49;
            this.label4.Text = "Fecha hasta:";
            // 
            // dtp_Fecha_Inicial
            // 
            this.dtp_Fecha_Inicial.Checked = true;
            this.dtp_Fecha_Inicial.FillColor = System.Drawing.Color.White;
            this.dtp_Fecha_Inicial.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtp_Fecha_Inicial.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Fecha_Inicial.Location = new System.Drawing.Point(20, 97);
            this.dtp_Fecha_Inicial.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Fecha_Inicial.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Fecha_Inicial.Name = "dtp_Fecha_Inicial";
            this.dtp_Fecha_Inicial.Size = new System.Drawing.Size(258, 31);
            this.dtp_Fecha_Inicial.TabIndex = 48;
            this.dtp_Fecha_Inicial.Value = new System.DateTime(2025, 8, 21, 23, 30, 26, 774);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 18);
            this.label3.TabIndex = 47;
            this.label3.Text = "Fecha desde:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 38);
            this.label2.TabIndex = 47;
            this.label2.Text = "Filtros";
            // 
            // guna2ShadowPanel2
            // 
            this.guna2ShadowPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel2.Controls.Add(this.dgvReportes);
            this.guna2ShadowPanel2.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel2.Location = new System.Drawing.Point(318, 235);
            this.guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            this.guna2ShadowPanel2.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel2.Size = new System.Drawing.Size(882, 516);
            this.guna2ShadowPanel2.TabIndex = 46;
            // 
            // dgvReportes
            // 
            this.dgvReportes.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvReportes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReportes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvReportes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvReportes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReportes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReportes.ColumnHeadersHeight = 4;
            this.dgvReportes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReportes.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReportes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvReportes.Location = new System.Drawing.Point(3, 3);
            this.dgvReportes.Name = "dgvReportes";
            this.dgvReportes.RowHeadersVisible = false;
            this.dgvReportes.Size = new System.Drawing.Size(876, 510);
            this.dgvReportes.TabIndex = 71;
            this.dgvReportes.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvReportes.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvReportes.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvReportes.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvReportes.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvReportes.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvReportes.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvReportes.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvReportes.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvReportes.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvReportes.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvReportes.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvReportes.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvReportes.ThemeStyle.ReadOnly = false;
            this.dgvReportes.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvReportes.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvReportes.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvReportes.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvReportes.ThemeStyle.RowsStyle.Height = 22;
            this.dgvReportes.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvReportes.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.BorderRadius = 10;
            this.guna2GradientPanel1.Controls.Add(this.label10);
            this.guna2GradientPanel1.Controls.Add(this.lbl_Cont_Trab);
            this.guna2GradientPanel1.Controls.Add(this.lbl_Ordenes);
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(134)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(230)))), ((int)(((byte)(213)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(32, 22);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.ShadowDecoration.BorderRadius = 14;
            this.guna2GradientPanel1.ShadowDecoration.Color = System.Drawing.Color.Gray;
            this.guna2GradientPanel1.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.guna2GradientPanel1.Size = new System.Drawing.Size(255, 120);
            this.guna2GradientPanel1.TabIndex = 62;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(19, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 18);
            this.label10.TabIndex = 52;
            this.label10.Text = "Cantidad en el periodo";
            // 
            // lbl_Cont_Trab
            // 
            this.lbl_Cont_Trab.AutoSize = true;
            this.lbl_Cont_Trab.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cont_Trab.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Cont_Trab.ForeColor = System.Drawing.Color.White;
            this.lbl_Cont_Trab.Location = new System.Drawing.Point(16, 9);
            this.lbl_Cont_Trab.Name = "lbl_Cont_Trab";
            this.lbl_Cont_Trab.Size = new System.Drawing.Size(109, 33);
            this.lbl_Cont_Trab.TabIndex = 8;
            this.lbl_Cont_Trab.Text = "Órdenes";
            // 
            // lbl_Ordenes
            // 
            this.lbl_Ordenes.AutoSize = true;
            this.lbl_Ordenes.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Ordenes.Font = new System.Drawing.Font("Montserrat", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Ordenes.ForeColor = System.Drawing.Color.White;
            this.lbl_Ordenes.Location = new System.Drawing.Point(36, 42);
            this.lbl_Ordenes.Name = "lbl_Ordenes";
            this.lbl_Ordenes.Size = new System.Drawing.Size(44, 49);
            this.lbl_Ordenes.TabIndex = 7;
            this.lbl_Ordenes.Text = "0";
            // 
            // guna2ShadowPanel3
            // 
            this.guna2ShadowPanel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel3.Controls.Add(this.guna2GradientPanel3);
            this.guna2ShadowPanel3.Controls.Add(this.guna2GradientPanel2);
            this.guna2ShadowPanel3.Controls.Add(this.guna2GradientPanel1);
            this.guna2ShadowPanel3.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel3.Location = new System.Drawing.Point(318, 61);
            this.guna2ShadowPanel3.Name = "guna2ShadowPanel3";
            this.guna2ShadowPanel3.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel3.Size = new System.Drawing.Size(882, 168);
            this.guna2ShadowPanel3.TabIndex = 65;
            // 
            // guna2GradientPanel3
            // 
            this.guna2GradientPanel3.BorderRadius = 10;
            this.guna2GradientPanel3.Controls.Add(this.label12);
            this.guna2GradientPanel3.Controls.Add(this.label8);
            this.guna2GradientPanel3.Controls.Add(this.Nom_Serv_moda);
            this.guna2GradientPanel3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(134)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel3.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(230)))), ((int)(((byte)(213)))));
            this.guna2GradientPanel3.Location = new System.Drawing.Point(598, 22);
            this.guna2GradientPanel3.Name = "guna2GradientPanel3";
            this.guna2GradientPanel3.ShadowDecoration.BorderRadius = 14;
            this.guna2GradientPanel3.ShadowDecoration.Color = System.Drawing.Color.Gray;
            this.guna2GradientPanel3.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.guna2GradientPanel3.Size = new System.Drawing.Size(255, 120);
            this.guna2GradientPanel3.TabIndex = 64;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(17, 90);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 18);
            this.label12.TabIndex = 53;
            this.label12.Text = "Más frecuente";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(14, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 33);
            this.label8.TabIndex = 8;
            this.label8.Text = "Servicio";
            // 
            // Nom_Serv_moda
            // 
            this.Nom_Serv_moda.AutoSize = true;
            this.Nom_Serv_moda.BackColor = System.Drawing.Color.Transparent;
            this.Nom_Serv_moda.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nom_Serv_moda.ForeColor = System.Drawing.Color.White;
            this.Nom_Serv_moda.Location = new System.Drawing.Point(14, 46);
            this.Nom_Serv_moda.Name = "Nom_Serv_moda";
            this.Nom_Serv_moda.Size = new System.Drawing.Size(126, 33);
            this.Nom_Serv_moda.TabIndex = 7;
            this.Nom_Serv_moda.Text = "Nom_Serv";
            // 
            // guna2GradientPanel2
            // 
            this.guna2GradientPanel2.BorderRadius = 10;
            this.guna2GradientPanel2.Controls.Add(this.label11);
            this.guna2GradientPanel2.Controls.Add(this.label6);
            this.guna2GradientPanel2.Controls.Add(this.lbl_Ingresos_totales);
            this.guna2GradientPanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(134)))), ((int)(((byte)(245)))));
            this.guna2GradientPanel2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(230)))), ((int)(((byte)(213)))));
            this.guna2GradientPanel2.Location = new System.Drawing.Point(315, 22);
            this.guna2GradientPanel2.Name = "guna2GradientPanel2";
            this.guna2GradientPanel2.ShadowDecoration.BorderRadius = 14;
            this.guna2GradientPanel2.ShadowDecoration.Color = System.Drawing.Color.Gray;
            this.guna2GradientPanel2.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.guna2GradientPanel2.Size = new System.Drawing.Size(255, 120);
            this.guna2GradientPanel2.TabIndex = 63;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(19, 90);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 18);
            this.label11.TabIndex = 53;
            this.label11.Text = "Total facturado";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(16, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 33);
            this.label6.TabIndex = 8;
            this.label6.Text = "Ingresos";
            // 
            // lbl_Ingresos_totales
            // 
            this.lbl_Ingresos_totales.AutoSize = true;
            this.lbl_Ingresos_totales.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Ingresos_totales.Font = new System.Drawing.Font("Montserrat", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Ingresos_totales.ForeColor = System.Drawing.Color.White;
            this.lbl_Ingresos_totales.Location = new System.Drawing.Point(13, 42);
            this.lbl_Ingresos_totales.Name = "lbl_Ingresos_totales";
            this.lbl_Ingresos_totales.Size = new System.Drawing.Size(64, 49);
            this.lbl_Ingresos_totales.TabIndex = 7;
            this.lbl_Ingresos_totales.Text = "$0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Montserrat SemiBold", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(226, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(249, 18);
            this.label15.TabIndex = 69;
            this.label15.Text = "Histórico técnico, operativo y financiero";
            // 
            // btn_Export_xlsx
            // 
            this.btn_Export_xlsx.Animated = true;
            this.btn_Export_xlsx.BorderRadius = 8;
            this.btn_Export_xlsx.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Export_xlsx.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Export_xlsx.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Export_xlsx.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Export_xlsx.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(202)))), ((int)(((byte)(130)))));
            this.btn_Export_xlsx.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Export_xlsx.ForeColor = System.Drawing.Color.White;
            this.btn_Export_xlsx.Image = global::Union_Formularios.Properties.Resources.icon_excel;
            this.btn_Export_xlsx.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Export_xlsx.ImageOffset = new System.Drawing.Point(8, -1);
            this.btn_Export_xlsx.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Export_xlsx.Location = new System.Drawing.Point(936, 14);
            this.btn_Export_xlsx.Name = "btn_Export_xlsx";
            this.btn_Export_xlsx.ShadowDecoration.BorderRadius = 14;
            this.btn_Export_xlsx.Size = new System.Drawing.Size(264, 42);
            this.btn_Export_xlsx.TabIndex = 61;
            this.btn_Export_xlsx.Text = "Exportar a Excel";
            this.btn_Export_xlsx.Click += new System.EventHandler(this.btn_Export_xlsx_Click);
            // 
            // Formulario_Reportes
            // 
            this.ClientSize = new System.Drawing.Size(1212, 793);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.guna2ShadowPanel3);
            this.Controls.Add(this.btn_Export_xlsx);
            this.Controls.Add(this.guna2ShadowPanel2);
            this.Controls.Add(this.guna2ShadowPanel1);
            this.Controls.Add(this.label1);
            this.Name = "Formulario_Reportes";
            this.Text = "Reportes";
            this.Load += new System.EventHandler(this.Formulario_Reportes_Load);
            this.guna2ShadowPanel1.ResumeLayout(false);
            this.guna2ShadowPanel1.PerformLayout();
            this.guna2ShadowPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportes)).EndInit();
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2GradientPanel1.PerformLayout();
            this.guna2ShadowPanel3.ResumeLayout(false);
            this.guna2GradientPanel3.ResumeLayout(false);
            this.guna2GradientPanel3.PerformLayout();
            this.guna2GradientPanel2.ResumeLayout(false);
            this.guna2GradientPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Formulario_Reportes_Load(object sender, EventArgs e)
        {

        }
    }
}
