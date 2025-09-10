using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Modelo_Dominio_Vehiculos
    {
        public int VehicleID { get; set; }
        public string Placa { get; set; }

        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int AnioModelo { get; set; }

        public string NumeroMotor { get; set; }
        public string NumeroChasis { get; set; }
        public string Color { get; set; }
        public string Combustible { get; set; }
        public int Kilometraje { get; set; }
        public string Estado { get; set; }

        public int ID_Propietario { get; set; }
        public string Propietario { get; set; }
    }

}
