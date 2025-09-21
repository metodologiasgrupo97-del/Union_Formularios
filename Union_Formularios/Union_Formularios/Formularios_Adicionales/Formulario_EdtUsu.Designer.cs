namespace Formulario_Principal_Car_EFULL.Formularios
{
    partial class Formulario_EdtUsu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Eliminar_Usu = new Guna.UI2.WinForms.Guna2Button();
            this.dgv_Trabajadores_agg = new Guna.UI2.WinForms.Guna2DataGridView();
            this.Ft_Perfil = new System.Windows.Forms.DataGridViewImageColumn();
            this.UserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LoginName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Password = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Trabajadores_agg)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 38);
            this.label1.TabIndex = 5;
            this.label1.Text = "Trabajadores agregados";
            // 
            // btn_Eliminar_Usu
            // 
            this.btn_Eliminar_Usu.Animated = true;
            this.btn_Eliminar_Usu.BorderRadius = 8;
            this.btn_Eliminar_Usu.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Eliminar_Usu.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Eliminar_Usu.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Eliminar_Usu.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Eliminar_Usu.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.btn_Eliminar_Usu.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Eliminar_Usu.ForeColor = System.Drawing.Color.White;
            this.btn_Eliminar_Usu.Image = global::Union_Formularios.Properties.Resources.icon_trash;
            this.btn_Eliminar_Usu.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Eliminar_Usu.ImageOffset = new System.Drawing.Point(5, -2);
            this.btn_Eliminar_Usu.ImageSize = new System.Drawing.Size(25, 25);
            this.btn_Eliminar_Usu.Location = new System.Drawing.Point(861, 549);
            this.btn_Eliminar_Usu.Name = "btn_Eliminar_Usu";
            this.btn_Eliminar_Usu.ShadowDecoration.BorderRadius = 14;
            this.btn_Eliminar_Usu.Size = new System.Drawing.Size(180, 45);
            this.btn_Eliminar_Usu.TabIndex = 38;
            this.btn_Eliminar_Usu.Text = "Eliminar";
            // 
            // dgv_Trabajadores_agg
            // 
            this.dgv_Trabajadores_agg.AllowUserToAddRows = false;
            this.dgv_Trabajadores_agg.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgv_Trabajadores_agg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Trabajadores_agg.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_Trabajadores_agg.ColumnHeadersHeight = 15;
            this.dgv_Trabajadores_agg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgv_Trabajadores_agg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Ft_Perfil,
            this.UserID,
            this.LoginName,
            this.Password,
            this.FirstName,
            this.LastName,
            this.Email});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Trabajadores_agg.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgv_Trabajadores_agg.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgv_Trabajadores_agg.Location = new System.Drawing.Point(36, 71);
            this.dgv_Trabajadores_agg.Name = "dgv_Trabajadores_agg";
            this.dgv_Trabajadores_agg.ReadOnly = true;
            this.dgv_Trabajadores_agg.RowHeadersVisible = false;
            this.dgv_Trabajadores_agg.Size = new System.Drawing.Size(1005, 454);
            this.dgv_Trabajadores_agg.TabIndex = 39;
            this.dgv_Trabajadores_agg.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgv_Trabajadores_agg.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgv_Trabajadores_agg.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgv_Trabajadores_agg.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgv_Trabajadores_agg.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgv_Trabajadores_agg.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgv_Trabajadores_agg.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgv_Trabajadores_agg.ThemeStyle.HeaderStyle.Height = 15;
            this.dgv_Trabajadores_agg.ThemeStyle.ReadOnly = true;
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.Height = 22;
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgv_Trabajadores_agg.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgv_Trabajadores_agg.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Trabajadores_agg_CellDoubleClick);
            // 
            // Ft_Perfil
            // 
            this.Ft_Perfil.HeaderText = "Foto de perfil";
            this.Ft_Perfil.Name = "Ft_Perfil";
            this.Ft_Perfil.ReadOnly = true;
            // 
            // UserID
            // 
            this.UserID.HeaderText = "ID";
            this.UserID.Name = "UserID";
            this.UserID.ReadOnly = true;
            // 
            // LoginName
            // 
            this.LoginName.HeaderText = "Nombre de Usuario";
            this.LoginName.Name = "LoginName";
            this.LoginName.ReadOnly = true;
            // 
            // Password
            // 
            this.Password.HeaderText = "Contraseña";
            this.Password.Name = "Password";
            this.Password.ReadOnly = true;
            // 
            // FirstName
            // 
            this.FirstName.HeaderText = "Nombre";
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            // 
            // LastName
            // 
            this.LastName.HeaderText = "Apellido";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            // 
            // Email
            // 
            this.Email.HeaderText = "Correo";
            this.Email.Name = "Email";
            this.Email.ReadOnly = true;
            // 
            // Formulario_EdtUsu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 619);
            this.Controls.Add(this.dgv_Trabajadores_agg);
            this.Controls.Add(this.btn_Eliminar_Usu);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Formulario_EdtUsu";
            this.ShowIcon = false;
            this.Text = "Editar Trabajador";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Trabajadores_agg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button btn_Eliminar_Usu;
        private Guna.UI2.WinForms.Guna2DataGridView dgv_Trabajadores_agg;
        private System.Windows.Forms.DataGridViewImageColumn Ft_Perfil;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn LoginName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Password;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Email;
    }
}