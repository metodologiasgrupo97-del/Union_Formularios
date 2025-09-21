using Datos_Acceso.SqlServer; // <- NECESARIO para Conexion_SQL
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_EdtUsu : Form
    {
        public class UserEditData
        {
            public int Id;
            public string Usuario;
            public string Contrasena;
            public string Nombre;
            public string Apellido;
            public string Correo;
            public string Position; 
            public Image Foto;
        }

        public event Action<UserEditData> UsuarioSeleccionado;

        public Formulario_EdtUsu()
        {
            InitializeComponent();
            this.Load += Formulario_EdtUsu_Load;
            btn_Eliminar_Usu.Click += btn_Eliminar_Usu_Click;
            dgv_Trabajadores_agg.CellDoubleClick += dgv_Trabajadores_agg_CellDoubleClick;
        }

        private void Formulario_EdtUsu_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarUsuarios();
        }

        private DataGridViewTextBoxColumn MkText(string name, int width)
        {
            var c = new DataGridViewTextBoxColumn();
            c.Name = name;
            c.HeaderText = name;
            c.DataPropertyName = name;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            c.Width = width;
            return c;
        }

        private void ConfigurarGrid()
        {
            dgv_Trabajadores_agg.AutoGenerateColumns = false;
            dgv_Trabajadores_agg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_Trabajadores_agg.MultiSelect = false;
            dgv_Trabajadores_agg.AllowUserToAddRows = false;
            dgv_Trabajadores_agg.Columns.Clear();

            var colImg = new DataGridViewImageColumn();
            colImg.HeaderText = "Foto de perfil";
            colImg.Name = "FotoPerfil";
            colImg.DataPropertyName = "FotoPerfil";
            colImg.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colImg.Width = 70;
            dgv_Trabajadores_agg.Columns.Add(colImg);

            dgv_Trabajadores_agg.Columns.Add(MkText("ID", 80));
            dgv_Trabajadores_agg.Columns.Add(MkText("Usuario", 150));
            dgv_Trabajadores_agg.Columns.Add(MkText("Contrasena", 150));
            dgv_Trabajadores_agg.Columns.Add(MkText("Nombre", 130));
            dgv_Trabajadores_agg.Columns.Add(MkText("Apellido", 130));
            dgv_Trabajadores_agg.Columns.Add(MkText("Correo", 220));
            dgv_Trabajadores_agg.Columns.Add(MkText("Position", 120)); 
        }

        private void CargarUsuarios()
        {
            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    string idCol = FindColQ(cn, "Users", "UserID", "ID_Usuario", "ID", "UsuarioID");
                    if (idCol == null)
                        throw new InvalidOperationException("No se encontró columna de ID en dbo.Users.");

                    string loginCol = FindColQ(cn, "Users", "LoginName");
                    if (loginCol == null)
                        throw new InvalidOperationException("No se encontró la columna LoginName en dbo.Users.");

                    // <-- NUEVO: detectar columna de posición/rol
                    string posCol = FindColQ(cn, "Users", "Position", "Rol", "Role", "Cargo", "Puesto");

                    string passCol = FindColQ(cn, "Users", "Contrasena", "Contraseña", "Password", "PasswordHash", "Clave");
                    string nomCol = FindColQ(cn, "Users", "Nombre", "FirstName");
                    string apeCol = FindColQ(cn, "Users", "Apellido", "LastName");
                    string mailCol = FindColQ(cn, "Users", "Correo", "Email", "CorreoElectronico");
                    string fotoCol = FindColQ(cn, "Users", "FotoPerfil", "Foto", "Imagen", "Avatar", "Picture");

                    if (passCol == null) passCol = "CAST(NULL AS NVARCHAR(200))";
                    if (nomCol == null) nomCol = "CAST(NULL AS NVARCHAR(200))";
                    if (apeCol == null) apeCol = "CAST(NULL AS NVARCHAR(200))";
                    if (mailCol == null) mailCol = "CAST(NULL AS NVARCHAR(200))";
                    if (fotoCol == null) fotoCol = "CAST(NULL AS VARBINARY(MAX))";
                    if (posCol == null) posCol = "CAST(NULL AS NVARCHAR(50))";

                    string sql =
                        "SELECT " + fotoCol + " AS FotoPerfil," +
                        idCol + " AS ID," +
                        loginCol + " AS Usuario," +
                        passCol + " AS Contrasena," +
                        nomCol + " AS Nombre," +
                        apeCol + " AS Apellido," +
                        mailCol + " AS Correo," +
                        posCol + " AS Position " +       
                        "FROM dbo.Users " +
                        "ORDER BY " + idCol + " ASC;";

                    var dtSrc = new DataTable();
                    using (var da = new SqlDataAdapter(sql, cn)) da.Fill(dtSrc);

                    var dtView = new DataTable();
                    dtView.Columns.Add("FotoPerfil", typeof(Image));
                    dtView.Columns.Add("ID", typeof(int));
                    dtView.Columns.Add("Usuario", typeof(string));
                    dtView.Columns.Add("Contrasena", typeof(string));
                    dtView.Columns.Add("Nombre", typeof(string));
                    dtView.Columns.Add("Apellido", typeof(string));
                    dtView.Columns.Add("Correo", typeof(string));
                    dtView.Columns.Add("Position", typeof(string)); 

                    foreach (DataRow r in dtSrc.Rows)
                    {
                        Image img = null;
                        if (!(r["FotoPerfil"] is DBNull))
                        {
                            var bytes = (byte[])r["FotoPerfil"];
                            if (bytes != null && bytes.Length > 0)
                            {
                                try { using (var ms = new MemoryStream(bytes)) img = Image.FromStream(ms); }
                                catch { img = null; }
                            }
                        }

                        int id = ToInt(r["ID"]);
                        string u = SafeStr(r["Usuario"]);
                        string p = SafeStr(r["Contrasena"]);
                        string n = SafeStr(r["Nombre"]);
                        string a = SafeStr(r["Apellido"]);
                        string c = SafeStr(r["Correo"]);
                        string rol = SafeStr(r["Position"]);

                        dtView.Rows.Add(img, id, u, p, n, a, c, rol);
                    }

                    dgv_Trabajadores_agg.DataSource = dtView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgv_Trabajadores_agg_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv_Trabajadores_agg.Rows[e.RowIndex];

            var data = new UserEditData
            {
                Id = ToInt(row.Cells["ID"].Value),
                Usuario = Convert.ToString(row.Cells["Usuario"].Value),
                Contrasena = Convert.ToString(row.Cells["Contrasena"].Value),
                Nombre = Convert.ToString(row.Cells["Nombre"].Value),
                Apellido = Convert.ToString(row.Cells["Apellido"].Value),
                Correo = Convert.ToString(row.Cells["Correo"].Value),
                Position = Convert.ToString(row.Cells["Position"].Value), 
                Foto = row.Cells["FotoPerfil"].Value as Image
            };

            UsuarioSeleccionado?.Invoke(data);
            this.Close();
        }

        private void btn_Eliminar_Usu_Click(object sender, EventArgs e)
        {
            if (dgv_Trabajadores_agg.CurrentRow == null) return;
            int id = ToInt(dgv_Trabajadores_agg.CurrentRow.Cells["ID"].Value);
            if (id <= 0) return;

            try
            {
                using (var cn = Conexion_SQL.OpenConnection())
                {
                    // 1) Bloqueo si tiene facturas
                    if (HasInvoices(cn, id))
                    {
                        var opcion = MessageBox.Show(
                            "Este usuario tiene facturas asociadas, por lo que no se puede eliminar.\n\n" +
                            "¿Deseas inactivarlo en su lugar?",
                            "No se puede eliminar",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (opcion == DialogResult.Yes)
                        {
                            if (InactivarUsuario(cn, id))
                            {
                                CargarUsuarios();
                                MessageBox.Show("Usuario inactivado correctamente.", "Usuarios",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No existe un campo de estado/activo para inactivar al usuario.\n" +
                                    "Contacta al administrador para configurar esta opción.",
                                    "Usuarios",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        return; // salimos: no se elimina
                    }

                    // 2) Confirmación de eliminación definitiva (sin facturas)
                    if (MessageBox.Show("¿Eliminar el usuario seleccionado?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    // 3) Eliminar
                    string idCol = FindColQ(cn, "Users", "UserID", "ID_Usuario", "ID", "UsuarioID");
                    if (idCol == null) throw new InvalidOperationException("No se encontró columna de ID en dbo.Users.");

                    using (var cmd = new SqlCommand("DELETE FROM dbo.Users WHERE " + idCol + "=@id", cn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int aff = cmd.ExecuteNonQuery();
                        if (aff > 0)
                        {
                            CargarUsuarios();
                            MessageBox.Show("Usuario eliminado.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 547) // violación de FK
            {
                // Por si el check previo falló o hay otras relaciones
                MessageBox.Show(
                    "No se puede eliminar porque el usuario tiene registros relacionados (p. ej., facturas).\n" +
                    "Inactívalo en lugar de eliminarlo.",
                    "Relación existente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static int ToInt(object v)
        {
            int n; return v == null || v == DBNull.Value ? 0 : (int.TryParse(Convert.ToString(v), out n) ? n : 0);
        }
        private static string SafeStr(object v)
        {
            return v == null || v == DBNull.Value ? "" : Convert.ToString(v);
        }

        private static string FindColQ(SqlConnection cn, string table, params string[] candidates)
        {
            string inList = string.Join("','", candidates);
            string sql = "SELECT TOP 1 '[' + name + ']' " +
                         "FROM sys.columns WHERE object_id = OBJECT_ID('dbo." + table + "') " +
                         "AND name IN ('" + inList + "');";
            using (var cmd = new SqlCommand(sql, cn))
            {
                var r = cmd.ExecuteScalar();
                return r == null ? null : r.ToString(); 
            }
        }
        private bool HasInvoices(SqlConnection cn, int userId)
        {
            string facIdCol = FindColQ(cn, "Facturas", "UserID", "UsuarioID", "TrabajadorID", "VendedorID", "CreadoPor", "RegistradoPor");
            if (facIdCol == null)
            {
                return false;
            }

            string sqlCount = $"SELECT COUNT(1) FROM dbo.Facturas WHERE {facIdCol} = @id";
            using (var cmd = new SqlCommand(sqlCount, cn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                int cnt = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                return cnt > 0;
            }
        }

        private bool InactivarUsuario(SqlConnection cn, int userId)
        {
            string idCol = FindColQ(cn, "Users", "UserID", "ID_Usuario", "ID", "UsuarioID");
            if (idCol == null) throw new InvalidOperationException("No se encontró columna de ID en dbo.Users.");

            string estadoCol = FindColQ(cn, "Users", "Estado");
            string activoCol = FindColQ(cn, "Users", "Activo", "Habilitado", "Enabled");

            if (estadoCol != null)
            {
                string sql = $"UPDATE dbo.Users SET {estadoCol} = @val WHERE {idCol} = @id";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@val", "Inactivo");
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            else if (activoCol != null)
            {
                string sql = $"UPDATE dbo.Users SET {activoCol} = @val WHERE {idCol} = @id";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@val", 0);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            return false; 
        }
    }
}
