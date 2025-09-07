using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
        public partial class Formulario_Facturas : Form
        {
        private string connectionString = @"Server=localhost;Database=CAR_EFULL;Trusted_Connection=True;";

        public Formulario_Facturas()
        {
            InitializeComponent();

            dgvFacturas.CellDoubleClick += DgvFacturas_CellDoubleClick;
            dgvFacturas.SelectionChanged += DgvFacturas_SelectionChanged;
            this.Load += FormularioFacturacion_Load;
        }
        private void FormularioFacturacion_Load(object sender, EventArgs e)
        {
            CargarFacturas();
        }

        private void CargarFacturas()
        {
            try
            {
                string query = @"
                SELECT 
                    F.CodigoFactura,
                    P.Nombre + ' ' + P.Apellido AS Propietario,
                    V.Placa AS Vehiculo,
                    F.TipoServicio AS Servicio,
                    F.Fecha,
                    F.Total
                FROM Facturas F
                INNER JOIN Propietarios P ON F.ID_Propietario = P.ID_Propietario
                INNER JOIN Vehiculos V ON F.VehicleID = V.VehicleID
                ORDER BY F.Fecha DESC";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvFacturas.DataSource = dt;

                    // Ajustes visuales
                    dgvFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar facturas: " + ex.Message);
            }
        }

        private void DgvFacturas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Aquí asumimos que tus facturas digitales están guardadas como "CodigoFactura.jpg" en C:\Facturas\
                string codigoFactura = dgvFacturas.Rows[e.RowIndex].Cells["CodigoFactura"].Value.ToString();
                string rutaFactura = $@"C:\Facturas\{codigoFactura}.jpg"; // Ajusta según tu ruta

                if (File.Exists(rutaFactura))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(rutaFactura);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo abrir la factura: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("La factura digitalizada no se encuentra disponible.");
                }
            }
        }
        private void DgvFacturas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFacturas.SelectedRows.Count > 0)
            {
                string codigoFactura = dgvFacturas.SelectedRows[0].Cells["CodigoFactura"].Value.ToString();
                string rutaFactura = $@"C:\Facturas\{codigoFactura}.jpg"; // Ajusta según tu ruta

                if (File.Exists(rutaFactura))
                {
                    try
                    {
                        using (var tempImage = Image.FromFile(rutaFactura))
                        {
                            pictureBoxFactura.Image = new Bitmap(tempImage);
                        }
                        pictureBoxFactura.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch
                    {
                        pictureBoxFactura.Image = null;
                    }
                }
                else
                {
                    pictureBoxFactura.Image = null;
                }
            }
        }
        private Guna.UI2.WinForms.Guna2DataGridView dgvFacturas;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private FontAwesome.Sharp.IconPictureBox pictureBoxFactura;
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel2;
        private Label label2;
        private Label label1;

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvFacturas = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.pictureBoxFactura = new FontAwesome.Sharp.IconPictureBox();
            this.guna2ShadowPanel2 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFactura)).BeginInit();
            this.guna2ShadowPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.AllowUserToAddRows = false;
            this.dgvFacturas.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvFacturas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
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
            // guna2ShadowPanel1
            // 
            this.guna2ShadowPanel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ShadowPanel1.Controls.Add(this.pictureBoxFactura);
            this.guna2ShadowPanel1.FillColor = System.Drawing.Color.White;
            this.guna2ShadowPanel1.Location = new System.Drawing.Point(698, 84);
            this.guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            this.guna2ShadowPanel1.ShadowColor = System.Drawing.Color.Black;
            this.guna2ShadowPanel1.Size = new System.Drawing.Size(490, 642);
            this.guna2ShadowPanel1.TabIndex = 48;
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
            this.Controls.Add(this.guna2ShadowPanel1);
            this.Controls.Add(this.label1);
            this.Name = "Formulario_Facturas";
            this.Text = "Facturación";
            this.Load += new System.EventHandler(this.Formulario_Facturas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.guna2ShadowPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFactura)).EndInit();
            this.guna2ShadowPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Formulario_Facturas_Load(object sender, EventArgs e)
        {

        }
    }
}
