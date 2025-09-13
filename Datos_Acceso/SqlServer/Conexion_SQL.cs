using System;
using System.Data.SqlClient;
using System.IO;

public abstract class Conexion_SQL
{
    // Variable estática para guardar el nombre del servidor
    private static string _serverName;

    static Conexion_SQL()
    {
        // Verificar si ya se ha guardado el nombre del servidor
        if (string.IsNullOrEmpty(_serverName))
        {
            _serverName = ObtenerServidorDesdeArchivo();
            if (string.IsNullOrEmpty(_serverName))
            {
                _serverName = PedirServidor();
                GuardarServidorEnArchivo(_serverName);
            }
        }
    }

    // Obtener el servidor desde un archivo de configuración (si existe)
    private static string ObtenerServidorDesdeArchivo()
    {
        string filePath = "servidor_config.txt";
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath); 
        }
        return null;
    }

    public static void GuardarServidorEnArchivo(string serverName)
    {
        string filePath = "servidor_config.txt";
        File.WriteAllText(filePath, serverName); // Guardar el nombre del servidor
    }

    // Método para pedir el nombre del servidor al usuario
    private static string PedirServidor()
    {
        Console.WriteLine("Por favor, ingrese el nombre del servidor:");
        return Console.ReadLine(); 
    }

    // Obtener la conexión con el servidor
    public static SqlConnection OpenConnection()
    {
        string connectionString = $"Server={_serverName};Database=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True;";
        var cn = new SqlConnection(connectionString);
        cn.Open();
        return cn;
    }

    public static SqlConnection GetConnection()
    {
        string connectionString = $"Server={_serverName};Database=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True;";
        return new SqlConnection(connectionString);
    }
}