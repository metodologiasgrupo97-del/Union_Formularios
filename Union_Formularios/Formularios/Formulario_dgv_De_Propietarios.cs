using Formulario_Principal_Car_EFULL.Formularios;
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

namespace Union_Formularios.Formularios
{
    public partial class Formulario_dgv_De_Propietarios : Form
    {
        public Formulario_Vehiculos formPadre { get; set; }

        public Formulario_dgv_De_Propietarios()
        {
            InitializeComponent();
        }
        private void dgvPropietarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dgvPropietarios.Rows[e.RowIndex].Cells["ID_Propietario"].Value);
                string nombre = dgvPropietarios.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                string apellido = dgvPropietarios.Rows[e.RowIndex].Cells["Apellido"].Value.ToString();
                string nombreCompleto = $"{nombre} {apellido}";

                formPadre.EstablecerPropietario(id, nombreCompleto);
                this.Close(); 
            }
        }

        private void Formulario_dgv_De_Propietarios_Load(object sender, EventArgs e)
        {
            DataTable propietariosDT = new DataTable();

            using (SqlConnection cn = new SqlConnection("Server=DESKTOP-9TRMID2; DataBase=CAR_EFULL; Integrated Security=true"))
            {
                cn.Open();
                string query = "SELECT ID_Propietario, Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro FROM Propietarios WHERE Estado = 'Activo'";
                SqlDataAdapter adapter = new SqlDataAdapter(query, cn);
                adapter.Fill(propietariosDT);
            }

            dgvPropietarios.DataSource = propietariosDT;
            if (dgvPropietarios.Columns.Contains("ID_Propietario"))
            {
                dgvPropietarios.Columns["ID_Propietario"].Visible = false;
            }
        }

    }
}
