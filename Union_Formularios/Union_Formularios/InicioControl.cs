using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Union_Formularios
{
    public partial class InicioControl : UserControl
    {
        public InicioControl()
        {
            InitializeComponent();
            this.Load += InicioControl_Load;
            CargarTotales();
            CargarVehiculos();
        }

        private void InicioControl_Load(object sender, EventArgs e)
        {
            ConfigurarRelojCentrado();
        }

        private void ConfigurarRelojCentrado()
        {
            Control cont = lblHora.Parent ?? this;

            lblHora.AutoSize = false;
            lblHora.TextAlign = ContentAlignment.MiddleCenter;
            lblHora.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblHora.Left = 0;
            lblHora.Width = cont.ClientSize.Width;
            lblHora.Top = 10;

            lblFecha.AutoSize = false;
            lblFecha.TextAlign = ContentAlignment.MiddleCenter;
            lblFecha.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFecha.Left = 0;
            lblFecha.Width = cont.ClientSize.Width;
            lblFecha.Top = lblHora.Bottom + 8;

            cont.Resize += (s, e) =>
            {
                lblHora.Width = cont.ClientSize.Width;
                lblFecha.Width = cont.ClientSize.Width;
                lblFecha.Top = lblHora.Bottom + 8;
            };
        }

        private void CargarTotales()
        {
            using (var cn = Conexion_SQL.OpenConnection())
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Vehiculos", cn))
                    lblTotalVehiculos.Text = ((int)cmd.ExecuteScalar()).ToString();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Propietarios", cn))
                    lblTotalPropietarios.Text = ((int)cmd.ExecuteScalar()).ToString();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users", cn))
                    lblTotalTrabajadores.Text = ((int)cmd.ExecuteScalar()).ToString();

                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Facturas", cn))
                    llblTotalFacturas.Text = ((int)cmd.ExecuteScalar()).ToString();
            }
        }

        private void Horafecha_Tick_1(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToLongTimeString();
            lblFecha.Text = DateTime.Now.ToLongDateString();
        }

        private void CargarVehiculos()
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    var dt = new DataTable();
                    string sqlNuevo = @"
                    SELECT 
                        V.Placa,
                        MV.Nombre   AS Marca,
                        ModV.Nombre AS Modelo,
                        V.Color,
                        V.Estado,
                        (P.Nombre + ' ' + P.Apellido) AS Propietario
                    FROM dbo.Vehiculos V
                    INNER JOIN dbo.Propietarios   P    ON P.ID_Propietario = V.ID_Propietario
                    INNER JOIN dbo.MarcaVehiculo  MV   ON MV.MarcaID       = V.MarcaID
                    INNER JOIN dbo.ModeloVehiculo ModV ON ModV.ModeloID    = V.ModeloID
                    ORDER BY V.VehicleID DESC;";
                    try
                    {
                        new SqlDataAdapter(sqlNuevo, cn).Fill(dt);
                    }
                    catch (SqlException ex) when (ex.Number == 207)
                    {
                        string sqlViejo = @"
                        SELECT 
                            V.Placa,
                            V.Marca     AS Marca,
                            V.Modelo    AS Modelo,
                            V.Color,
                            V.Estado,
                            (P.Nombre + ' ' + P.Apellido) AS Propietario
                        FROM dbo.Vehiculos V
                        LEFT JOIN dbo.Propietarios P ON P.ID_Propietario = V.ID_Propietario
                        ORDER BY V.VehicleID DESC;";
                        dt.Clear();
                        new SqlDataAdapter(sqlViejo, cn).Fill(dt);
                    }
                    dgvVehiculos.AutoGenerateColumns = true;
                    dgvVehiculos.DataSource = dt.Rows.Count > 0 ? dt : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los vehículos: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
