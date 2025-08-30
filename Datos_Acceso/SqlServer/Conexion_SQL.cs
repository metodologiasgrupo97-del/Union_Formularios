using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Datos_Acceso
{
    public abstract class Conexion_SQL
    {
        private readonly string Connection_String;
        public Conexion_SQL()
        {
            Connection_String = "Server = DESKTOP-9TRMID2; DataBase = CAR_EFULL; integrated security = true";
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(Connection_String);
        }
        public class ConexionVehiculo : Conexion_SQL
        {
            public SqlConnection Conectar()
            {
                return GetConnection();
            }
        }
    }
}