using System.Net.Mail;
using System.Net;
using CondominioWepApp.Models;
using QRCoder;

namespace CondominioWepApp.Email
{
    public class EmailSender
    {
        public static void SendEmail(SmtpParams smtpParams)
        {
            using (MailMessage mm = new MailMessage("angelporras799@gmail.com", smtpParams.ReceiverEmail))
            {
                mm.Subject = "Bienvenido al Condominio Buenaventura";
                mm.IsBodyHtml = true;

                using (var sr = new StreamReader("wwwroot/welcome.txt"))
                {
                    // Read the stream as a string, and write the string to the console.
                    string body = sr.ReadToEnd().Replace("@CLIENTNAME", smtpParams.ClientName);
                    body = body.Replace("@IMAGEQR", Misc.QRGenerator.GenerateQR());
                    mm.Body = body;
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("angelporras799@gmail.com", "equacjtooxuviynf");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }
}
