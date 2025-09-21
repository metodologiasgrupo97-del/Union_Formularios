using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Datos_Acceso.SqlServer
{
    public static class RepuestosDAO
    {
        private static object Db(object v) { return v == null ? (object)DBNull.Value : v; }

        // --------- Datos fijos (categorías) ----------
        public static readonly string[] CategoriasFijas = new[]
        {
            "Motor","Transmisión","Sistema de frenos","Suspensión y dirección","Sistema eléctrico",
            "Carrocería y cabina","Sistema de combustible","Refrigeración y aire",
            "Sistema de escape","Lubricantes y clasificadores","Neumáticos y llantas",
            "Rodamientos, pernos y tuercas","Turbo, filtros y correas","Inyección","Sensores","Filtros"
        };

        // --------- Código siguiente ----------
        public static string ObtenerSiguienteCodigo(SqlConnection cn)
        {
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 Codigo 
                FROM dbo.Repuestos
                WHERE Codigo LIKE 'RPT-%'
                ORDER BY RepuestoID DESC;", cn))
            {
                var last = cmd.ExecuteScalar() as string;
                if (string.IsNullOrWhiteSpace(last)) return "RPT-001";

                var m = Regex.Match(last, @"^RPT-(\d+)$");
                if (!m.Success) return "RPT-001";

                int n = int.Parse(m.Groups[1].Value);
                return "RPT-" + (n + 1).ToString("000");
            }
        }

        // --------- Combos (catálogos) ----------
        public static DataTable GetTiposVehiculo()
        {
            using (var cn = Conexion_SQL.OpenConnection())
            using (var da = new SqlDataAdapter(
                "SELECT TipoID, Nombre FROM dbo.TipoVehiculo ORDER BY Nombre;", cn))
            { var dt = new DataTable(); da.Fill(dt); return dt; }
        }

        public static DataTable GetMarcasPorTipo(int tipoId)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            using (var cmd = new SqlCommand(@"
        SELECT MarcaID, Nombre
        FROM dbo.MarcaVehiculo
        WHERE TipoID = @t
        ORDER BY Nombre;", cn))
            {
                cmd.Parameters.AddWithValue("@t", tipoId);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetModelosPorMarca(int marcaId)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            using (var cmd = new SqlCommand(@"
        SELECT ModeloID, Nombre
        FROM dbo.ModeloVehiculo
        WHERE MarcaID = @m
        ORDER BY Nombre;", cn))
            {
                cmd.Parameters.AddWithValue("@m", marcaId);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // --------- Listado para el grid ----------
        // Usa la vista vw_RepuestosFull (trae nombres e IDs)
        public static DataTable Listar(int? tipoId, int? marcaId, int? modeloId,
                                       string categoria, string tipoRepuesto)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            {
                var sb = new StringBuilder();
                sb.Append(@"
                    SELECT 
                        RepuestoID, Codigo, Nombre, Categoria, TipoRepuesto,
                        TipoVehiculo, Marca, Modelo,
                        PrecioUnitario, Stock, Activo,
                        TipoID, MarcaID, ModeloID
                    FROM dbo.vw_RepuestosFull
                    WHERE 1=1");

                var cmd = new SqlCommand(); cmd.Connection = cn;

                if (tipoId.HasValue) { sb.Append(" AND TipoID=@t"); cmd.Parameters.AddWithValue("@t", tipoId.Value); }
                if (marcaId.HasValue) { sb.Append(" AND MarcaID=@m"); cmd.Parameters.AddWithValue("@m", marcaId.Value); }
                if (modeloId.HasValue) { sb.Append(" AND ModeloID=@mo"); cmd.Parameters.AddWithValue("@mo", modeloId.Value); }
                if (!string.IsNullOrWhiteSpace(categoria))
                { sb.Append(" AND Categoria=@c"); cmd.Parameters.AddWithValue("@c", categoria); }
                if (!string.IsNullOrWhiteSpace(tipoRepuesto))
                { sb.Append(" AND TipoRepuesto=@tr"); cmd.Parameters.AddWithValue("@tr", tipoRepuesto); }

                sb.Append(" ORDER BY RepuestoID DESC;");
                cmd.CommandText = sb.ToString();

                using (var da = new SqlDataAdapter(cmd))
                { var dt = new DataTable(); da.Fill(dt); return dt; }
            }
        }

        // --------- Insert / Update / Delete lógico ----------
        public static int Insertar(string codigo, string nombre, string categoria, string tipoRepuesto,
                                   int? tipoId, int? marcaId, int? modeloId,
                                   decimal precio, int stock, bool activo,
                                   int? impuestoDefault)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            {
                // Traemos textos de marca/modelo para columnas espejo (compatibilidad UI)
                string marcaTxt = null, modeloTxt = null;
                if (marcaId.HasValue)
                {
                    using (var cmdM = new SqlCommand("SELECT Nombre FROM dbo.MarcaVehiculo WHERE MarcaID=@id;", cn))
                    { cmdM.Parameters.AddWithValue("@id", marcaId.Value); marcaTxt = cmdM.ExecuteScalar() as string; }
                }
                if (modeloId.HasValue)
                {
                    using (var cmdMo = new SqlCommand("SELECT Nombre FROM dbo.ModeloVehiculo WHERE ModeloID=@id;", cn))
                    { cmdMo.Parameters.AddWithValue("@id", modeloId.Value); modeloTxt = cmdMo.ExecuteScalar() as string; }
                }

                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.Repuestos
                    (Codigo, Nombre, Categoria, TipoRepuesto,
                     Marca, Modelo, TipoID, MarcaID, ModeloID,
                     PrecioUnitario, Stock, Activo, ImpuestoID_Default)
                    OUTPUT INSERTED.RepuestoID
                    VALUES
                    (@Codigo, @Nombre, @Categoria, @TipoRepuesto,
                     @MarcaTxt, @ModeloTxt, @TipoID, @MarcaID, @ModeloID,
                     @Precio, @Stock, @Activo, @Imp);", cn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Categoria", Db(categoria));
                    cmd.Parameters.AddWithValue("@TipoRepuesto", tipoRepuesto);
                    cmd.Parameters.AddWithValue("@MarcaTxt", Db(marcaTxt));
                    cmd.Parameters.AddWithValue("@ModeloTxt", Db(modeloTxt));
                    cmd.Parameters.AddWithValue("@TipoID", (object)tipoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MarcaID", (object)marcaId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeloID", (object)modeloId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@Activo", activo ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Imp", (object)impuestoDefault ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void Actualizar(int repuestoId, string codigo, string nombre, string categoria, string tipoRepuesto,
                                      int? tipoId, int? marcaId, int? modeloId,
                                      decimal precio, int stock, bool activo,
                                      int? impuestoDefault)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            {
                string marcaTxt = null, modeloTxt = null;
                if (marcaId.HasValue)
                {
                    using (var cmdM = new SqlCommand("SELECT Nombre FROM dbo.MarcaVehiculo WHERE MarcaID=@id;", cn))
                    { cmdM.Parameters.AddWithValue("@id", marcaId.Value); marcaTxt = cmdM.ExecuteScalar() as string; }
                }
                if (modeloId.HasValue)
                {
                    using (var cmdMo = new SqlCommand("SELECT Nombre FROM dbo.ModeloVehiculo WHERE ModeloID=@id;", cn))
                    { cmdMo.Parameters.AddWithValue("@id", modeloId.Value); modeloTxt = cmdMo.ExecuteScalar() as string; }
                }

                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.Repuestos SET
                        Codigo=@Codigo, Nombre=@Nombre, Categoria=@Categoria, TipoRepuesto=@TipoRepuesto,
                        Marca=@MarcaTxt, Modelo=@ModeloTxt, 
                        TipoID=@TipoID, MarcaID=@MarcaID, ModeloID=@ModeloID,
                        PrecioUnitario=@Precio, Stock=@Stock, Activo=@Activo, ImpuestoID_Default=@Imp
                    WHERE RepuestoID=@Id;", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", repuestoId);
                    cmd.Parameters.AddWithValue("@Codigo", codigo);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Categoria", Db(categoria));
                    cmd.Parameters.AddWithValue("@TipoRepuesto", tipoRepuesto);
                    cmd.Parameters.AddWithValue("@MarcaTxt", Db(marcaTxt));
                    cmd.Parameters.AddWithValue("@ModeloTxt", Db(modeloTxt));
                    cmd.Parameters.AddWithValue("@TipoID", (object)tipoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MarcaID", (object)marcaId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeloID", (object)modeloId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@Activo", activo ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Imp", (object)impuestoDefault ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Desactivar(int repuestoId)
        {
            using (var cn = Conexion_SQL.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE dbo.Repuestos SET Activo=0 WHERE RepuestoID=@Id;", cn))
            { cmd.Parameters.AddWithValue("@Id", repuestoId); cmd.ExecuteNonQuery(); }
        }
    }
}
