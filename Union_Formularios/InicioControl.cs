using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Datos_Acceso.SqlServer;

namespace Union_Formularios
{
    public partial class InicioControl : UserControl
    {
        public InicioControl()
        {
            InitializeComponent();
            CargarTotales();
            CargarVehiculos();
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
                    const string sql = @"
                SELECT 
                    v.Placa,
                    ISNULL(NULLIF(v.Marca, ''), '(sin marca)')  AS Marca,
                    ISNULL(NULLIF(v.Modelo,''), '(sin modelo)') AS Modelo,
                    v.Color,
                    v.Estado,
                    (p.Nombre + ' ' + ISNULL(p.Apellido,'')) AS Propietario
                FROM dbo.Vehiculos v
                INNER JOIN dbo.Propietarios p ON p.ID_Propietario = v.ID_Propietario
                ORDER BY v.Placa;";

                    using (var da = new SqlDataAdapter(sql, cn))
                    {
                        var tabla = new DataTable();
                        da.Fill(tabla);
                        dgvVehiculos.DataSource = tabla;
                        dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvVehiculos.ReadOnly = true;
                        dgvVehiculos.AllowUserToAddRows = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los vehículos: " + ex.Message);
            }
        }
    }
}
