using Formulario_Principal_Car_EFULL.Formularios;
using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Union_Formularios.Formularios
{
    public partial class Formulario_EdtDash : Form
    {
        private Form Sub_Formualrio;
        private Panel panelEscritorio;
        private bool _eventsWired;

        private Color _colorPrimario = Color.FromArgb(0, 122, 204);
        private Color _colorSecundario = Color.FromArgb(52, 58, 64);
        private Image _logoImg = null;
        private string _logoPath = null;
        private string _logoMime = null;

        public Formulario_EdtDash(Panel panelContenedor)
        {
            InitializeComponent();
            this.panelEscritorio = panelContenedor;
            WireNavButtonsOnce();
            WireDashboardHandlers();
        }

        public Formulario_EdtDash()
        {
            InitializeComponent();
            WireNavButtonsOnce();
            WireDashboardHandlers();
        }

        // ------------------- Navegación (tus handlers) -------------------
        private void WireNavButtonsOnce()
        {
            if (_eventsWired) return;
            HookClickByName("edt_perfil_dash", edt_perfil_dash_Click);
            HookClickByName("add_trabajador_dash", add_trabajador_dash_Click);
            HookClickByName("edt_configproducts_dash", edt_configproducts_dash_Click);
            _eventsWired = true;
        }

        private void HookClickByName(string controlName, EventHandler handler)
        {
            var ctrl = this.Controls.Find(controlName, true).FirstOrDefault();
            if (ctrl == null) return;
            ctrl.Click -= handler;
            ctrl.Click += handler;
        }

        private void OpenOrEmbed(Form form)
        {
            if (panelEscritorio == null || panelEscritorio.IsDisposed)
            {
                form.Dispose();
                return;
            }

            var existente = panelEscritorio.Controls
                .OfType<Form>()
                .FirstOrDefault(f => f.GetType() == form.GetType());

            if (existente != null)
            {
                existente.BringToFront();
                existente.Show();
                existente.Focus();
                form.Dispose();
                return;
            }

            if (Sub_Formualrio != null)
                Sub_Formualrio.Close();

            Sub_Formualrio = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelEscritorio.Controls.Clear();
            panelEscritorio.Controls.Add(form);
            panelEscritorio.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void edt_perfil_dash_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Configuracion(panelEscritorio));
        }

        private void add_trabajador_dash_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_Add_Users(panelEscritorio));
        }
        private void edt_configproducts_dash_Click(object sender, EventArgs e)
        {
            OpenOrEmbed(new Formulario_EdtDash(panelEscritorio));
        }

        // ------------------- Validaciones ya existentes -------------------
        private void guna2TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txt_RUC_Config_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // ------------------- UI + Carga inicial -------------------
        private void WireDashboardHandlers()
        {
            if (pcb_Logo_Conf != null) pcb_Logo_Conf.SizeMode = PictureBoxSizeMode.Zoom;
            if (Preview_Logo != null) Preview_Logo.SizeMode = PictureBoxSizeMode.Zoom;

            // Colores iniciales en botones (FillColor si es Guna2)
            SetBtnColor(btn_Color_Primario, _colorPrimario);
            SetBtnColor(btn_Color_Secundario, _colorSecundario);

            if (btn_Color_Primario != null) btn_Color_Primario.Click += Btn_Color_Primario_Click;
            if (btn_Color_Secundario != null) btn_Color_Secundario.Click += Btn_Color_Secundario_Click;
            if (btn_Examinar_Logo != null) btn_Examinar_Logo.Click += Btn_Examinar_Logo_Click;
            if (btn_Quitarlogo != null) btn_Quitarlogo.Click += Btn_Quitarlogo_Click;
            if (btn_Restablecer != null) btn_Restablecer.Click += Btn_Restablecer_Click;
            if (btn_Vistaprevia != null) btn_Vistaprevia.Click += (s, e) => ApplyPreview();
            if (btn_Guardar_Dashboard != null) btn_Guardar_Dashboard.Click += Btn_Guardar_Dashboard_Click;
            if (txt_color_primario != null) txt_color_primario.KeyDown += TxtColorPrimario_KeyDown;
            if (txt_color_secundario != null) txt_color_secundario.KeyDown += TxtColorSecundario_KeyDown;

            LoadEmpresaConfig();
            EmpresaConfigService.TemaCambiado += () => LoadEmpresaConfig();
        }

        private void SetBtnColor(Control btn, Color c)
        {
            if (btn == null) return;
            if (btn is Guna2Button gbtn) gbtn.FillColor = c;
            else btn.BackColor = c;
        }

        // ------------------- Selectores de color -------------------
        private void Btn_Color_Primario_Click(object sender, EventArgs e)
        {
            using (var cd = new ColorDialog() { AllowFullOpen = true, FullOpen = true, Color = _colorPrimario })
            {
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    _colorPrimario = cd.Color;
                    SetBtnColor(btn_Color_Primario, cd.Color);
                }
            }
        }

        private void Btn_Color_Secundario_Click(object sender, EventArgs e)
        {
            using (var cd = new ColorDialog() { AllowFullOpen = true, FullOpen = true, Color = _colorSecundario })
            {
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    _colorSecundario = cd.Color;
                    SetBtnColor(btn_Color_Secundario, cd.Color);
                }
            }
        }

        // ------------------- Logo: examinar / quitar -------------------
        private void Btn_Examinar_Logo_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()
            {
                Title = "Seleccionar logo",
                Filter = "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = false
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _logoPath = ofd.FileName;
                        _logoMime = DetectMimeFromExtension(Path.GetExtension(_logoPath));
                        using (var fs = new FileStream(_logoPath, FileMode.Open, FileAccess.Read))
                            _logoImg = Image.FromStream(fs);

                        pcb_Logo_Conf.Image = (Image)_logoImg.Clone();
                        lbl_nom_img.Text = TruncarNombreArchivo(Path.GetFileName(_logoPath), 15);
                        ApplyPreview();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo cargar la imagen.\n" + ex.Message,
                            "Logo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Btn_Quitarlogo_Click(object sender, EventArgs e)
        {
            _logoImg = null;
            _logoPath = null;
            _logoMime = null;

            if (pcb_Logo_Conf != null) pcb_Logo_Conf.Image = null;
            if (Preview_Logo != null) Preview_Logo.Image = null;
            if (lbl_nom_img != null) lbl_nom_img.Text = "…………………";

            ApplyPreview();
        }

        // ------------------- Restablecer -------------------
        private void Btn_Restablecer_Click(object sender, EventArgs e)
        {
            _colorPrimario = Color.FromArgb(0, 122, 204);
            _colorSecundario = Color.FromArgb(52, 58, 64);
            SetBtnColor(btn_Color_Primario, _colorPrimario);
            SetBtnColor(btn_Color_Secundario, _colorSecundario);

            _logoImg = null; _logoPath = null; _logoMime = null;
            if (pcb_Logo_Conf != null) pcb_Logo_Conf.Image = null;
            if (Preview_Logo != null) Preview_Logo.Image = null;
            if (lbl_nom_img != null) lbl_nom_img.Text = "…………………";

            txt_Nom_Comercial?.Clear();
            txt_Razon_Social?.Clear();
            txt_RUC_Config?.Clear();
            txt_telf_Config?.Clear();

            ApplyPreview();
        }

        // ------------------- Vista previa -------------------
        private void ApplyPreview()
        {
            if (Panel_Color_Primario_Preview != null)
                Panel_Color_Primario_Preview.BackColor = _colorPrimario;

            if (Panel_Color_Secundario_Preview != null)
                Panel_Color_Secundario_Preview.BackColor = _colorSecundario;

            if (Preview_Logo != null)
                Preview_Logo.Image = _logoImg != null ? (Image)_logoImg.Clone() : null;
        }

        // =================== Helpers correo ===================
        private static System.Collections.Generic.IEnumerable<Control> AllControls(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var child in AllControls(c)) yield return child;
            }
        }

        // Lee el correo desde ANY TextBox/Guna2TextBox: por nombre "correo" o por texto/placeholder con '@'
        private string GetCorreoText()
        {
            // 1) Preferimos controles cuyo nombre contenga "correo"
            var candidatos = AllControls(this)
                .Where(c => (c is Guna2TextBox || c is TextBoxBase) &&
                            (c.Name?.IndexOf("correo", StringComparison.OrdinalIgnoreCase) >= 0));

            // 2) Si no hay, probamos TODOS los textboxes (por contenido con '@')
            if (!candidatos.Any())
                candidatos = AllControls(this).Where(c => (c is Guna2TextBox || c is TextBoxBase));

            foreach (var ctrl in candidatos)
            {
                string txt = (ctrl as Guna2TextBox)?.Text?.Trim()
                          ?? (ctrl as TextBoxBase)?.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(txt) && txt.Contains("@"))
                    return txt;

                if (ctrl is Guna2TextBox g)
                {
                    var phProp = g.GetType().GetProperty("PlaceholderText");
                    var dfProp = g.GetType().GetProperty("DefaultText");
                    string ph = phProp?.GetValue(g) as string;
                    string df = dfProp?.GetValue(g) as string;

                    if (!string.IsNullOrWhiteSpace(ph) && ph.Contains("@")) return ph.Trim();
                    if (!string.IsNullOrWhiteSpace(df) && df.Contains("@")) return df.Trim();
                }
            }
            return null;
        }

        // Fija el correo en el primer control "correo" que encuentre; si no, en el primer textbox disponible
        private void SetCorreoText(string valor)
        {
            var ctrl = AllControls(this)
                .FirstOrDefault(c => (c is Guna2TextBox || c is TextBoxBase) &&
                                     (c.Name?.IndexOf("correo", StringComparison.OrdinalIgnoreCase) >= 0));

            if (ctrl == null)
                ctrl = AllControls(this).FirstOrDefault(c => (c is Guna2TextBox || c is TextBoxBase));

            if (ctrl is Guna2TextBox g)
            {
                var phProp = g.GetType().GetProperty("PlaceholderText");
                phProp?.SetValue(g, string.Empty); // evitamos confusión visual
                g.Text = valor ?? "";
            }
            else if (ctrl is TextBoxBase t)
            {
                t.Text = valor ?? "";
            }
        }

        // ------------------- Cargar desde BD -------------------
        private void LoadEmpresaConfig()
        {
            try
            {
                EmpresaConfigService.Cargar();
                var cfg = EmpresaConfigService.EmpresaActual;

                // Texto base
                txt_Nom_Comercial.Text = cfg.NombreComercial ?? "";
                txt_Razon_Social.Text = cfg.RazonSocial ?? "";
                txt_RUC_Config.Text = cfg.RUC ?? "";
                txt_telf_Config.Text = cfg.Telefono ?? "";

                // Dirección en el control correcto
                var dirCtrl = this.Controls.Find("txt_Direccion_Config", true).FirstOrDefault()
                           ?? this.Controls.Find("txt_Direccion", true).FirstOrDefault();

                if (dirCtrl is Guna2TextBox gDir) gDir.Text = cfg.Direccion ?? "";
                else if (dirCtrl is TextBoxBase tDir) tDir.Text = cfg.Direccion ?? "";

                // Correo (helper)
                SetCorreoText(cfg.Correo ?? "");

                // Colores
                _colorPrimario = cfg.ColorPrimario;
                _colorSecundario = cfg.ColorSecundario;
                SetBtnColor(btn_Color_Primario, _colorPrimario);
                SetBtnColor(btn_Color_Secundario, _colorSecundario);

                // HEX en los textbox correctos
                if (txt_color_primario != null) txt_color_primario.Text = EmpresaConfigService.ColorAHex(_colorPrimario);
                if (txt_color_secundario != null) txt_color_secundario.Text = EmpresaConfigService.ColorAHex(_colorSecundario);

                // Logo
                var logo = EmpresaConfigService.ObtenerLogoComoImagen();
                if (logo != null)
                {
                    _logoImg = (Image)logo.Clone();
                    _logoPath = null; // viene de BD
                    pcb_Logo_Conf.Image = (Image)_logoImg.Clone();
                    Preview_Logo.Image = (Image)_logoImg.Clone();
                    lbl_nom_img.Text = TruncarNombreArchivo("logo.png", 15);
                }
                else
                {
                    _logoImg = null; _logoPath = null;
                    pcb_Logo_Conf.Image = null;
                    Preview_Logo.Image = null;
                    lbl_nom_img.Text = "…………………";
                }

                ApplyPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar configuración.\n" + ex.Message,
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ------------------- Guardar en BD -------------------
        private void Btn_Guardar_Dashboard_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Nom_Comercial?.Text))
            {
                MessageBox.Show("Ingresa el Nombre comercial.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Nom_Comercial?.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_Razon_Social?.Text))
            {
                MessageBox.Show("Ingresa la Razón social.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Razon_Social?.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_RUC_Config?.Text))
            {
                MessageBox.Show("Ingresa el R.U.C.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_RUC_Config?.Focus();
                return;
            }

            try
            {
                SaveEmpresaConfig();
                MessageBox.Show("Configuración guardada en la base de datos.",
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la configuración.\n" + ex.Message,
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string TruncarNombreArchivo(string nombreArchivo, int max = 15)
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

        private void SaveEmpresaConfig()
        {
            // Colores a HEX
            string c1 = EmpresaConfigService.ColorAHex(_colorPrimario);
            string c2 = EmpresaConfigService.ColorAHex(_colorSecundario);

            byte[] logoBytes = null;
            string mimeType = null;

            if (_logoImg != null && !string.IsNullOrEmpty(_logoPath))
            {
                logoBytes = File.ReadAllBytes(_logoPath);
                mimeType = DetectMimeFromExtension(Path.GetExtension(_logoPath));
            }
            else if (_logoImg != null && string.IsNullOrEmpty(_logoPath))
            {
                using (var ms = new MemoryStream())
                {
                    _logoImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    logoBytes = ms.ToArray();
                }
                mimeType = "image/png";
            }

            // Dirección (prioriza txt_Direccion_Config)
            var dirCtrl = this.Controls.Find("txt_Direccion_Config", true).FirstOrDefault()
                       ?? this.Controls.Find("txt_Direccion", true).FirstOrDefault();
            string direccionValue = (dirCtrl as Guna2TextBox)?.Text
                                 ?? (dirCtrl as TextBoxBase)?.Text
                                 ?? null;

            // Correo (robusto)
            string correoValue = GetCorreoText();

            var dto = new EmpresaConfigService.EmpresaDatos
            {
                RazonSocial = txt_Razon_Social?.Text,
                NombreComercial = txt_Nom_Comercial?.Text,
                RUC = txt_RUC_Config?.Text,
                Direccion = direccionValue,
                Telefono = txt_telf_Config?.Text,
                Correo = string.IsNullOrWhiteSpace(correoValue) ? null : correoValue,
                ColorPrimarioHex = c1,
                ColorSecundarioHex = c2,
                Logo = logoBytes,
                LogoMimeType = mimeType
            };

            EmpresaConfigService.Guardar(dto);
        }

        private string DetectMimeFromExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return "application/octet-stream";
            ext = ext.ToLowerInvariant();
            return ext == ".png" ? "image/png" :
                   ext == ".jpg" || ext == ".jpeg" ? "image/jpeg" :
                   "application/octet-stream";
        }

        private void TxtColorPrimario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidarYAplicarColor(sender as Guna2TextBox, true);
                e.SuppressKeyPress = true;
            }
        }

        private void TxtColorSecundario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidarYAplicarColor(sender as Guna2TextBox, false);
                e.SuppressKeyPress = true;
            }
        }

        private void ValidarYAplicarColor(Guna2TextBox txt, bool esPrimario)
        {
            if (txt == null) return;
            string valor = txt.Text.Trim();

            if (!valor.StartsWith("#"))
                valor = "#" + valor;

            try
            {
                Color nuevoColor = ColorTranslator.FromHtml(valor);

                if (esPrimario)
                {
                    _colorPrimario = nuevoColor;
                    SetBtnColor(btn_Color_Primario, nuevoColor);
                    txt.Text = valor.ToUpper();
                }
                else
                {
                    _colorSecundario = nuevoColor;
                    SetBtnColor(btn_Color_Secundario, nuevoColor);
                    txt.Text = valor.ToUpper();
                }

                ApplyPreview();
            }
            catch
            {
                MessageBox.Show("El valor ingresado no es un color HEX válido.\nEjemplo válido: FF5733 o #FF5733",
                    "Color inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
                txt.Clear();
            }
        }
    }
}
