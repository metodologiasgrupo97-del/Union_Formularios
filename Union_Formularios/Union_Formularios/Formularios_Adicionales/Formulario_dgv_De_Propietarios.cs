using Formulario_Principal_Car_EFULL.Formularios;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_dgv_De_Propietarios : Form
    {
        public Formulario_Vehiculos formPadre { get; set; }
        public Formulario_dgv_De_Propietarios()
        {
            InitializeComponent();
        }

        private void Formulario_dgv_De_Propietarios_Load(object sender, EventArgs e)
        {
            var propietariosDT = new DataTable();

            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(@"
        SELECT 
            ID_Propietario,
            Cedula,
            (Nombre + ' ' + ISNULL(Apellido, '')) AS NombreCompleto,  -- <- AQUI
            Telefono,
            Correo,
            Direccion,
            Estado,
            FechaRegistro
        FROM dbo.Propietarios
        WHERE Estado = 'Activo'
        ORDER BY Nombre, Apellido;", cn))
            {
                da.Fill(propietariosDT);
            }

            dgvPropietarios.AutoGenerateColumns = true;
            dgvPropietarios.DataSource = propietariosDT;
            if (dgvPropietarios.Columns.Contains("ID_Propietario"))
                dgvPropietarios.Columns["ID_Propietario"].Visible = false;

            dgvPropietarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPropietarios.ReadOnly = true;
            dgvPropietarios.MultiSelect = false;
        }
        private void dgvPropietarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // ignora encabezados

            var row = dgvPropietarios.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ID_Propietario"].Value);

            string nombre = row.Cells["NombreCompleto"].Value?.ToString() ?? "";

            formPadre?.EstablecerPropietario(id, nombre);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
