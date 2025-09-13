using Datos_Acceso.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Union_Formularios.Formularios;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_Propietarios : Form
    {
        // ===== 1) CAMPOS Y PROPIEDADES =====
        // Tabla en memoria para cargar y (en Load) filtrar propietarios activos.
        private DataTable propietariosDT;

        // Referencia opcional al formulario de Vehículos (para comunicación cruzada si se requiere).
        public Formulario_Vehiculos formPadre { get; set; }

        // ===== 2) CONSTRUCTOR =====
        // Inicializa los componentes visuales del formulario.
        // (No altera lógica de carga ni de datos; eso ocurre en el evento Load).
        public Formulario_Propietarios()
        {
            InitializeComponent();
        }

        // ===== 3) CICLO DE VIDA =====
        // Evento Load: consulta la tabla Propietarios, carga en DataTable, aplica filtro Estado='Activo'
        // y bindea el DataGridView para mostrar solo activos al iniciar.
        private void Formulario_Propietarios_Load(object sender, EventArgs e)
        {
            propietariosDT = new DataTable();

            using (SqlConnection cn = Conexion_SQL.OpenConnection())
            {
                cn.Open();
                string query = "SELECT ID_Propietario, Cedula, Nombre, Apellido, Telefono, Correo, Direccion, Estado, FechaRegistro FROM Propietarios";
                SqlDataAdapter adapter = new SqlDataAdapter(query, cn);
                adapter.Fill(propietariosDT);
            }
            // Mostrar solo Activos mediante vista filtrada sobre el DataTable cargado.
            DataView vistaActivos = propietariosDT.DefaultView;
            vistaActivos.RowFilter = "Estado = 'Activo'";
            dgvPropietarios.DataSource = vistaActivos;
        }

        // ===== 4) HELPERS / ACCESO A DATOS =====
        // Cuenta referencias (vehículos y facturas) de un propietario antes de intentar eliminar.
        // Devuelve tupla con cantidades para tomar decisiones en la UI (eliminar, marcar inactivo, etc.).
        private (int vehiculos, int facturas) ContarReferenciasPropietario(int idPropietario)
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT 
	            Vehiculos = (SELECT COUNT(*) FROM Vehiculos WHERE ID_Propietario = @id),
	            Facturas  = (SELECT COUNT(*) FROM Facturas  WHERE ID_Propietario = @id);", cn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPropietario;
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                            return (Convert.ToInt32(rd["Vehiculos"]), Convert.ToInt32(rd["Facturas"]));
                    }
                }
            }
            return (0, 0);
        }

        // Marca propietario como Inactivo cuando existen dependencias (p. ej., facturas) y no se puede eliminar.
        // No cambia la vista actual; el llamador decide cuándo recargar la grilla.
        private void MarcarPropietarioInactivo(int idPropietario)
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Propietarios SET Estado = 'Inactivo' WHERE ID_Propietario = @id", cn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPropietario;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Recarga completa de la tabla Propietarios para refrescar el DataGridView.
        // (Aquí no se aplica filtro Estado='Activo'; el llamador muestra todo el resultado de la consulta).
        private void CargarPropietarios()
        {
            using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
            {
                cn.Open();
                string query = "SELECT * FROM Propietarios";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPropietarios.DataSource = dt;
            }
        }

        // ===== 5) HANDLERS DE UI =====
        // Doble clic en una fila de la grilla: abre el formulario de edición en modo "editar" (con ID).
        // Al cerrar el editor, recarga la grilla para reflejar cambios.
        private void dgvPropietarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int idPropietario = Convert.ToInt32(
                dgvPropietarios.Rows[e.RowIndex].Cells["ID_Propietario"].Value
            );

            // Abre directamente en modo edición (constructor con ID)
            var frmEditar = new Formulario_Add_Propietario(idPropietario);
            frmEditar.FormClosed += (s, args) => CargarPropietarios();

            frmEditar.ShowDialog(this);
        }

        // Botón Eliminar: valida selección, verifica dependencias (facturas/vehículos),
        // ofrece marcar como Inactivo si hay facturas y, si procede, elimina dentro de transacción
        // (desvincula vehículos primero cuando corresponda).
        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPropietarios.CurrentRow == null)
                {
                    MessageBox.Show("Por favor, seleccione un propietario para eliminar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                int idPropietario = Convert.ToInt32(dgvPropietarios.CurrentRow.Cells["ID_Propietario"].Value);
                string cedula = dgvPropietarios.CurrentRow.Cells["Cedula"].Value?.ToString() ?? "";
                string nombre = dgvPropietarios.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "(sin nombre)";

                // Cuenta referencias antes de intentar borrar
                var refs = ContarReferenciasPropietario(idPropietario);
                int vehiculos = refs.vehiculos;
                int facturas = refs.facturas;

                if (facturas > 0)
                {
                    // No borrar: hay facturas 
                    var resp = MessageBox.Show(
                        $"El propietario {nombre} ({cedula}) tiene {facturas} factura(s) asociada(s).\n" +
                        $"Por integridad, no se puede eliminar.\n\n¿Deseas marcarlo como INACTIVO?",
                        "Propietario con facturas", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        MarcarPropietarioInactivo(idPropietario);
                        CargarPropietarios();
                        MessageBox.Show("Se marcó el propietario como Inactivo.", "Listo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                // No hay facturas
                string msg = vehiculos > 0
                    ? $"El propietario tiene {vehiculos} vehículo(s) asociado(s).\n" +
                      $"Se DESVINCULARÁN (ID_Propietario = NULL) y luego se ELIMINARÁ el propietario.\n\n¿Confirmas?"
                    : $"¿Eliminar al propietario {nombre} ({cedula})?";

                if (MessageBox.Show(msg, "Confirmar eliminación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                using (SqlConnection cn = new ConexionSQL_Implementacion().AbrirConexion())
                {
                    cn.Open();
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            if (vehiculos > 0)
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "UPDATE Vehiculos SET ID_Propietario = NULL WHERE ID_Propietario = @id", cn, tx))
                                {
                                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPropietario;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                "DELETE FROM Propietarios WHERE ID_Propietario = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPropietario;
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw; // Se propaga al catch externo para mostrar mensaje
                        }
                    }
                }

                CargarPropietarios();
                MessageBox.Show("Propietario eliminado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex) when (ex.Number == 547) // Violación de FK
            {
                MessageBox.Show(
                    "No se pudo eliminar debido a registros relacionados (restricción de integridad). " +
                    "Puedes marcarlo como Inactivo.", "Restricción de integridad",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al eliminar el propietario:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Añadir: abre formulario de alta de propietario en modo nuevo (sin ID).
        // Al cerrarse, se recarga el DataGridView para reflejar el nuevo registro.
        private void btn_Añadir_Click(object sender, EventArgs e)
        {
            Formulario_Add_Propietario frmAdd = new Formulario_Add_Propietario();
            frmAdd.FormClosed += (s, args) => CargarPropietarios(); // Recargar al cerrar
            frmAdd.ShowDialog();
        }


        /// ////////////////////////////////////////////////////////////////////////////
        private Guna.UI2.WinForms.Guna2DataGridView dgvPropietarios;
        private Guna.UI2.WinForms.Guna2Button btn_Eliminar;
        private Guna.UI2.WinForms.Guna2Button btn_Añadir;
        private Label label1;

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvPropietarios = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Eliminar = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Añadir = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPropietarios)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPropietarios
            // 
            this.dgvPropietarios.AllowUserToAddRows = false;
            this.dgvPropietarios.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvPropietarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPropietarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPropietarios.ColumnHeadersHeight = 15;
            this.dgvPropietarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPropietarios.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPropietarios.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPropietarios.Location = new System.Drawing.Point(67, 109);
            this.dgvPropietarios.Name = "dgvPropietarios";
            this.dgvPropietarios.ReadOnly = true;
            this.dgvPropietarios.RowHeadersVisible = false;
            this.dgvPropietarios.Size = new System.Drawing.Size(1084, 491);
            this.dgvPropietarios.TabIndex = 43;
            this.dgvPropietarios.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvPropietarios.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvPropietarios.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvPropietarios.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvPropietarios.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvPropietarios.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvPropietarios.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPropietarios.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvPropietarios.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvPropietarios.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPropietarios.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvPropietarios.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvPropietarios.ThemeStyle.HeaderStyle.Height = 15;
            this.dgvPropietarios.ThemeStyle.ReadOnly = true;
            this.dgvPropietarios.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvPropietarios.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPropietarios.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPropietarios.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvPropietarios.ThemeStyle.RowsStyle.Height = 22;
            this.dgvPropietarios.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPropietarios.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvPropietarios.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPropietarios_CellDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 38);
            this.label1.TabIndex = 40;
            this.label1.Text = "Propietarios agregados";
            // 
            // btn_Eliminar
            // 
            this.btn_Eliminar.Animated = true;
            this.btn_Eliminar.BorderRadius = 8;
            this.btn_Eliminar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Eliminar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Eliminar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Eliminar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Eliminar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.btn_Eliminar.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Eliminar.ForeColor = System.Drawing.Color.White;
            this.btn_Eliminar.Image = global::Union_Formularios.Properties.Resources.icon_trash;
            this.btn_Eliminar.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Eliminar.ImageOffset = new System.Drawing.Point(5, -2);
            this.btn_Eliminar.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Eliminar.Location = new System.Drawing.Point(775, 626);
            this.btn_Eliminar.Name = "btn_Eliminar";
            this.btn_Eliminar.ShadowDecoration.BorderRadius = 14;
            this.btn_Eliminar.Size = new System.Drawing.Size(180, 45);
            this.btn_Eliminar.TabIndex = 42;
            this.btn_Eliminar.Text = "Eliminar";
            this.btn_Eliminar.Click += new System.EventHandler(this.btn_Eliminar_Click);
            // 
            // btn_Añadir
            // 
            this.btn_Añadir.Animated = true;
            this.btn_Añadir.BorderRadius = 8;
            this.btn_Añadir.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Añadir.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Añadir.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Añadir.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Añadir.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(208)))), ((int)(((byte)(117)))));
            this.btn_Añadir.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Añadir.ForeColor = System.Drawing.Color.White;
            this.btn_Añadir.Image = global::Union_Formularios.Properties.Resources.icon_Add_Usu;
            this.btn_Añadir.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Añadir.ImageOffset = new System.Drawing.Point(8, 0);
            this.btn_Añadir.ImageSize = new System.Drawing.Size(28, 28);
            this.btn_Añadir.Location = new System.Drawing.Point(971, 626);
            this.btn_Añadir.Name = "btn_Añadir";
            this.btn_Añadir.ShadowDecoration.BorderRadius = 14;
            this.btn_Añadir.Size = new System.Drawing.Size(180, 45);
            this.btn_Añadir.TabIndex = 41;
            this.btn_Añadir.Text = "Añadir";
            this.btn_Añadir.Click += new System.EventHandler(this.btn_Añadir_Click);
            // 
            // Formulario_Propietarios
            // 
            this.ClientSize = new System.Drawing.Size(1212, 753);
            this.Controls.Add(this.dgvPropietarios);
            this.Controls.Add(this.btn_Eliminar);
            this.Controls.Add(this.btn_Añadir);
            this.Controls.Add(this.label1);
            this.Name = "Formulario_Propietarios";
            this.Text = "Gestión de Propietarios";
            this.Load += new System.EventHandler(this.Formulario_Propietarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPropietarios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
