using Datos_Acceso;
using Datos_Acceso.SqlServer;
using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Dominio
{
    public class Modelo_Registro_Usuario
    {
        public Registro_Datos_Usuario registroUsuario = new Registro_Datos_Usuario();

        public string RegistrarTrabajador(string usuario, string contraseña, string nombre, string apellido, string puesto, string correo, string telefono, byte[] foto)
        {
            return registroUsuario.RegistrarTrabajador(usuario, contraseña, nombre, apellido, puesto, correo, telefono,foto);
        }

        public static bool UsuarioExiste(string usuario, string correo)
        {
            using (SqlConnection connection = new ConexionSQL_Implementacion().AbrirConexion())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE LoginName = @usuario OR Email = @correo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@usuario", usuario);
                    command.Parameters.AddWithValue("@correo", correo);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
