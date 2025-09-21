using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Datos_Acceso.MailServices
{
    public abstract class Servidor_de_Correo_Maestro
    {
        private SmtpClient smtpClient;
        protected string senderMail { get; set; }
        protected string password { get; set; }
        protected string host { get; set; }
        protected int port { get; set; }
        protected bool ssl { get; set; }
        protected void InicializarSMTP() 
        {
            smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential(senderMail, password);
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
        }

        public void enviarmensajedecorreo (string asunto, string cuerpo, List<string> destinatario_Correo) 
        {
            var mailMessage = new MailMessage();
            try
            {
                mailMessage.From = new MailAddress(senderMail);
                foreach (string mail in destinatario_Correo) 
                {
                    mailMessage.To.Add(mail);
                }
                mailMessage.Subject = asunto; 
                mailMessage.Body = cuerpo;
                mailMessage.Priority = MailPriority.Normal;
                smtpClient.Send(mailMessage);
            }
            catch (Exception) { }
            finally 
            {
                mailMessage.Dispose();
                smtpClient.Dispose();
            }
        }
    }
}
