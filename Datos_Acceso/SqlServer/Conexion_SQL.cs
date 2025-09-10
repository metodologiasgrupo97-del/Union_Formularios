// Datos_Acceso.SqlServer.Conexion_SQL
using System.Data.SqlClient;

namespace Datos_Acceso.SqlServer
{
    public abstract class Conexion_SQL
    {
        // Ajusta tu cadena de conexión aquí
        private static readonly string _cs = "Server=DESKTOP-9TRMID2;Database=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection OpenConnection()
        {
            var cn = new SqlConnection(_cs);  // instancia nueva SIEMPRE
            cn.Open();
            return cn;
        }
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_cs);    
        }
    }
}
