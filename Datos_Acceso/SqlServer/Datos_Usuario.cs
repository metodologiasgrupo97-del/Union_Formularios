using Capa_Corte_Transversal;
using Capa_Corte_Transversal.Cache;
using Datos_Acceso;
using Datos_Acceso.MailServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Datos_Accesos
{
    public class Datos_Usuario : Conexion_SQL
    {
        public bool Login(string usser, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select * from Users where LoginName = @user and Password = @pass";
                    command.Parameters.AddWithValue("@user", usser);
                    command.Parameters.AddWithValue("@pass", password);
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            Cache_Inicio_Sesion_Usuario.UserID = reader.GetInt32(0);
                            Cache_Inicio_Sesion_Usuario.FirstName = reader.GetString(3);
                            Cache_Inicio_Sesion_Usuario.LastName = reader.GetString(4);
                            Cache_Inicio_Sesion_Usuario.Position = reader.GetString(5);
                            Cache_Inicio_Sesion_Usuario.Email = reader.GetString(6);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public void Cargo_de_trabajo() 
        {
            if(Cache_Inicio_Sesion_Usuario.Position == Cargos.Administrador) 
            {

            }
            if(Cache_Inicio_Sesion_Usuario.Position == Cargos.Trabajador) 
            {

            }
        }
        public string Recuperar_Contraseña(string pedir_usuario) 
        {
            using (var connection = GetConnection()) 
            {
                connection.Open();
                using (var command = new SqlCommand()) 
                { 
                    command.Connection = connection;
                    command.CommandText = "select * from Users where LoginName = @user or Email = @mail";
                    command.Parameters.AddWithValue("@user", pedir_usuario);
                    command.Parameters.AddWithValue("@mail", pedir_usuario);
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader();

                    if(reader.Read() == true) 
                    {
                        string nombre_usuario = reader.GetString(1);
                        string correo_usuario = reader.GetString(6);
                        string contraseña_usuario = reader.GetString(2);

                        var mailService = new Soporte_Del_Correo();
                        mailService.enviarmensajedecorreo(asunto: "SISTEMA: Proceso de Recuperación de Contraseña", cuerpo: "Hola, " + nombre_usuario + " \nHa solicitado recuperar su contraseña.\n" + "Su contraseña actual es: " + contraseña_usuario, destinatario_Correo: new List<string> {correo_usuario});
                        return "Hola, " + nombre_usuario + "\nHa solicitado recuperar su contraseña.\n" + "Por favor revise su correo: " + correo_usuario;
                    }
                    else
                    {
                        return "Lo sentimos, no tiene una cuenta con este\nnombre de usuario o correo eléctronico.";
                    }
                }
            }
        }
    }
}
