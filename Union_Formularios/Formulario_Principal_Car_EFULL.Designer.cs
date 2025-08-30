namespace Formulario_Principal_Car_EFULL
{
    partial class Fr_Dashboard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fr_Dashboard));
            this.Panel_Dashboard = new System.Windows.Forms.Panel();
            this.btnConfig = new FontAwesome.Sharp.IconButton();
            this.iconButton4 = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imagen_Circular1 = new CustomControls.RJControls.Imagen_Circular();
            this.lblEmail = new System.Windows.Forms.Label();
            this.Nom_Usu = new System.Windows.Forms.Label();
            this.lblCargo = new System.Windows.Forms.Label();
            this.btnReportes = new FontAwesome.Sharp.IconButton();
            this.btnFacturas = new FontAwesome.Sharp.IconButton();
            this.btnPropietarios = new FontAwesome.Sharp.IconButton();
            this.btnVehiculos = new FontAwesome.Sharp.IconButton();
            this.btnMantenimiento = new FontAwesome.Sharp.IconButton();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.btnInicio = new System.Windows.Forms.PictureBox();
            this.Panel_Titulo = new System.Windows.Forms.Panel();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.lblInicio = new System.Windows.Forms.Label();
            this.Icono_Formulario_Principal = new FontAwesome.Sharp.IconPictureBox();
            this.Panel_Escritorio = new System.Windows.Forms.Panel();
            this.Horafecha = new System.Windows.Forms.Timer(this.components);
            this.Sombra_Panel = new System.Windows.Forms.Panel();
            this.Panel_Dashboard.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imagen_Circular1)).BeginInit();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnInicio)).BeginInit();
            this.Panel_Titulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icono_Formulario_Principal)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Dashboard
            // 
            this.Panel_Dashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(67)))), ((int)(((byte)(190)))));
            this.Panel_Dashboard.Controls.Add(this.btnConfig);
            this.Panel_Dashboard.Controls.Add(this.iconButton4);
            this.Panel_Dashboard.Controls.Add(this.panel1);
            this.Panel_Dashboard.Controls.Add(this.btnReportes);
            this.Panel_Dashboard.Controls.Add(this.btnFacturas);
            this.Panel_Dashboard.Controls.Add(this.btnPropietarios);
            this.Panel_Dashboard.Controls.Add(this.btnVehiculos);
            this.Panel_Dashboard.Controls.Add(this.btnMantenimiento);
            this.Panel_Dashboard.Controls.Add(this.panelLogo);
            this.Panel_Dashboard.Dock = System.Windows.Forms.DockStyle.Left;
            this.Panel_Dashboard.Location = new System.Drawing.Point(0, 0);
            this.Panel_Dashboard.Name = "Panel_Dashboard";
            this.Panel_Dashboard.Size = new System.Drawing.Size(268, 839);
            this.Panel_Dashboard.TabIndex = 0;
            // 
            // btnConfig
            // 
            this.btnConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.IconChar = FontAwesome.Sharp.IconChar.Gears;
            this.btnConfig.IconColor = System.Drawing.Color.Gainsboro;
            this.btnConfig.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnConfig.IconSize = 32;
            this.btnConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.Location = new System.Drawing.Point(0, 425);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnConfig.Size = new System.Drawing.Size(268, 60);
            this.btnConfig.TabIndex = 8;
            this.btnConfig.Text = "Configuración";
            this.btnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // iconButton4
            // 
            this.iconButton4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.iconButton4.FlatAppearance.BorderSize = 0;
            this.iconButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.iconButton4.ForeColor = System.Drawing.Color.White;
            this.iconButton4.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.iconButton4.IconColor = System.Drawing.Color.Gainsboro;
            this.iconButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton4.IconSize = 32;
            this.iconButton4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconButton4.Location = new System.Drawing.Point(0, 683);
            this.iconButton4.Name = "iconButton4";
            this.iconButton4.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.iconButton4.Size = new System.Drawing.Size(268, 60);
            this.iconButton4.TabIndex = 7;
            this.iconButton4.Text = "Cerrar Sesion";
            this.iconButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton4.UseVisualStyleBackColor = true;
            this.iconButton4.Click += new System.EventHandler(this.iconButton4_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(73)))), ((int)(((byte)(201)))));
            this.panel1.Controls.Add(this.imagen_Circular1);
            this.panel1.Controls.Add(this.lblEmail);
            this.panel1.Controls.Add(this.Nom_Usu);
            this.panel1.Controls.Add(this.lblCargo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 743);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 96);
            this.panel1.TabIndex = 1;
            // 
            // imagen_Circular1
            // 
            this.imagen_Circular1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imagen_Circular1.BorderCapStyle = System.Drawing.Drawing2D.DashCap.Flat;
            this.imagen_Circular1.BorderColor = System.Drawing.Color.White;
            this.imagen_Circular1.BorderColor2 = System.Drawing.Color.White;
            this.imagen_Circular1.BorderLineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.imagen_Circular1.BorderSize = 2;
            this.imagen_Circular1.GradientAngle = 50F;
            this.imagen_Circular1.Image = global::Union_Formularios.Properties.Resources.Usuario_Default;
            this.imagen_Circular1.Location = new System.Drawing.Point(12, 6);
            this.imagen_Circular1.Name = "imagen_Circular1";
            this.imagen_Circular1.Size = new System.Drawing.Size(77, 77);
            this.imagen_Circular1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imagen_Circular1.TabIndex = 0;
            this.imagen_Circular1.TabStop = false;
            // 
            // lblEmail
            // 
            this.lblEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblEmail.Location = new System.Drawing.Point(91, 62);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(42, 15);
            this.lblEmail.TabIndex = 12;
            this.lblEmail.Text = "Correo";
            // 
            // Nom_Usu
            // 
            this.Nom_Usu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Nom_Usu.AutoSize = true;
            this.Nom_Usu.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nom_Usu.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Nom_Usu.Location = new System.Drawing.Point(91, 14);
            this.Nom_Usu.Name = "Nom_Usu";
            this.Nom_Usu.Size = new System.Drawing.Size(68, 23);
            this.Nom_Usu.TabIndex = 9;
            this.Nom_Usu.Text = "Usuario";
            // 
            // lblCargo
            // 
            this.lblCargo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCargo.AutoSize = true;
            this.lblCargo.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCargo.ForeColor = System.Drawing.Color.Cyan;
            this.lblCargo.Location = new System.Drawing.Point(91, 39);
            this.lblCargo.Name = "lblCargo";
            this.lblCargo.Size = new System.Drawing.Size(50, 20);
            this.lblCargo.TabIndex = 11;
            this.lblCargo.Text = "Cargo";
            // 
            // btnReportes
            // 
            this.btnReportes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReportes.FlatAppearance.BorderSize = 0;
            this.btnReportes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportes.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnReportes.ForeColor = System.Drawing.Color.White;
            this.btnReportes.IconChar = FontAwesome.Sharp.IconChar.ChartLine;
            this.btnReportes.IconColor = System.Drawing.Color.Gainsboro;
            this.btnReportes.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnReportes.IconSize = 32;
            this.btnReportes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportes.Location = new System.Drawing.Point(0, 365);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnReportes.Size = new System.Drawing.Size(268, 60);
            this.btnReportes.TabIndex = 6;
            this.btnReportes.Text = "Reportes";
            this.btnReportes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReportes.UseVisualStyleBackColor = true;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // btnFacturas
            // 
            this.btnFacturas.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFacturas.FlatAppearance.BorderSize = 0;
            this.btnFacturas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFacturas.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnFacturas.ForeColor = System.Drawing.Color.White;
            this.btnFacturas.IconChar = FontAwesome.Sharp.IconChar.FileInvoiceDollar;
            this.btnFacturas.IconColor = System.Drawing.Color.Gainsboro;
            this.btnFacturas.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFacturas.IconSize = 32;
            this.btnFacturas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFacturas.Location = new System.Drawing.Point(0, 305);
            this.btnFacturas.Name = "btnFacturas";
            this.btnFacturas.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnFacturas.Size = new System.Drawing.Size(268, 60);
            this.btnFacturas.TabIndex = 5;
            this.btnFacturas.Text = " Facturación";
            this.btnFacturas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFacturas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFacturas.UseVisualStyleBackColor = true;
            this.btnFacturas.Click += new System.EventHandler(this.btnFacturas_Click);
            // 
            // btnPropietarios
            // 
            this.btnPropietarios.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPropietarios.FlatAppearance.BorderSize = 0;
            this.btnPropietarios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPropietarios.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnPropietarios.ForeColor = System.Drawing.Color.White;
            this.btnPropietarios.IconChar = FontAwesome.Sharp.IconChar.UserTie;
            this.btnPropietarios.IconColor = System.Drawing.Color.Gainsboro;
            this.btnPropietarios.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPropietarios.IconSize = 32;
            this.btnPropietarios.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPropietarios.Location = new System.Drawing.Point(0, 245);
            this.btnPropietarios.Name = "btnPropietarios";
            this.btnPropietarios.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnPropietarios.Size = new System.Drawing.Size(268, 60);
            this.btnPropietarios.TabIndex = 4;
            this.btnPropietarios.Text = "Gestión de Propietarios";
            this.btnPropietarios.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPropietarios.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPropietarios.UseVisualStyleBackColor = true;
            this.btnPropietarios.Click += new System.EventHandler(this.btnPropietarios_Click);
            // 
            // btnVehiculos
            // 
            this.btnVehiculos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnVehiculos.FlatAppearance.BorderSize = 0;
            this.btnVehiculos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVehiculos.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnVehiculos.ForeColor = System.Drawing.Color.White;
            this.btnVehiculos.IconChar = FontAwesome.Sharp.IconChar.CarSide;
            this.btnVehiculos.IconColor = System.Drawing.Color.Gainsboro;
            this.btnVehiculos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnVehiculos.IconSize = 32;
            this.btnVehiculos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVehiculos.Location = new System.Drawing.Point(0, 185);
            this.btnVehiculos.Name = "btnVehiculos";
            this.btnVehiculos.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnVehiculos.Size = new System.Drawing.Size(268, 60);
            this.btnVehiculos.TabIndex = 3;
            this.btnVehiculos.Text = "Gestión de Vehículos";
            this.btnVehiculos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVehiculos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnVehiculos.UseVisualStyleBackColor = true;
            this.btnVehiculos.Click += new System.EventHandler(this.btnVehiculos_Click);
            // 
            // btnMantenimiento
            // 
            this.btnMantenimiento.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMantenimiento.FlatAppearance.BorderSize = 0;
            this.btnMantenimiento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMantenimiento.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnMantenimiento.ForeColor = System.Drawing.Color.White;
            this.btnMantenimiento.IconChar = FontAwesome.Sharp.IconChar.Wrench;
            this.btnMantenimiento.IconColor = System.Drawing.Color.Gainsboro;
            this.btnMantenimiento.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMantenimiento.IconSize = 32;
            this.btnMantenimiento.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMantenimiento.Location = new System.Drawing.Point(0, 125);
            this.btnMantenimiento.Name = "btnMantenimiento";
            this.btnMantenimiento.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnMantenimiento.Size = new System.Drawing.Size(268, 60);
            this.btnMantenimiento.TabIndex = 2;
            this.btnMantenimiento.Text = "Gestión de Mantenimiento";
            this.btnMantenimiento.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMantenimiento.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMantenimiento.UseVisualStyleBackColor = true;
            this.btnMantenimiento.Click += new System.EventHandler(this.btnMantenimiento_Click);
            // 
            // panelLogo
            // 
            this.panelLogo.Controls.Add(this.btnInicio);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(268, 125);
            this.panelLogo.TabIndex = 0;
            // 
            // btnInicio
            // 
            this.btnInicio.Image = global::Union_Formularios.Properties.Resources.Leorium_logo2_1;
            this.btnInicio.Location = new System.Drawing.Point(43, 19);
            this.btnInicio.Name = "btnInicio";
            this.btnInicio.Size = new System.Drawing.Size(182, 87);
            this.btnInicio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnInicio.TabIndex = 0;
            this.btnInicio.TabStop = false;
            this.btnInicio.Click += new System.EventHandler(this.btnInicio_Click);
            // 
            // Panel_Titulo
            // 
            this.Panel_Titulo.BackColor = System.Drawing.Color.Gainsboro;
            this.Panel_Titulo.Controls.Add(this.iconButton3);
            this.Panel_Titulo.Controls.Add(this.iconButton1);
            this.Panel_Titulo.Controls.Add(this.lblInicio);
            this.Panel_Titulo.Controls.Add(this.Icono_Formulario_Principal);
            this.Panel_Titulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_Titulo.Location = new System.Drawing.Point(268, 0);
            this.Panel_Titulo.Name = "Panel_Titulo";
            this.Panel_Titulo.Size = new System.Drawing.Size(1228, 57);
            this.Panel_Titulo.TabIndex = 1;
            this.Panel_Titulo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_Titulo_MouseDown);
            // 
            // iconButton3
            // 
            this.iconButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(45)))), ((int)(((byte)(62)))));
            this.iconButton3.FlatAppearance.BorderSize = 0;
            this.iconButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton3.IconChar = FontAwesome.Sharp.IconChar.Minus;
            this.iconButton3.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton3.IconSize = 27;
            this.iconButton3.Location = new System.Drawing.Point(1165, 11);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(25, 27);
            this.iconButton3.TabIndex = 6;
            this.iconButton3.UseVisualStyleBackColor = true;
            this.iconButton3.Click += new System.EventHandler(this.iconButton3_Click);
            // 
            // iconButton1
            // 
            this.iconButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(145)))), ((int)(((byte)(68)))));
            this.iconButton1.FlatAppearance.BorderSize = 0;
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.SquareXmark;
            this.iconButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 27;
            this.iconButton1.Location = new System.Drawing.Point(1196, 11);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(25, 27);
            this.iconButton1.TabIndex = 4;
            this.iconButton1.UseVisualStyleBackColor = true;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // lblInicio
            // 
            this.lblInicio.AutoSize = true;
            this.lblInicio.Font = new System.Drawing.Font("Montserrat", 15F, System.Drawing.FontStyle.Bold);
            this.lblInicio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInicio.Location = new System.Drawing.Point(66, 13);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(72, 31);
            this.lblInicio.TabIndex = 3;
            this.lblInicio.Text = "Inicio";
            // 
            // Icono_Formulario_Principal
            // 
            this.Icono_Formulario_Principal.BackColor = System.Drawing.Color.Gainsboro;
            this.Icono_Formulario_Principal.ForeColor = System.Drawing.Color.Gray;
            this.Icono_Formulario_Principal.IconChar = FontAwesome.Sharp.IconChar.House;
            this.Icono_Formulario_Principal.IconColor = System.Drawing.Color.Gray;
            this.Icono_Formulario_Principal.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Icono_Formulario_Principal.Location = new System.Drawing.Point(28, 12);
            this.Icono_Formulario_Principal.Name = "Icono_Formulario_Principal";
            this.Icono_Formulario_Principal.Size = new System.Drawing.Size(32, 32);
            this.Icono_Formulario_Principal.TabIndex = 2;
            this.Icono_Formulario_Principal.TabStop = false;
            // 
            // Panel_Escritorio
            // 
            this.Panel_Escritorio.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Panel_Escritorio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Escritorio.Location = new System.Drawing.Point(268, 57);
            this.Panel_Escritorio.Name = "Panel_Escritorio";
            this.Panel_Escritorio.Size = new System.Drawing.Size(1228, 782);
            this.Panel_Escritorio.TabIndex = 3;
            // 
            // Horafecha
            // 
            this.Horafecha.Enabled = true;
            // 
            // Sombra_Panel
            // 
            this.Sombra_Panel.BackColor = System.Drawing.Color.LightGray;
            this.Sombra_Panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Sombra_Panel.Location = new System.Drawing.Point(268, 57);
            this.Sombra_Panel.Name = "Sombra_Panel";
            this.Sombra_Panel.Size = new System.Drawing.Size(1228, 9);
            this.Sombra_Panel.TabIndex = 2;
            // 
            // Fr_Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1496, 839);
            this.Controls.Add(this.Sombra_Panel);
            this.Controls.Add(this.Panel_Escritorio);
            this.Controls.Add(this.Panel_Titulo);
            this.Controls.Add(this.Panel_Dashboard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Fr_Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMainMenu";
            this.Load += new System.EventHandler(this.Fr_Dashboard_Load);
            this.Panel_Dashboard.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imagen_Circular1)).EndInit();
            this.panelLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnInicio)).EndInit();
            this.Panel_Titulo.ResumeLayout(false);
            this.Panel_Titulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icono_Formulario_Principal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Panel_Dashboard;
        private System.Windows.Forms.Panel panelLogo;
        private FontAwesome.Sharp.IconButton btnReportes;
        private FontAwesome.Sharp.IconButton btnFacturas;
        private FontAwesome.Sharp.IconButton btnPropietarios;
        private FontAwesome.Sharp.IconButton btnVehiculos;
        private FontAwesome.Sharp.IconButton btnMantenimiento;
        private System.Windows.Forms.PictureBox btnInicio;
        private System.Windows.Forms.Panel Panel_Titulo;
        private FontAwesome.Sharp.IconPictureBox Icono_Formulario_Principal;
        private System.Windows.Forms.Label lblInicio;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton iconButton4;
        private System.Windows.Forms.Label Nom_Usu;
        private FontAwesome.Sharp.IconButton btnConfig;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblCargo;
        private System.Windows.Forms.Panel Panel_Escritorio;
        private CustomControls.RJControls.Imagen_Circular imagen_Circular1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer Horafecha;
        private System.Windows.Forms.Panel Sombra_Panel;
    }
}

