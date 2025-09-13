using System;
using System.Data.SqlClient;
using System.IO;

public abstract class Conexion_SQL
{
    private static string _serverName;

    static Conexion_SQL()
    {
        _serverName = ObtenerServidorDesdeArchivo();
        // 👇 Ya no lanza excepción aquí
        if (string.IsNullOrWhiteSpace(_serverName))
        {
            _serverName = null; // queda en null hasta que Pedir_Nom_Servidor lo guarde
        }
    }

    private static string ObtenerServidorDesdeArchivo()
    {
        string filePath = "servidor_config.txt";
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath).Trim();
        }
        return null;
    }

    public static void GuardarServidorEnArchivo(string serverName)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío.");

        string filePath = "servidor_config.txt";
        File.WriteAllText(filePath, serverName);
        _serverName = serverName; // importante: actualizar la variable estática
    }

    public static SqlConnection OpenConnection()
    {
        if (string.IsNullOrWhiteSpace(_serverName))
            throw new InvalidOperationException("Falta configurar el servidor. Use Pedir_Nom_Servidor primero.");

        string connectionString = $"Server={_serverName};Database=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True;";
        var cn = new SqlConnection(connectionString);
        cn.Open();
        return cn;
    }

    public static SqlConnection GetConnection()
    {
        if (string.IsNullOrWhiteSpace(_serverName))
            throw new InvalidOperationException("Falta configurar el servidor. Use Pedir_Nom_Servidor primero.");

        string connectionString = $"Server={_serverName};Database=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True;";
        return new SqlConnection(connectionString);
    }
}
