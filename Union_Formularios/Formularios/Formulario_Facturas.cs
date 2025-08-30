using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Formulario_Principal_Car_EFULL.Formularios
{
    public partial class Formulario_Facturas : Form
    {
        public Formulario_Facturas()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Formulario_Facturas
            // 
            this.ClientSize = new System.Drawing.Size(1069, 619);
            this.Name = "Formulario_Facturas";
            this.Text = "Facturación";
            this.Load += new System.EventHandler(this.Formulario_Facturas_Load);
            this.ResumeLayout(false);

        }

        private void Formulario_Facturas_Load(object sender, EventArgs e)
        {

        }
    }
}
