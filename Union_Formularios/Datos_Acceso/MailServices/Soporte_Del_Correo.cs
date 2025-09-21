using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos_Acceso.MailServices
{
    class Soporte_Del_Correo:Servidor_de_Correo_Maestro
    {
        public Soporte_Del_Correo() 
        {
            senderMail = "efullcar@gmail.com";
            password = "carefull@2025+";
            host = "smtp.gmail.com";
            port = 587;
            ssl = true;
            InicializarSMTP();
        }
    }

}