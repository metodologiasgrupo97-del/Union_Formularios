namespace Union_Formularios
{
    partial class InicioControl
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblHora = new System.Windows.Forms.Label();
            this.guna2GradientPanel3 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbl_Cont_Prop = new System.Windows.Forms.Label();
            this.lblTotalPropietarios = new System.Windows.Forms.Label();
            this.iconPictureBox3 = new FontAwesome.Sharp.IconPictureBox();
            this.guna2GradientPanel2 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbl_Cont_Vehi = new System.Windows.Forms.Label();
            this.lblTotalVehiculos = new System.Windows.Forms.Label();
            this.iconPictureBox2 = new FontAwesome.Sharp.IconPictureBox();
            this.guna2GradientPanel1 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lbl_Cont_Trab = new System.Windows.Forms.Label();
            this.lblTotalTrabajadores = new System.Windows.Forms.Label();
            this.iconPictureBox1 = new FontAwesome.Sharp.IconPictureBox();
            this.Horafecha = new System.Windows.Forms.Timer(this.components);
            this.dgvVehiculos = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2GradientPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox3)).BeginInit();
            this.guna2GradientPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).BeginInit();
            this.guna2GradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehiculos)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFecha
            // 
            this.lblFecha.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblFecha.AutoSize = true;
            this.lblFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.lblFecha.Location = new System.Drawing.Point(685, 448);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(0, 31);
            this.lblFecha.TabIndex = 11;
            // 
            // lblHora
            // 
            this.lblHora.AutoSize = true;
            this.lblHora.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHora.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblHora.Location = new System.Drawing.Point(771, 310);
            this.lblHora.Name = "lblHora";
            this.lblHora.Size = new System.Drawing.Size(0, 108);
            this.lblHora.TabIndex = 10;
            // 
            // guna2GradientPanel3
            // 
            this.guna2GradientPanel3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.guna2GradientPanel3.BorderRadius = 10;
            this.guna2GradientPanel3.Controls.Add(this.lbl_Cont_Prop);
            this.guna2GradientPanel3.Controls.Add(this.lblTotalPropietarios);
            this.guna2GradientPanel3.Controls.Add(this.iconPictureBox3);
            this.guna2GradientPanel3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(70)))), ((int)(((byte)(93)))));
            this.guna2GradientPanel3.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(43)))), ((int)(((byte)(112)))));
            this.guna2GradientPanel3.Location = new System.Drawing.Point(811, 38);
            this.guna2GradientPanel3.Name = "guna2GradientPanel3";
            this.guna2GradientPanel3.Size = new System.Drawing.Size(284, 149);
            this.guna2GradientPanel3.TabIndex = 9;
            // 
            // lbl_Cont_Prop
            // 
            this.lbl_Cont_Prop.AutoSize = true;
            this.lbl_Cont_Prop.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cont_Prop.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Cont_Prop.ForeColor = System.Drawing.Color.White;
            this.lbl_Cont_Prop.Location = new System.Drawing.Point(133, 27);
            this.lbl_Cont_Prop.Name = "lbl_Cont_Prop";
            this.lbl_Cont_Prop.Size = new System.Drawing.Size(147, 33);
            this.lbl_Cont_Prop.TabIndex = 12;
            this.lbl_Cont_Prop.Text = "Propietarios";
            // 
            // lblTotalPropietarios
            // 
            this.lblTotalPropietarios.AutoSize = true;
            this.lblTotalPropietarios.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalPropietarios.Font = new System.Drawing.Font("Montserrat", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPropietarios.ForeColor = System.Drawing.Color.White;
            this.lblTotalPropietarios.Location = new System.Drawing.Point(194, 51);
            this.lblTotalPropietarios.Name = "lblTotalPropietarios";
            this.lblTotalPropietarios.Size = new System.Drawing.Size(65, 75);
            this.lblTotalPropietarios.TabIndex = 11;
            this.lblTotalPropietarios.Text = "0";
            // 
            // iconPictureBox3
            // 
            this.iconPictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.iconPictureBox3.IconChar = FontAwesome.Sharp.IconChar.UserTie;
            this.iconPictureBox3.IconColor = System.Drawing.Color.White;
            this.iconPictureBox3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox3.IconSize = 99;
            this.iconPictureBox3.Location = new System.Drawing.Point(27, 27);
            this.iconPictureBox3.Name = "iconPictureBox3";
            this.iconPictureBox3.Size = new System.Drawing.Size(100, 99);
            this.iconPictureBox3.TabIndex = 2;
            this.iconPictureBox3.TabStop = false;
            // 
            // guna2GradientPanel2
            // 
            this.guna2GradientPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.guna2GradientPanel2.BorderRadius = 10;
            this.guna2GradientPanel2.Controls.Add(this.lbl_Cont_Vehi);
            this.guna2GradientPanel2.Controls.Add(this.lblTotalVehiculos);
            this.guna2GradientPanel2.Controls.Add(this.iconPictureBox2);
            this.guna2GradientPanel2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(49)))), ((int)(((byte)(205)))));
            this.guna2GradientPanel2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(74)))), ((int)(((byte)(225)))));
            this.guna2GradientPanel2.Location = new System.Drawing.Point(432, 38);
            this.guna2GradientPanel2.Name = "guna2GradientPanel2";
            this.guna2GradientPanel2.Size = new System.Drawing.Size(284, 149);
            this.guna2GradientPanel2.TabIndex = 8;
            // 
            // lbl_Cont_Vehi
            // 
            this.lbl_Cont_Vehi.AutoSize = true;
            this.lbl_Cont_Vehi.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cont_Vehi.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Cont_Vehi.ForeColor = System.Drawing.Color.White;
            this.lbl_Cont_Vehi.Location = new System.Drawing.Point(147, 27);
            this.lbl_Cont_Vehi.Name = "lbl_Cont_Vehi";
            this.lbl_Cont_Vehi.Size = new System.Drawing.Size(121, 33);
            this.lbl_Cont_Vehi.TabIndex = 10;
            this.lbl_Cont_Vehi.Text = "Vehículos";
            // 
            // lblTotalVehiculos
            // 
            this.lblTotalVehiculos.AutoSize = true;
            this.lblTotalVehiculos.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalVehiculos.Font = new System.Drawing.Font("Montserrat", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalVehiculos.ForeColor = System.Drawing.Color.White;
            this.lblTotalVehiculos.Location = new System.Drawing.Point(175, 51);
            this.lblTotalVehiculos.Name = "lblTotalVehiculos";
            this.lblTotalVehiculos.Size = new System.Drawing.Size(65, 75);
            this.lblTotalVehiculos.TabIndex = 9;
            this.lblTotalVehiculos.Text = "0";
            // 
            // iconPictureBox2
            // 
            this.iconPictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.iconPictureBox2.IconChar = FontAwesome.Sharp.IconChar.Car;
            this.iconPictureBox2.IconColor = System.Drawing.Color.White;
            this.iconPictureBox2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox2.IconSize = 99;
            this.iconPictureBox2.Location = new System.Drawing.Point(24, 27);
            this.iconPictureBox2.Name = "iconPictureBox2";
            this.iconPictureBox2.Size = new System.Drawing.Size(100, 99);
            this.iconPictureBox2.TabIndex = 1;
            this.iconPictureBox2.TabStop = false;
            // 
            // guna2GradientPanel1
            // 
            this.guna2GradientPanel1.BorderRadius = 10;
            this.guna2GradientPanel1.Controls.Add(this.lbl_Cont_Trab);
            this.guna2GradientPanel1.Controls.Add(this.lblTotalTrabajadores);
            this.guna2GradientPanel1.Controls.Add(this.iconPictureBox1);
            this.guna2GradientPanel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(157)))), ((int)(((byte)(90)))));
            this.guna2GradientPanel1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(111)))), ((int)(((byte)(125)))));
            this.guna2GradientPanel1.Location = new System.Drawing.Point(52, 38);
            this.guna2GradientPanel1.Name = "guna2GradientPanel1";
            this.guna2GradientPanel1.ShadowDecoration.BorderRadius = 14;
            this.guna2GradientPanel1.ShadowDecoration.Color = System.Drawing.Color.Gray;
            this.guna2GradientPanel1.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.guna2GradientPanel1.Size = new System.Drawing.Size(284, 149);
            this.guna2GradientPanel1.TabIndex = 7;
            // 
            // lbl_Cont_Trab
            // 
            this.lbl_Cont_Trab.AutoSize = true;
            this.lbl_Cont_Trab.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cont_Trab.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Cont_Trab.ForeColor = System.Drawing.Color.White;
            this.lbl_Cont_Trab.Location = new System.Drawing.Point(119, 22);
            this.lbl_Cont_Trab.Name = "lbl_Cont_Trab";
            this.lbl_Cont_Trab.Size = new System.Drawing.Size(157, 33);
            this.lbl_Cont_Trab.TabIndex = 8;
            this.lbl_Cont_Trab.Text = "Trabajadores";
            // 
            // lblTotalTrabajadores
            // 
            this.lblTotalTrabajadores.AutoSize = true;
            this.lblTotalTrabajadores.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalTrabajadores.Font = new System.Drawing.Font("Montserrat", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTrabajadores.ForeColor = System.Drawing.Color.White;
            this.lblTotalTrabajadores.Location = new System.Drawing.Point(156, 51);
            this.lblTotalTrabajadores.Name = "lblTotalTrabajadores";
            this.lblTotalTrabajadores.Size = new System.Drawing.Size(65, 75);
            this.lblTotalTrabajadores.TabIndex = 7;
            this.lblTotalTrabajadores.Text = "0";
            // 
            // iconPictureBox1
            // 
            this.iconPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.iconPictureBox1.IconChar = FontAwesome.Sharp.IconChar.User;
            this.iconPictureBox1.IconColor = System.Drawing.Color.White;
            this.iconPictureBox1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox1.IconSize = 99;
            this.iconPictureBox1.Location = new System.Drawing.Point(14, 27);
            this.iconPictureBox1.Name = "iconPictureBox1";
            this.iconPictureBox1.Size = new System.Drawing.Size(100, 99);
            this.iconPictureBox1.TabIndex = 0;
            this.iconPictureBox1.TabStop = false;
            // 
            // Horafecha
            // 
            this.Horafecha.Enabled = true;
            this.Horafecha.Tick += new System.EventHandler(this.Horafecha_Tick_1);
            // 
            // dgvVehiculos
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            this.dgvVehiculos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVehiculos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvVehiculos.ColumnHeadersHeight = 15;
            this.dgvVehiculos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVehiculos.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvVehiculos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVehiculos.Location = new System.Drawing.Point(43, 238);
            this.dgvVehiculos.Name = "dgvVehiculos";
            this.dgvVehiculos.RowHeadersVisible = false;
            this.dgvVehiculos.Size = new System.Drawing.Size(611, 443);
            this.dgvVehiculos.TabIndex = 46;
            this.dgvVehiculos.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVehiculos.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvVehiculos.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvVehiculos.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvVehiculos.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvVehiculos.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvVehiculos.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVehiculos.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvVehiculos.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvVehiculos.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVehiculos.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvVehiculos.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvVehiculos.ThemeStyle.HeaderStyle.Height = 15;
            this.dgvVehiculos.ThemeStyle.ReadOnly = false;
            this.dgvVehiculos.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVehiculos.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvVehiculos.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVehiculos.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvVehiculos.ThemeStyle.RowsStyle.Height = 22;
            this.dgvVehiculos.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvVehiculos.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvVehiculos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVehiculos_CellContentClick);
            // 
            // InicioControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvVehiculos);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.lblHora);
            this.Controls.Add(this.guna2GradientPanel3);
            this.Controls.Add(this.guna2GradientPanel2);
            this.Controls.Add(this.guna2GradientPanel1);
            this.Name = "InicioControl";
            this.Size = new System.Drawing.Size(1138, 719);
            this.guna2GradientPanel3.ResumeLayout(false);
            this.guna2GradientPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox3)).EndInit();
            this.guna2GradientPanel2.ResumeLayout(false);
            this.guna2GradientPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox2)).EndInit();
            this.guna2GradientPanel1.ResumeLayout(false);
            this.guna2GradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehiculos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblHora;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel3;
        private System.Windows.Forms.Label lbl_Cont_Prop;
        private System.Windows.Forms.Label lblTotalPropietarios;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox3;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel2;
        private System.Windows.Forms.Label lbl_Cont_Vehi;
        private System.Windows.Forms.Label lblTotalVehiculos;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox2;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel1;
        private System.Windows.Forms.Label lbl_Cont_Trab;
        private System.Windows.Forms.Label lblTotalTrabajadores;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox1;
        private System.Windows.Forms.Timer Horafecha;
        private Guna.UI2.WinForms.Guna2DataGridView dgvVehiculos;
    }
}
