using Datos_Acceso.SqlServer;
using Formulario_Principal_Car_EFULL.Formularios;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_Seleccionar_Trabajador : Form
    {
        public Formulario_Mantenimiento formPadre { get; set; }
        public event Action<int, string> TrabajadorSeleccionado;

        // --- Constructores ---
        public Formulario_Seleccionar_Trabajador()   
        {
            InitializeComponent();
        }

        public Formulario_Seleccionar_Trabajador(Formulario_Mantenimiento padre)
        {
            InitializeComponent();
            this.formPadre = padre;
        }

        private void Formulario_Seleccionar_Trabajador_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable trabajadoresDT = new DataTable();
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    string query = @"SELECT UserID, FirstName AS Nombre, LastName AS Apellido, Position AS Cargo 
                                     FROM Users 
                                     WHERE Position = 'Trabajador'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, cn);
                    adapter.Fill(trabajadoresDT);
                }

                dgvTrabajadores.DataSource = trabajadoresDT;

                if (dgvTrabajadores.Columns.Contains("UserID"))
                    dgvTrabajadores.Columns["UserID"].Visible = false;

                dgvTrabajadores.ReadOnly = true;
                dgvTrabajadores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvTrabajadores.AllowUserToAddRows = false;
                dgvTrabajadores.AllowUserToDeleteRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar los trabajadores:\n" + ex.Message,
                    "Trabajadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTrabajadores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvTrabajadores.Rows[e.RowIndex];

                int id = Convert.ToInt32(row.Cells["UserID"].Value);
                string nombre = row.Cells["Nombre"].Value.ToString();
                string apellido = row.Cells["Apellido"].Value.ToString();
                string nombreCompleto = $"{nombre} {apellido}".Trim();

                formPadre?.EstablecerTrabajador(id, nombreCompleto);

                TrabajadorSeleccionado?.Invoke(id, nombreCompleto);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible seleccionar el trabajador:\n" + ex.Message,
                    "Trabajadores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
