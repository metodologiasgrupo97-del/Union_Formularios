using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Datos_Acceso;
using Datos_Accesos;
using Capa_Corte_Transversal.Cache;

namespace Dominio
{
    public class Modelo_Dominio_Usuario
    {
        Datos_Usuario Datos_Usuario = new Datos_Usuario();
        public bool LoginUser(string usser, string password)
        {
            return Datos_Usuario.Login(usser, password);
        }
        public void Cargo_de_trabajo()
        {
            if (Cache_Inicio_Sesion_Usuario.Position == "Administrador")
            {

            }
            if (Cache_Inicio_Sesion_Usuario.Position == "Trabajador")
            {

            }
        }
        public string Recuperar_Contraseña(string pedir_usuario)
        {
            return Datos_Usuario.Recuperar_Contraseña(pedir_usuario);
        }
    }
}