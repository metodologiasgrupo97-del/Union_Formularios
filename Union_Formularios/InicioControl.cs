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
            string cadenaConexion = "Server=DESKTOP-9TRMID2;Database=CAR_EFULL;Integrated Security=true";

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();

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

                // Total de trabajadores o usuarios
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", cn))
                {
                    int totalTrabajadores = (int)cmd.ExecuteScalar();
                    lblTotalTrabajadores.Text = totalTrabajadores.ToString();
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
            using (SqlConnection cn = new SqlConnection("Server=DESKTOP-9TRMID2; DataBase=CAR_EFULL; Integrated Security=true"))
            {
                try
                {
                    cn.Open();
                    string consulta = @"
                        SELECT 
                            V.Placa,
                            V.Marca,
                            V.Modelo,
                            V.Color,
                            V.Estado,
                            P.Nombre AS Propietario
                        FROM Vehiculos V
                        INNER JOIN Propietarios P ON V.ID_Propietario = P.ID_Propietario";

                    SqlDataAdapter adaptador = new SqlDataAdapter(consulta, cn);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvVehiculos.DataSource = tabla;

                    dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvVehiculos.ReadOnly = true;
                    dgvVehiculos.AllowUserToAddRows = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los vehículos: " + ex.Message);
                }
            }
        }

        private void dgvVehiculos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
