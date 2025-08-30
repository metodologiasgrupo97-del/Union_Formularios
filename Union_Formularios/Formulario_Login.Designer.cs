namespace Union_Formularios
{
    partial class Formulario_Login
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Formulario_Login));
            this.Panel_Iniciar_sesion = new System.Windows.Forms.Panel();
            this.txtContraI = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lbl_Mensaje_de_Error_Login = new System.Windows.Forms.Label();
            this.Simbolo_Error = new FontAwesome.Sharp.IconPictureBox();
            this.Mostrar_Cont = new FontAwesome.Sharp.IconButton();
            this.Ocultar_Cont = new FontAwesome.Sharp.IconButton();
            this.button2 = new System.Windows.Forms.Button();
            this.pContraI = new System.Windows.Forms.Panel();
            this.pUsuarioI = new System.Windows.Forms.Panel();
            this.txtUsuI = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Panel_Iniciar_sesion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Simbolo_Error)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Iniciar_sesion
            // 
            this.Panel_Iniciar_sesion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(72)))), ((int)(((byte)(186)))));
            this.Panel_Iniciar_sesion.Controls.Add(this.txtContraI);
            this.Panel_Iniciar_sesion.Controls.Add(this.label1);
            this.Panel_Iniciar_sesion.Controls.Add(this.pictureBox1);
            this.Panel_Iniciar_sesion.Controls.Add(this.linkLabel1);
            this.Panel_Iniciar_sesion.Controls.Add(this.lbl_Mensaje_de_Error_Login);
            this.Panel_Iniciar_sesion.Controls.Add(this.Simbolo_Error);
            this.Panel_Iniciar_sesion.Controls.Add(this.Mostrar_Cont);
            this.Panel_Iniciar_sesion.Controls.Add(this.Ocultar_Cont);
            this.Panel_Iniciar_sesion.Controls.Add(this.button2);
            this.Panel_Iniciar_sesion.Controls.Add(this.pContraI);
            this.Panel_Iniciar_sesion.Controls.Add(this.pUsuarioI);
            this.Panel_Iniciar_sesion.Controls.Add(this.txtUsuI);
            this.Panel_Iniciar_sesion.Controls.Add(this.label12);
            this.Panel_Iniciar_sesion.Controls.Add(this.label13);
            this.Panel_Iniciar_sesion.Controls.Add(this.label16);
            this.Panel_Iniciar_sesion.Controls.Add(this.pictureBox2);
            this.Panel_Iniciar_sesion.Location = new System.Drawing.Point(63, 27);
            this.Panel_Iniciar_sesion.Name = "Panel_Iniciar_sesion";
            this.Panel_Iniciar_sesion.Size = new System.Drawing.Size(403, 676);
            this.Panel_Iniciar_sesion.TabIndex = 12;
            this.Panel_Iniciar_sesion.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel_Iniciar_sesion_Paint);
            this.Panel_Iniciar_sesion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_Principal_MouseDown);
            // 
            // txtContraI
            // 
            this.txtContraI.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtContraI.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContraI.ForeColor = System.Drawing.Color.Gray;
            this.txtContraI.Location = new System.Drawing.Point(25, 401);
            this.txtContraI.Name = "txtContraI";
            this.txtContraI.Size = new System.Drawing.Size(329, 25);
            this.txtContraI.TabIndex = 20;
            this.txtContraI.Tag = "ContraI";
            this.txtContraI.Text = "Ingrese la contraseña";
            this.txtContraI.TextChanged += new System.EventHandler(this.txtContraI_TextChanged);
            this.txtContraI.Enter += new System.EventHandler(this.txtEnter2);
            this.txtContraI.Leave += new System.EventHandler(this.txtLeave2);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(72)))), ((int)(((byte)(186)))));
            this.label1.Font = new System.Drawing.Font("Montserrat", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(115, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 42);
            this.label1.TabIndex = 19;
            this.label1.Text = "Bienvenido";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Union_Formularios.Properties.Resources.Leorium_2_0;
            this.pictureBox1.Location = new System.Drawing.Point(135, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(132, 108);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.White;
            this.linkLabel1.LinkColor = System.Drawing.Color.Black;
            this.linkLabel1.Location = new System.Drawing.Point(134, 628);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(146, 13);
            this.linkLabel1.TabIndex = 16;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "¿Ha olvidado su contraseña?";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lbl_Mensaje_de_Error_Login
            // 
            this.lbl_Mensaje_de_Error_Login.AutoSize = true;
            this.lbl_Mensaje_de_Error_Login.BackColor = System.Drawing.Color.White;
            this.lbl_Mensaje_de_Error_Login.Location = new System.Drawing.Point(50, 450);
            this.lbl_Mensaje_de_Error_Login.Name = "lbl_Mensaje_de_Error_Login";
            this.lbl_Mensaje_de_Error_Login.Size = new System.Drawing.Size(87, 13);
            this.lbl_Mensaje_de_Error_Login.TabIndex = 14;
            this.lbl_Mensaje_de_Error_Login.Text = "Mensaje de Error";
            this.lbl_Mensaje_de_Error_Login.Visible = false;
            this.lbl_Mensaje_de_Error_Login.Click += new System.EventHandler(this.lbl_Mensaje_de_Error_Login_Click);
            // 
            // Simbolo_Error
            // 
            this.Simbolo_Error.BackColor = System.Drawing.Color.White;
            this.Simbolo_Error.ForeColor = System.Drawing.Color.Red;
            this.Simbolo_Error.IconChar = FontAwesome.Sharp.IconChar.CircleExclamation;
            this.Simbolo_Error.IconColor = System.Drawing.Color.Red;
            this.Simbolo_Error.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Simbolo_Error.Location = new System.Drawing.Point(22, 442);
            this.Simbolo_Error.Name = "Simbolo_Error";
            this.Simbolo_Error.Size = new System.Drawing.Size(32, 32);
            this.Simbolo_Error.TabIndex = 15;
            this.Simbolo_Error.TabStop = false;
            this.Simbolo_Error.Visible = false;
            // 
            // Mostrar_Cont
            // 
            this.Mostrar_Cont.BackColor = System.Drawing.Color.White;
            this.Mostrar_Cont.FlatAppearance.BorderSize = 0;
            this.Mostrar_Cont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Mostrar_Cont.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mostrar_Cont.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.Mostrar_Cont.IconColor = System.Drawing.Color.Black;
            this.Mostrar_Cont.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Mostrar_Cont.IconSize = 32;
            this.Mostrar_Cont.Location = new System.Drawing.Point(353, 403);
            this.Mostrar_Cont.Name = "Mostrar_Cont";
            this.Mostrar_Cont.Size = new System.Drawing.Size(31, 26);
            this.Mostrar_Cont.TabIndex = 12;
            this.Mostrar_Cont.UseVisualStyleBackColor = false;
            this.Mostrar_Cont.Click += new System.EventHandler(this.iconButton1_Click);
            this.Mostrar_Cont.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mostrar_Cont_MouseClick);
            // 
            // Ocultar_Cont
            // 
            this.Ocultar_Cont.BackColor = System.Drawing.Color.White;
            this.Ocultar_Cont.FlatAppearance.BorderSize = 0;
            this.Ocultar_Cont.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ocultar_Cont.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ocultar_Cont.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            this.Ocultar_Cont.IconColor = System.Drawing.Color.Black;
            this.Ocultar_Cont.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Ocultar_Cont.IconSize = 32;
            this.Ocultar_Cont.Location = new System.Drawing.Point(353, 403);
            this.Ocultar_Cont.Name = "Ocultar_Cont";
            this.Ocultar_Cont.Size = new System.Drawing.Size(31, 26);
            this.Ocultar_Cont.TabIndex = 13;
            this.Ocultar_Cont.UseVisualStyleBackColor = false;
            this.Ocultar_Cont.Click += new System.EventHandler(this.iconButton1_Click_1);
            this.Ocultar_Cont.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Ocultar_Cont_MouseClick);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(72)))), ((int)(((byte)(186)))));
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DimGray;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(25, 498);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(359, 41);
            this.button2.TabIndex = 11;
            this.button2.Text = "Acceder";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pContraI
            // 
            this.pContraI.BackColor = System.Drawing.Color.Silver;
            this.pContraI.Location = new System.Drawing.Point(27, 426);
            this.pContraI.Name = "pContraI";
            this.pContraI.Size = new System.Drawing.Size(329, 3);
            this.pContraI.TabIndex = 10;
            // 
            // pUsuarioI
            // 
            this.pUsuarioI.BackColor = System.Drawing.Color.Silver;
            this.pUsuarioI.Location = new System.Drawing.Point(22, 369);
            this.pUsuarioI.Name = "pUsuarioI";
            this.pUsuarioI.Size = new System.Drawing.Size(359, 3);
            this.pUsuarioI.TabIndex = 10;
            // 
            // txtUsuI
            // 
            this.txtUsuI.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsuI.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuI.ForeColor = System.Drawing.Color.Gray;
            this.txtUsuI.Location = new System.Drawing.Point(25, 343);
            this.txtUsuI.Name = "txtUsuI";
            this.txtUsuI.Size = new System.Drawing.Size(359, 25);
            this.txtUsuI.TabIndex = 6;
            this.txtUsuI.Tag = "UsuarioI";
            this.txtUsuI.Text = "Ingrese el usuario";
            this.txtUsuI.TextChanged += new System.EventHandler(this.txtUsuI_TextChanged);
            this.txtUsuI.Enter += new System.EventHandler(this.txtEnter);
            this.txtUsuI.Leave += new System.EventHandler(this.txtLeave);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(22, 382);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 18);
            this.label12.TabIndex = 2;
            this.label12.Text = "Contraseña: ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(22, 322);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 18);
            this.label13.TabIndex = 2;
            this.label13.Text = "Usuario:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.White;
            this.label16.Font = new System.Drawing.Font("Montserrat", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(99, 263);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(208, 42);
            this.label16.TabIndex = 1;
            this.label16.Text = "Iniciar Sesión";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Union_Formularios.Properties.Resources.Ola_para_login;
            this.pictureBox2.Location = new System.Drawing.Point(-111, 144);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(514, 631);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 18;
            this.pictureBox2.TabStop = false;
            // 
            // Formulario_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(548, 729);
            this.Controls.Add(this.Panel_Iniciar_sesion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Formulario_Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Formulario_Login";
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_Principal_MouseDown);
            this.Panel_Iniciar_sesion.ResumeLayout(false);
            this.Panel_Iniciar_sesion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Simbolo_Error)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Panel_Iniciar_sesion;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pContraI;
        private System.Windows.Forms.Panel pUsuarioI;
        private System.Windows.Forms.TextBox txtUsuI;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private FontAwesome.Sharp.IconButton Mostrar_Cont;
        private FontAwesome.Sharp.IconButton Ocultar_Cont;
        private FontAwesome.Sharp.IconPictureBox Simbolo_Error;
        private System.Windows.Forms.Label lbl_Mensaje_de_Error_Login;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtContraI;
    }
}

