﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Datos_Acceso.SqlServer
{
    public class ConexionSQL_Implementacion : Conexion_SQL
    {
        public SqlConnection AbrirConexion()
        {
            return GetConnection();
        }
    }

}

