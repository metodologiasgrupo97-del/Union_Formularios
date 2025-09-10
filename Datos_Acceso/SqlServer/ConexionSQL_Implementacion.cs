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
        // Devuelve una conexión usando la clase abstracta (sin abrirla aquí)
        public SqlConnection AbrirConexion()
        {
            // Solo delega: NO hay string literal, NO abre la conexión
            return Conexion_SQL.GetConnection();
        }
    }
}
