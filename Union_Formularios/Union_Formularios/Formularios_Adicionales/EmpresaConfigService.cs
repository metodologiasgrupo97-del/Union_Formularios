using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
// Asegúrate de tener esta referencia a tu conexión centralizada:
using Datos_Acceso.SqlServer; // Conexion_SQL

public static class EmpresaConfigService
{
    public class EmpresaDatos
    {
        public int EmpresaID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }           
        public string ColorPrimarioHex { get; set; }   
        public string ColorSecundarioHex { get; set; }  
        public byte[] Logo { get; set; }
        public string LogoMimeType { get; set; }

        public Color ColorPrimario => HexAColorSeguro(ColorPrimarioHex, Color.FromArgb(0, 122, 204));
        public Color ColorSecundario => HexAColorSeguro(ColorSecundarioHex, Color.FromArgb(52, 58, 64));
    }

    public static EmpresaDatos EmpresaActual { get; private set; } = new EmpresaDatos();
    public static event Action TemaCambiado;

    public static void Cargar()
    {
        var dto = new EmpresaDatos();

        using (var cn = Conexion_SQL.OpenConnection())
        using (var cmd = new SqlCommand(@"
            SELECT TOP 1 EmpresaID, RazonSocial, NombreComercial, RUC, Direccion, Telefono, Correo,
                   ColorPrimarioHex, ColorSecundarioHex, Logo, LogoMimeType
            FROM Empresa
            ORDER BY EmpresaID;", cn))
        using (var r = cmd.ExecuteReader())
        {
            if (r.Read())
            {
                dto.EmpresaID = r.GetInt32(0);
                dto.RazonSocial = r.IsDBNull(1) ? null : r.GetString(1);
                dto.NombreComercial = r.IsDBNull(2) ? null : r.GetString(2);
                dto.RUC = r.IsDBNull(3) ? null : r.GetString(3);
                dto.Direccion = r.IsDBNull(4) ? null : r.GetString(4);
                dto.Telefono = r.IsDBNull(5) ? null : r.GetString(5);
                dto.Correo = r.IsDBNull(6) ? null : r.GetString(6);   // NUEVO
                dto.ColorPrimarioHex = r.IsDBNull(7) ? null : NormalizaHex(r.GetString(7));
                dto.ColorSecundarioHex = r.IsDBNull(8) ? null : NormalizaHex(r.GetString(8));
                dto.Logo = r.IsDBNull(9) ? null : (byte[])r[9];
                dto.LogoMimeType = r.IsDBNull(10) ? null : r.GetString(10);
            }
        }

        EmpresaActual = dto;
    }

    public static void Guardar(EmpresaDatos datos)
    {
        if (datos == null) throw new ArgumentNullException(nameof(datos));

        datos.ColorPrimarioHex = NormalizaHex(datos.ColorPrimarioHex) ?? "#007ACC";
        datos.ColorSecundarioHex = NormalizaHex(datos.ColorSecundarioHex) ?? "#343A40";

        using (var cn = Conexion_SQL.OpenConnection())
        using (var tx = cn.BeginTransaction())
        {
            try
            {
                int count;
                using (var cmdCount = new SqlCommand("SELECT COUNT(1) FROM Empresa;", cn, tx))
                    count = (int)cmdCount.ExecuteScalar();

                if (count > 0)
                {
                    var sql = @"
                        UPDATE TOP (1) Empresa
                        SET RazonSocial        = @RS,
                            NombreComercial    = @NC,
                            RUC                = @RUC,
                            Direccion          = @DIR,
                            Telefono           = @TEL,
                            Correo             = COALESCE(NULLIF(@MAIL,''), Correo),
                            ColorPrimarioHex   = @C1,
                            ColorSecundarioHex = @C2";

                    bool actualizaLogo = datos.Logo != null && datos.Logo.Length > 0 && !string.IsNullOrWhiteSpace(datos.LogoMimeType);
                    if (actualizaLogo) sql += ", Logo = @LOGO, LogoMimeType = @MIME";

                    sql += ";";

                    using (var cmd = new SqlCommand(sql, cn, tx))
                    {
                        cmd.Parameters.Add("@RS", SqlDbType.NVarChar, 150).Value = (object)datos.RazonSocial ?? DBNull.Value;
                        cmd.Parameters.Add("@NC", SqlDbType.NVarChar, 150).Value = (object)datos.NombreComercial ?? DBNull.Value;
                        cmd.Parameters.Add("@RUC", SqlDbType.NVarChar, 20).Value = (object)datos.RUC ?? DBNull.Value;
                        cmd.Parameters.Add("@DIR", SqlDbType.NVarChar, 200).Value = (object)datos.Direccion ?? DBNull.Value;
                        cmd.Parameters.Add("@TEL", SqlDbType.NVarChar, 30).Value = (object)datos.Telefono ?? DBNull.Value;
                        cmd.Parameters.Add("@MAIL", SqlDbType.NVarChar, 150).Value = (object)datos.Correo ?? DBNull.Value; // NUEVO
                        cmd.Parameters.Add("@C1", SqlDbType.NVarChar, 7).Value = (object)datos.ColorPrimarioHex ?? DBNull.Value;
                        cmd.Parameters.Add("@C2", SqlDbType.NVarChar, 7).Value = (object)datos.ColorSecundarioHex ?? DBNull.Value;

                        if (actualizaLogo)
                        {
                            cmd.Parameters.Add("@LOGO", SqlDbType.VarBinary).Value = datos.Logo;
                            cmd.Parameters.Add("@MIME", SqlDbType.NVarChar, 50).Value = datos.LogoMimeType;
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO Empresa
                            (RazonSocial, NombreComercial, RUC, Direccion, Telefono, Correo,
                             ColorPrimarioHex, ColorSecundarioHex, Logo, LogoMimeType)
                        VALUES
                            (@RS, @NC, @RUC, @DIR, @TEL, @MAIL,
                             @C1, @C2, @LOGO, @MIME);", cn, tx))
                    {
                        cmd.Parameters.Add("@RS", SqlDbType.NVarChar, 150).Value = (object)datos.RazonSocial ?? DBNull.Value;
                        cmd.Parameters.Add("@NC", SqlDbType.NVarChar, 150).Value = (object)datos.NombreComercial ?? DBNull.Value;
                        cmd.Parameters.Add("@RUC", SqlDbType.NVarChar, 20).Value = (object)datos.RUC ?? DBNull.Value;
                        cmd.Parameters.Add("@DIR", SqlDbType.NVarChar, 200).Value = (object)datos.Direccion ?? DBNull.Value;
                        cmd.Parameters.Add("@TEL", SqlDbType.NVarChar, 30).Value = (object)datos.Telefono ?? DBNull.Value;
                        cmd.Parameters.Add("@MAIL", SqlDbType.NVarChar, 150).Value = (object)datos.Correo ?? DBNull.Value; // NUEVO
                        cmd.Parameters.Add("@C1", SqlDbType.NVarChar, 7).Value = (object)datos.ColorPrimarioHex ?? DBNull.Value;
                        cmd.Parameters.Add("@C2", SqlDbType.NVarChar, 7).Value = (object)datos.ColorSecundarioHex ?? DBNull.Value;
                        cmd.Parameters.Add("@LOGO", SqlDbType.VarBinary).Value = (object)datos.Logo ?? DBNull.Value;
                        cmd.Parameters.Add("@MIME", SqlDbType.NVarChar, 50).Value = (object)datos.LogoMimeType ?? DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        RecargarYNotificar();
    }

    public static void RecargarYNotificar()
    {
        Cargar();
        TemaCambiado?.Invoke();
    }

    public static string ColorAHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    private static string NormalizaHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        hex = hex.Trim();
        if (!hex.StartsWith("#")) hex = "#" + hex;
        if (hex.Length == 4) // #RGB -> #RRGGBB
        {
            hex = $"#{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
        }
        return hex.ToUpperInvariant();
    }

    public static Color HexAColorSeguro(string hex, Color predeterminado)
    {
        if (string.IsNullOrWhiteSpace(hex)) return predeterminado;
        try
        {
            hex = NormalizaHex(hex);
            return ColorTranslator.FromHtml(hex);
        }
        catch { return predeterminado; }
    }

    public static Image ObtenerLogoComoImagen()
    {
        if (EmpresaActual?.Logo == null || EmpresaActual.Logo.Length == 0) return null;
        using (var ms = new MemoryStream(EmpresaActual.Logo))
        using (var tmp = Image.FromStream(ms, useEmbeddedColorManagement: true, validateImageData: true))
        {
            return new Bitmap(tmp);
        }
    }

    public static void AplicarColor(Control control, Color color)
    {
        if (control == null) return;
        var propFill = control.GetType().GetProperty("FillColor");
        if (propFill != null && propFill.PropertyType == typeof(Color) && propFill.CanWrite)
        {
            propFill.SetValue(control, color);
        }
        else
        {
            control.BackColor = color;
        }
    }

    public static string TruncarNombreArchivo(string nombreArchivo, int max = 15)
    {
        if (string.IsNullOrWhiteSpace(nombreArchivo)) return "";
        if (nombreArchivo.Length <= max) return nombreArchivo;

        string ext = Path.GetExtension(nombreArchivo);
        string baseName = Path.GetFileNameWithoutExtension(nombreArchivo);
        int reservar = Math.Max(1, max - (ext?.Length ?? 0) - 3);
        if (reservar < 1) reservar = 1;
        if (reservar > baseName.Length) reservar = baseName.Length;

        return baseName.Substring(0, reservar) + "..." + ext;
    }
}
