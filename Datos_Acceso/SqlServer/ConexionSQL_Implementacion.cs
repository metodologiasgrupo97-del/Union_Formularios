using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Datos_Acceso.SqlServer
{
    public class ConexionSQL_Implementacion
    {
        private string connectionString = "Server=DESKTOP-9TRMID2; DataBase=CAR_EFULL; integrated security=true"; 

        public SqlConnection AbrirConexion()
        {
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
    }


}

