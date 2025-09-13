using System;
using Dominio;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Datos_Acceso.SqlServer;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Seleccionar_Vehiculo : Form
    {
        public event Action<Modelo_Dominio_Vehiculos> VehiculoSeleccionado;

        public Formulario_Seleccionar_Vehiculo()
        {
            InitializeComponent();
            this.Load += Formulario_Seleccionar_Vehiculo_Load;
            this.dgvVehiculo.CellDoubleClick += dgvVehiculo_CellDoubleClick;
        }

        private void Formulario_Seleccionar_Vehiculo_Load(object sender, EventArgs e)
        {
            try
            {
                var dt = new DataTable();
                using (var cn = Conexion_SQL.OpenConnection())
                using (var da = new SqlDataAdapter(@"
                SELECT
                    V.VehicleID,
                    V.Placa,
                    V.Tipo      AS Tipo,
                    V.Marca     AS Marca,
                    V.Modelo    AS Modelo,
                    V.Anio      AS AnioModelo,
                    V.NumeroMotor,
                    V.NumeroChasis,
                    V.Color,
                    V.Combustible,
                    V.Kilometraje,
                    V.Estado,
                    V.ID_Propietario,
                    (P.Nombre + ' ' + P.Apellido) AS Propietario
                FROM dbo.Vehiculos V
                LEFT JOIN dbo.Propietarios P ON P.ID_Propietario = V.ID_Propietario
                ORDER BY V.Placa;", cn))
                {
                    da.Fill(dt);
                }

                dgvVehiculo.DataSource = dt;

                if (dgvVehiculo.Columns.Contains("VehicleID"))
                    dgvVehiculo.Columns["VehicleID"].Visible = false;
                if (dgvVehiculo.Columns.Contains("ID_Propietario"))
                    dgvVehiculo.Columns["ID_Propietario"].Visible = false;

                dgvVehiculo.ReadOnly = true;
                dgvVehiculo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVehiculo.AllowUserToAddRows = false;
                dgvVehiculo.AllowUserToDeleteRows = false;
                dgvVehiculo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar los vehículos:\n" + ex.Message,
                    "Vehículos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dgvVehiculo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                var row = ((DataRowView)dgvVehiculo.Rows[e.RowIndex].DataBoundItem).Row;

                var v = new Modelo_Dominio_Vehiculos
                {
                    VehicleID = row.Field<int>("VehicleID"),
                    Placa = row.Field<string>("Placa"),
                    Tipo = row.Field<string>("Tipo"),
                    Marca = row.Field<string>("Marca"),
                    Modelo = row.Field<string>("Modelo"),
                    AnioModelo = row.Field<int>("AnioModelo"),
                    NumeroMotor = row.Field<string>("NumeroMotor"),
                    NumeroChasis = row.Field<string>("NumeroChasis"),
                    Color = row.Field<string>("Color"),
                    Combustible = row.Field<string>("Combustible"),
                    Kilometraje = row.Field<int>("Kilometraje"),
                    Estado = row.Field<string>("Estado"),
                    ID_Propietario = row.Field<int>("ID_Propietario"),
                    Propietario = row.Field<string>("Propietario")
                };

                VehiculoSeleccionado?.Invoke(v);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible seleccionar el vehículo:\n" + ex.Message,
                    "Vehículos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
