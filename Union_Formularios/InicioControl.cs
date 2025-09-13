using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                // Total de vehículos
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Vehiculos", cn))
                {
                    int totalVehiculos = (int)cmd.ExecuteScalar();
                    lblTotalVehiculos.Text = totalVehiculos.ToString();
                }
                // Total de propietarios
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Propietarios", cn))
                {
                    int totalPropietarios = (int)cmd.ExecuteScalar();
                    lblTotalPropietarios.Text = totalPropietarios.ToString();
                }
                // Total de trabajadores 
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", cn))
                {
                    int totalTrabajadores = (int)cmd.ExecuteScalar();
                    lblTotalTrabajadores.Text = totalTrabajadores.ToString();
                }
                // Total de facturas
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Facturas", cn))
                {
                    int totalFacturas = (int)cmd.ExecuteScalar();
                    llblTotalFacturas.Text = totalFacturas.ToString();
                }
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
                using (SqlConnection cn = Conexion_SQL.GetConnection()) 
                {
                    string consulta = @"
                    SELECT 
                        V.Placa,
                        Ma.Nombre AS Marca,
                        Mo.Nombre AS Modelo,
                        V.Color,
                        V.Estado,
                        (P.Nombre + ' ' + ISNULL(P.Apellido,'')) AS Propietario
                    FROM Vehiculos V
                    INNER JOIN Propietarios    P  ON V.ID_Propietario = P.ID_Propietario
                    INNER JOIN MarcaVehiculo   Ma ON V.MarcaID       = Ma.MarcaID
                    INNER JOIN ModeloVehiculo  Mo ON V.ModeloID      = Mo.ModeloID
                    ORDER BY V.Placa;";

                    using (SqlCommand cmd = new SqlCommand(consulta, cn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable tabla = new DataTable();
                        da.Fill(tabla);

                        dgvVehiculos.AutoGenerateColumns = true;
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
