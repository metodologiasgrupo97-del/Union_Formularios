using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos_Acceso.SqlServer;
using System.Data.SqlClient;

namespace Datos_Acceso.SqlServer
{
    public class Registro_Datos_Propietario
    {
        private readonly ConexionSQL_Implementacion conexion = new ConexionSQL_Implementacion();

        public bool CedulaExiste(string cedula)
        {
            using (SqlConnection con = conexion.AbrirConexion())
            {
                con.Open(); 

                string query = "SELECT COUNT(*) FROM Propietarios WHERE Cedula = @cedula";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


    }
}
