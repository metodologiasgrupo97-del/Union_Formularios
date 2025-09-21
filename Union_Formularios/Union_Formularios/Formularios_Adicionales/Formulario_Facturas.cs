using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
        public partial class Formulario_Facturas : Form
        {
        public Formulario_Facturas()
        {
            InitializeComponent();
            this.Load += Formulario_Facturas_Load;
            dgvFacturas.CellDoubleClick += DgvFacturas_CellDoubleClick;
        }

        private void Formulario_Facturas_Load(object sender, EventArgs e)
        {
            var asm = typeof(Formulario_Facturas).Assembly;
            var rdlcName = asm.GetManifestResourceNames()
                              .FirstOrDefault(n => n.EndsWith(".Factura.rdlc", StringComparison.OrdinalIgnoreCase));
            if (rdlcName == null)
            {
                MessageBox.Show("No se encontró el recurso embebido 'Factura.rdlc'. Verifica Build Action = Embedded Resource.",
                                "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Facturas_Preview.LocalReport.ReportEmbeddedResource = rdlcName;
            Facturas_Preview.LocalReport.DataSources.Clear();
            Facturas_Preview.RefreshReport();

            CargarFacturas();
        }

        // =================== Listado ===================
        private void CargarFacturas()
        {
            using (var cn = Conexion_SQL.OpenConnection())
            {
                string sql = @"
                SELECT 
                    F.FacturaID,            
                    F.CodigoFactura,
                    (P.Nombre + ' ' + ISNULL(P.Apellido,'')) AS Propietario,
                    V.Placa AS Vehiculo,
                    TS.Nombre AS Servicio,
                    F.Fecha,
                    F.Total
                FROM dbo.Facturas F
                INNER JOIN dbo.Propietarios P ON F.ID_Propietario = P.ID_Propietario
                INNER JOIN dbo.Vehiculos    V ON F.VehicleID     = V.VehicleID
                LEFT  JOIN dbo.TipoServicio TS ON TS.TipoServicioID = F.TipoServicioID
                ORDER BY F.Fecha DESC;";

                using (var da = new SqlDataAdapter(sql, cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvFacturas.DataSource = dt;
                    if (dgvFacturas.Columns.Contains("FacturaID"))
                        dgvFacturas.Columns["FacturaID"].Visible = false;
                }
            }
        }

        private void DgvFacturas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var val = dgvFacturas.Rows[e.RowIndex].Cells["FacturaID"]?.Value;
            if (val == null || val == DBNull.Value) return;
            int facturaID = Convert.ToInt32(val);
            CargarFacturaEnReportViewer(facturaID);
        }

        // =============== helpers de detección de esquema ===============
        private static bool HasColumn(SqlConnection cn, string fullTableName, string column)
        {
            using (var cmd = new SqlCommand(@"
                SELECT 1
                FROM sys.columns c
                WHERE c.[object_id] = OBJECT_ID(@tbl) AND c.name = @col;", cn))
            {
                cmd.Parameters.AddWithValue("@tbl", fullTableName);
                cmd.Parameters.AddWithValue("@col", column);
                return cmd.ExecuteScalar() != null;
            }
        }

        // Arma SELECT de cabecera adaptable al esquema de Vehiculos
        private static string BuildSqlCabecera(SqlConnection cn)
        {
            // Vehículos: detectar esquema
            bool vHasMarcaID = HasColumn(cn, "dbo.Vehiculos", "MarcaID");
            bool vHasModeloID = HasColumn(cn, "dbo.Vehiculos", "ModeloID");
            bool vHasModeloAnioID = HasColumn(cn, "dbo.Vehiculos", "ModeloAnioID");
            bool vHasMarcaTxt = HasColumn(cn, "dbo.Vehiculos", "Marca");
            bool vHasModeloTxt = HasColumn(cn, "dbo.Vehiculos", "Modelo");
            bool vHasAnioTxt = HasColumn(cn, "dbo.Vehiculos", "Anio");

            string marcaExpr, modeloExpr, anioExpr;

            string joins = @"
    INNER JOIN dbo.Propietarios   P  ON P.ID_Propietario = F.ID_Propietario
    INNER JOIN dbo.Vehiculos      V  ON V.VehicleID      = F.VehicleID
    LEFT  JOIN dbo.Users          U  ON U.UserID         = F.UserID
    LEFT  JOIN dbo.FacturaDetalle FD ON FD.FacturaID     = F.FacturaID   -- <<<<<< LEFT JOIN
    LEFT  JOIN dbo.Repuestos      R  ON R.RepuestoID     = FD.RepuestoID
    LEFT  JOIN dbo.Impuestos      I  ON I.ImpuestoID     = R.ImpuestoID_Default
    LEFT  JOIN dbo.TipoServicio   TS ON TS.TipoServicioID = F.TipoServicioID";

            if (vHasMarcaID && vHasModeloID && vHasModeloAnioID)
            {
                joins += @"
        LEFT  JOIN dbo.ModeloAnio     MaA ON MaA.ModeloAnioID = V.ModeloAnioID
        LEFT  JOIN dbo.ModeloVehiculo MO  ON MO.ModeloID      = V.ModeloID
        LEFT  JOIN dbo.MarcaVehiculo  MV  ON MV.MarcaID       = V.MarcaID";
                marcaExpr = "MV.Nombre";
                modeloExpr = "MO.Nombre";
                anioExpr = "MaA.Anio";
            }
            else if (vHasModeloAnioID)
            {
                joins += @"
        LEFT  JOIN dbo.ModeloAnio     MaA ON MaA.ModeloAnioID = V.ModeloAnioID
        LEFT  JOIN dbo.ModeloVehiculo MO  ON MO.ModeloID      = MaA.ModeloID
        LEFT  JOIN dbo.MarcaVehiculo  MV  ON MV.MarcaID       = MO.MarcaID";
                marcaExpr = "MV.Nombre";
                modeloExpr = "MO.Nombre";
                anioExpr = "MaA.Anio";
            }
            else if (vHasModeloID)
            {
                joins += @"
        LEFT  JOIN dbo.ModeloVehiculo MO  ON MO.ModeloID      = V.ModeloID
        LEFT  JOIN dbo.MarcaVehiculo  MV  ON MV.MarcaID       = MO.MarcaID";
                marcaExpr = "MV.Nombre";
                modeloExpr = "MO.Nombre";
                anioExpr = vHasAnioTxt ? "V.Anio" : "NULL";
            }
            else
            {
                marcaExpr = vHasMarcaTxt ? "V.Marca" : "NULL";
                modeloExpr = vHasModeloTxt ? "V.Modelo" : "NULL";
                anioExpr = vHasAnioTxt ? "V.Anio" : "NULL";
            }

            string groupExtras = "";
            if (marcaExpr != "NULL") groupExtras += ", " + marcaExpr;
            if (modeloExpr != "NULL") groupExtras += ", " + modeloExpr;
            if (anioExpr != "NULL") groupExtras += ", " + anioExpr;

            // SELECT de cabecera -> alias compatibles con tu RDLC (FacturaDT)
            string sql = $@"
    SELECT 
        F.FacturaID,
        F.CodigoFactura,
        CONVERT(date, F.Fecha) AS Fecha,
        TS.Nombre AS TipoServicio,
        (ISNULL(U.FirstName,'') + CASE WHEN U.FirstName IS NULL OR U.LastName IS NULL THEN '' ELSE ' ' END + ISNULL(U.LastName,'')) AS Mecanico,
        F.Observaciones,

        (P.Nombre + ' ' + ISNULL(P.Apellido,'')) AS Propietario,
        P.Cedula,
        P.Telefono,
        P.Correo,

        V.Placa,
        {marcaExpr}  AS Marca,
        {modeloExpr} AS Modelo,
        {anioExpr}   AS Anio,
        V.Color,

        -- Totales de cabecera ya guardados
        F.Subtotal,
        F.IVA,
        F.Total,

        -- Desgloses por impuesto (con LEFT JOIN, usar COALESCE)
        COALESCE(SUM(CASE 
            WHEN ISNULL(FD.IVA,0) > 0 OR ISNULL(I.TasaDecimal,0) > 0
            THEN FD.Cantidad * FD.PrecioUnitario ELSE 0 END),0) AS SubtotalBaseIVA,

        COALESCE(SUM(CASE 
            WHEN ISNULL(FD.IVA,0) = 0 AND I.Codigo LIKE 'IVA%'
            THEN FD.Cantidad * FD.PrecioUnitario ELSE 0 END),0) AS Subtotal0,

        COALESCE(SUM(CASE 
            WHEN ISNULL(FD.IVA,0) = 0 AND I.Codigo IN ('NO_OBJ','NOOBJ','NO_OBJETO','NOOBJETO','EXE','EXENTO')
            THEN FD.Cantidad * FD.PrecioUnitario ELSE 0 END),0) AS SubtotalNoObjeto,

        COALESCE(SUM(CASE 
            WHEN ISNULL(FD.IVA,0) = 0 AND I.ImpuestoID IS NULL
            THEN FD.Cantidad * FD.PrecioUnitario ELSE 0 END),0) AS SubtotalSinImpuestos

    FROM dbo.Facturas F
    {joins}
    WHERE F.FacturaID = @FacturaID
    GROUP BY
        F.FacturaID, F.CodigoFactura, CONVERT(date, F.Fecha),
        TS.Nombre, F.Observaciones,
        (ISNULL(U.FirstName,'') + CASE WHEN U.FirstName IS NULL OR U.LastName IS NULL THEN '' ELSE ' ' END + ISNULL(U.LastName,'')),
        (P.Nombre + ' ' + ISNULL(P.Apellido,'')), P.Cedula, P.Telefono, P.Correo,
        V.Placa, V.Color,
        F.Subtotal, F.IVA, F.Total{groupExtras};";
            return sql;
        }

        // =================== Cargar en ReportViewer ===================
        private void CargarFacturaEnReportViewer(int facturaID)
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    // EmpresaDT
                    var dtEmpresa = new DataTable("EmpresaDT");
                    const string sqlEmpresa = @"
                        SELECT TOP 1
                            EmpresaID, RazonSocial, NombreComercial, RUC, Direccion, Telefono, Correo,
                            Logo, COALESCE(LogoMimeType,'image/png') AS LogoMimeType
                        FROM dbo.Empresa
                        ORDER BY EmpresaID;";
                    using (var da = new SqlDataAdapter(sqlEmpresa, cn))
                        da.Fill(dtEmpresa);

                    // FacturaDT (cabecera)
                    var dtCab = new DataTable("FacturaDT");
                    string sqlCab = BuildSqlCabecera(cn);
                    using (var cmd = new SqlCommand(sqlCab, cn))
                    {
                        cmd.Parameters.Add("@FacturaID", SqlDbType.Int).Value = facturaID;
                        using (var da = new SqlDataAdapter(cmd))
                            da.Fill(dtCab);
                    }

                    // FacturaDetalleDT (detalle)
                    var dtDet = new DataTable("FacturaDetalleDT");
                    const string sqlDet = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY FD.FacturaDetalleID) AS Item,
                            FD.RepuestoID,
                            COALESCE(FD.Descripcion, R.Nombre) AS Descripcion,
                            FD.ClaveUnidad,
                            FD.Cantidad,
                            FD.PrecioUnitario,
                            (FD.Cantidad * FD.PrecioUnitario) AS Subtotal,
                            ISNULL(FD.IVA, 0) AS IVA,
                            (FD.Cantidad * FD.PrecioUnitario) + ISNULL(FD.IVA,0) AS TotalLinea
                        FROM dbo.FacturaDetalle FD
                        LEFT JOIN dbo.Repuestos R ON R.RepuestoID = FD.RepuestoID
                        WHERE FD.FacturaID = @FacturaID
                        ORDER BY FD.FacturaDetalleID;";
                    using (var cmd = new SqlCommand(sqlDet, cn))
                    {
                        cmd.Parameters.Add("@FacturaID", SqlDbType.Int).Value = facturaID;
                        using (var da = new SqlDataAdapter(cmd))
                            da.Fill(dtDet);
                    }

                    // Bind a RDLC
                    Facturas_Preview.LocalReport.DataSources.Clear();
                    Facturas_Preview.LocalReport.DataSources.Add(new ReportDataSource("dsEmpresa", dtEmpresa));
                    Facturas_Preview.LocalReport.DataSources.Add(new ReportDataSource("dsFactura", dtCab));
                    Facturas_Preview.LocalReport.DataSources.Add(new ReportDataSource("dsFacturaDetalle", dtDet));
                    Facturas_Preview.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la factura: " + ex.Message, "Reporte",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Guna.UI2.WinForms.Guna2DataGridView dgvFacturas;
        private Label label2;
        private Microsoft.Reporting.WinForms.ReportViewer Facturas_Preview;
        private Label label1;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvFacturas = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.Facturas_Preview = new Microsoft.Reporting.WinForms.ReportViewer();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.AllowUserToAddRows = false;
            this.dgvFacturas.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFacturas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFacturas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFacturas.ColumnHeadersHeight = 15;
            this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFacturas.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFacturas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFacturas.Location = new System.Drawing.Point(20, 76);
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.ReadOnly = true;
            this.dgvFacturas.RowHeadersVisible = false;
            this.dgvFacturas.Size = new System.Drawing.Size(609, 659);
            this.dgvFacturas.TabIndex = 47;
            this.dgvFacturas.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvFacturas.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvFacturas.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvFacturas.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvFacturas.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFacturas.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvFacturas.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFacturas.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvFacturas.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvFacturas.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvFacturas.ThemeStyle.HeaderStyle.Height = 15;
            this.dgvFacturas.ThemeStyle.ReadOnly = true;
            this.dgvFacturas.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFacturas.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvFacturas.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvFacturas.ThemeStyle.RowsStyle.Height = 22;
            this.dgvFacturas.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFacturas.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 38);
            this.label1.TabIndex = 44;
            this.label1.Text = "Facturas generadas";
            // 
            // Facturas_Preview
            // 
            this.Facturas_Preview.Location = new System.Drawing.Point(640, 76);
            this.Facturas_Preview.Name = "Facturas_Preview";
            this.Facturas_Preview.ServerReport.BearerToken = null;
            this.Facturas_Preview.Size = new System.Drawing.Size(556, 659);
            this.Facturas_Preview.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(633, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 38);
            this.label2.TabIndex = 50;
            this.label2.Text = "Factura";
            // 
            // Formulario_Facturas
            // 
            this.ClientSize = new System.Drawing.Size(1249, 778);
            this.Controls.Add(this.dgvFacturas);
            this.Controls.Add(this.Facturas_Preview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Formulario_Facturas";
            this.Text = "Facturación";
            this.Load += new System.EventHandler(this.Formulario_Facturas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}