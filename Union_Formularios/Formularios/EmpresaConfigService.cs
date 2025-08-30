using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

public class EmpresaDto
{
    public int EmpresaID { get; set; }
    public string RazonSocial { get; set; }
    public string NombreComercial { get; set; }
    public string RUC { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string ColorPrimarioHex { get; set; }
    public string ColorSecundarioHex { get; set; }
    public byte[] Logo { get; set; }
    public string LogoMimeType { get; set; }
}

public static class EmpresaConfigService
{
    public static string ConnStr = "Data Source=.;Initial Catalog=CAR_EFULL;Integrated Security=True;TrustServerCertificate=True";

    public static event Action<EmpresaDto> EmpresaChanged;

    public static EmpresaDto GetEmpresa()
    {
        try
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT TOP 1 EmpresaID,RazonSocial,NombreComercial,RUC,Direccion,Telefono,
                                                     ColorPrimarioHex,ColorSecundarioHex,Logo,LogoMimeType
                                              FROM Empresa ORDER BY EmpresaID", cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new EmpresaDto
                    {
                        EmpresaID = r.GetInt32(0),
                        RazonSocial = r.IsDBNull(1) ? null : r.GetString(1),
                        NombreComercial = r.IsDBNull(2) ? null : r.GetString(2),
                        RUC = r.IsDBNull(3) ? null : r.GetString(3),
                        Direccion = r.IsDBNull(4) ? null : r.GetString(4),
                        Telefono = r.IsDBNull(5) ? null : r.GetString(5),
                        ColorPrimarioHex = r.IsDBNull(6) ? null : r.GetString(6),
                        ColorSecundarioHex = r.IsDBNull(7) ? null : r.GetString(7),
                        Logo = r.IsDBNull(8) ? null : (byte[])r[8],
                        LogoMimeType = r.IsDBNull(9) ? null : r.GetString(9)
                    };
                }
            }
        }
        catch { return null; }
    }

    public static void UpsertEmpresa(EmpresaDto dto)
    {
        try
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM Empresa) UPDATE Empresa SET RazonSocial=@RS, NombreComercial=@NC, RUC=@RUC, Direccion=@DIR, Telefono=@TEL, ColorPrimarioHex=@C1, ColorSecundarioHex=@C2,Logo = COALESCE(@LOGO, Logo), LogoMimeType = COALESCE(@MIME, LogoMimeType),LogoUpdatedAt=SYSDATETIME() ELSE  INSERT INTO Empresa(RazonSocial,NombreComercial,RUC,Direccion,Telefono,ColorPrimarioHex,ColorSecundarioHex,Logo,LogoMimeType) VALUES(@RS,@NC,@RUC,@DIR,@TEL,@C1,@C2,@LOGO,@MIME);", cn))
            {
                cmd.Parameters.Add("@RS", SqlDbType.NVarChar, 150).Value = (object)dto.RazonSocial ?? DBNull.Value;
                cmd.Parameters.Add("@NC", SqlDbType.NVarChar, 150).Value = (object)dto.NombreComercial ?? DBNull.Value;
                cmd.Parameters.Add("@RUC", SqlDbType.NVarChar, 20).Value = (object)dto.RUC ?? DBNull.Value;
                cmd.Parameters.Add("@DIR", SqlDbType.NVarChar, 200).Value = (object)dto.Direccion ?? DBNull.Value;
                cmd.Parameters.Add("@TEL", SqlDbType.NVarChar, 30).Value = (object)dto.Telefono ?? DBNull.Value;
                cmd.Parameters.Add("@C1", SqlDbType.NVarChar, 9).Value = (object)dto.ColorPrimarioHex ?? DBNull.Value;
                cmd.Parameters.Add("@C2", SqlDbType.NVarChar, 9).Value = (object)dto.ColorSecundarioHex ?? DBNull.Value;
                cmd.Parameters.Add("@LOGO", SqlDbType.VarBinary).Value = (object)dto.Logo ?? DBNull.Value;
                cmd.Parameters.Add("@MIME", SqlDbType.NVarChar, 50).Value = (object)dto.LogoMimeType ?? DBNull.Value;
                cn.Open();
                cmd.ExecuteNonQuery();
            }

            EmpresaChanged?.Invoke(GetEmpresa()); // notifica a oyentes (dashboard, reportes…)
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("No se pudo guardar Empresa:\n" + ex.Message,
                "Configuración", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
    }

    public static string ColorToHex(Color c, bool includeAlpha = false)
    {
        return includeAlpha
            ? $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}"
            : $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }

    public static Color HexToColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return Color.Empty;
        hex = hex.TrimStart('#');
        if (hex.Length == 8) // AARRGGBB
            return Color.FromArgb(
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16),
                Convert.ToInt32(hex.Substring(6, 2), 16));
        if (hex.Length == 6) // RRGGBB
            return Color.FromArgb(255,
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16));
        return Color.Empty;
    }
}
