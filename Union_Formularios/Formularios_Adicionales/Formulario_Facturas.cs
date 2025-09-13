using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
        public partial class Formulario_Facturas : Form
        {
        public Formulario_Facturas()
        {
            InitializeComponent();
            dgvFacturas.CellDoubleClick += DgvFacturas_CellDoubleClick;
            this.Load += Formulario_Facturas_Load;
        }
        private void Formulario_Facturas_Load(object sender, EventArgs e)
        {
            var asm = typeof(Formulario_Facturas).Assembly;
            var rdlcName = asm.GetManifestResourceNames()
                              .FirstOrDefault(n => n.EndsWith(".Factura.rdlc"));
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
        private void FormularioFacturacion_Load(object sender, EventArgs e)
        {
            CargarFacturas();
        }

        private void CargarFacturas()
        {
            const string sql = @"
            SELECT 
                F.FacturaID,            
                F.CodigoFactura,
                P.Nombre + ' ' + P.Apellido AS Propietario,
                V.Placa AS Vehiculo,
                F.TipoServicio AS Servicio,
                F.Fecha,
                F.Total
            FROM Facturas F
            INNER JOIN Propietarios P ON F.ID_Propietario = P.ID_Propietario
            INNER JOIN Vehiculos V    ON F.VehicleID     = V.VehicleID
            ORDER BY F.Fecha DESC;";

            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(sql, cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                dgvFacturas.DataSource = dt;
                if (dgvFacturas.Columns.Contains("FacturaID"))
                    dgvFacturas.Columns["FacturaID"].Visible = false;
            }
        }

        private void DgvFacturas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int facturaID = Convert.ToInt32(dgvFacturas.Rows[e.RowIndex].Cells["FacturaID"].Value);
            CargarFacturaEnReportViewer(facturaID);   
        }


        private void CargarFacturaEnReportViewer(int facturaID)
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    // Detectar columnas de Vehiculos (marca/modelo/año) —lo de tu código está bien.
                    string colMarca = null, colModelo = null, colAnio = null;
                    using (var cmdCols = new SqlCommand(
                        "SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vehiculos');", cn))
                    using (var rd = cmdCols.ExecuteReader())
                    {
                        var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        while (rd.Read()) cols.Add(rd.GetString(0));

                        string[] candsMarca = { "ID_Marca", "MarcaID", "ID_MarcaVehiculo", "IDMarca", "IdMarca" };
                        string[] candsModelo = { "ID_Modelo", "ModeloID", "ID_ModeloVehiculo", "IDModelo", "IdModelo" };
                        string[] candsAnio = { "ID_Anio", "AnioID", "ID_ModeloAnio", "IDAnio", "IdAnio", "ID_Año", "AnoID" };

                        colMarca = candsMarca.FirstOrDefault(cols.Contains);
                        colModelo = candsModelo.FirstOrDefault(cols.Contains);
                        colAnio = candsAnio.FirstOrDefault(cols.Contains);
                    }

                    string sqlCab = (colMarca != null && colModelo != null && colAnio != null)
                    ? $@"
                SELECT 
                    F.FacturaID,
                    F.CodigoFactura,
                    F.Fecha,
                    F.TipoServicio,
                    (P.Nombre + ' ' + P.Apellido) AS Propietario,
                    P.Cedula, 
                    P.Telefono, 
                    P.Correo AS Email,      -- << alias clave
                    V.Placa,
                    MV.Nombre AS Marca,
                    MO.Nombre AS Modelo,
                    AN.Anio,
                    V.Color,
                    F.Subtotal,
                    F.IVA,
                    F.Total
                FROM Facturas F
                INNER JOIN Propietarios P ON F.ID_Propietario = P.ID_Propietario
                INNER JOIN Vehiculos V    ON F.VehicleID     = V.VehicleID
                LEFT JOIN MarcaVehiculo  MV ON MV.ID_Marca  = V.{colMarca}
                LEFT JOIN ModeloVehiculo MO ON MO.ID_Modelo = V.{colModelo}
                LEFT JOIN ModeloAnio     AN ON AN.ID_Anio   = V.{colAnio}
                WHERE F.FacturaID = @FacturaID;"
                    : @"
                SELECT 
                    F.FacturaID,
                    F.CodigoFactura,
                    F.Fecha,
                    F.TipoServicio,
                    (P.Nombre + ' ' + P.Apellido) AS Propietario,
                    P.Cedula, 
                    P.Telefono, 
                    P.Correo AS Email,      -- << alias clave
                    V.Placa,
                    NULL AS Marca,
                    NULL AS Modelo,
                    NULL AS Anio,
                    V.Color,
                    F.Subtotal,
                    F.IVA,
                    F.Total
                FROM Facturas F
                INNER JOIN Propietarios P ON F.ID_Propietario = P.ID_Propietario
                INNER JOIN Vehiculos V    ON F.VehicleID     = V.VehicleID
                WHERE F.FacturaID = @FacturaID;";

                    var dtCab = new DataTable("FacturaDT");
                    using (var cmdCab = new SqlCommand(sqlCab, cn))
                    {
                        cmdCab.Parameters.Add("@FacturaID", SqlDbType.Int).Value = facturaID;
                        using (var daCab = new SqlDataAdapter(cmdCab))
                            daCab.Fill(dtCab);
                    }

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
                FROM FacturaDetalle FD
                LEFT JOIN Repuestos R ON R.RepuestoID = FD.RepuestoID
                WHERE FD.FacturaID = @FacturaID
                ORDER BY FD.FacturaDetalleID;";

                    var dtDet = new DataTable("FacturaDetalleDT");
                    using (var cmdDet = new SqlCommand(sqlDet, cn))
                    {
                        cmdDet.Parameters.Add("@FacturaID", SqlDbType.Int).Value = facturaID;
                        using (var daDet = new SqlDataAdapter(cmdDet))
                            daDet.Fill(dtDet);
                    }

                    // Enlazar datasets (nombres EXACTOS como en el RDLC)
                    Facturas_Preview.LocalReport.DataSources.Clear();
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
        private Guna.UI2.WinForms.Guna2ShadowPanel panelReporte;
        private FontAwesome.Sharp.IconPictureBox pictureBoxFactura;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvFacturas = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.panelReporte = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.Facturas_Preview = new Microsoft.Reporting.WinForms.ReportViewer();
            this.pictureBoxFactura = new FontAwesome.Sharp.IconPictureBox();
            this.guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.panelReporte.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFactura)).BeginInit();
            this.guna2ShadowPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.AllowUserToAddRows = false;
            this.dgvFacturas.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFacturas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvFacturas.ColumnHeadersHeight = 15;
            this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFacturas.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvFacturas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFacturas.Location = new System.Drawing.Point(15, 12);
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.ReadOnly = true;
            this.dgvFacturas.RowHeadersVisible = false;
            this.dgvFacturas.Size = new System.Drawing.Size(639, 615);
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
            this.label1.Location = new System.Drawing.Point(18, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 38);
            this.label1.TabIndex = 44;
            this.label1.Text = "Facturas generadas";
            // 
            // panelReporte
            // 
            this.panelReporte.BackColor = System.Drawing.Color.Transparent;
            this.panelReporte.Controls.Add(this.Facturas_Preview);
            this.panelReporte.Controls.Add(this.pictureBoxFactura);
            this.panelReporte.FillColor = System.Drawing.Color.White;
            this.panelReporte.Location = new System.Drawing.Point(698, 84);
            this.panelReporte.Name = "panelReporte";
            this.panelReporte.ShadowColor = System.Drawing.Color.Black;
            this.panelReporte.Size = new System.Drawing.Size(490, 642);
            this.panelReporte.TabIndex = 48;
            // 
            // Facturas_Preview
            // 
            this.Facturas_Preview.Location = new System.Drawing.Point(7, 6);
            this.Facturas_Preview.Name = "Facturas_Preview";
            this.Facturas_Preview.ServerReport.BearerToken = null;
            this.Facturas_Preview.Size = new System.Drawing.Size(478, 628);
            this.Facturas_Preview.TabIndex = 1;
            // 
            // pictureBoxFactura
            // 
            this.pictureBoxFactura.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxFactura.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxFactura.IconChar = FontAwesome.Sharp.IconChar.None;
            this.pictureBoxFactura.IconColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxFactura.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.pictureBoxFactura.IconSize = 463;
            this.pictureBoxFactura.Location = new System.Drawing.Point(14, 12);
            this.pictureBoxFactura.Name = "pictureBoxFactura";
            this.pictureBoxFactura.Size = new System.Drawing.Size(463, 615);
            this.pictureBoxFactura.TabIndex = 0;
            this.pictureBoxFactura.TabStop = false;
            // 
            // guna2ShadowPanel2
            // 
            this.guna2ShadowPanel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel2.Controls.Add(this.dgvFacturas);
            this.guna2ShadowPanel2.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel2.Location = new System.Drawing.Point(25, 84);
            this.guna2ShadowPanel2.Name = "guna2ShadowPanel2";
            this.guna2ShadowPanel2.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel2.Size = new System.Drawing.Size(667, 642);
            this.guna2ShadowPanel2.TabIndex = 49;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(705, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 38);
            this.label2.TabIndex = 50;
            this.label2.Text = "Factura:";
            // 
            // Formulario_Facturas
            // 
            this.ClientSize = new System.Drawing.Size(1212, 753);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.guna2ShadowPanel2);
            this.Controls.Add(this.panelReporte);
            this.Controls.Add(this.label1);
            this.Name = "Formulario_Facturas";
            this.Text = "Facturación";
            this.Load += new System.EventHandler(this.Formulario_Facturas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.panelReporte.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFactura)).EndInit();
            this.guna2ShadowPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}