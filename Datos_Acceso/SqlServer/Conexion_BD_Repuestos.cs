using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Datos_Acceso.SqlServer
{
    public class Conexion_BD_Repuestos:Conexion_SQL
    {
        public DataTable ObtenerRepuestos()
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                string query = @"SELECT RepuestoID, Codigo, Nombre, Categoria, Marca, Modelo, PrecioUnitario, Stock FROM Repuestos WHERE Activo = 1";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
