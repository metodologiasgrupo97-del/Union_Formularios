using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;

namespace Union_Formularios
{
    public partial class Recuperar_Contraseña_Formulario : Form
    {
        public Recuperar_Contraseña_Formulario()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var usuario = new Modelo_Dominio_Usuario();
            var resultado = usuario.Recuperar_Contraseña(txtRecuperar.Text);
            label2.Text = resultado;
        }
    }
}
