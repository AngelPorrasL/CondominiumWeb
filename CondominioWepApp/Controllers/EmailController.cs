using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using CondominioWepApp.Models;

namespace CondominioWepApp.Controllers
{
    public class EmailController : Controller
    {
        // GET: EmailController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult SendEmail(SmtpParams smtpParams)
        {

            using (MailMessage mm = new MailMessage("angelporras799@gmail.com", smtpParams.ReceiverEmail))
            {
                mm.Subject = smtpParams.Subject;
                //mm.Body = smtpParams.Body;

                /*mm.Body = model.Body;*/
                /*if (model.Attachment.Length > 0)
                {
                    using (var stream = model.Attachment.OpenReadStream())
                    {
                        var attachment = new Attachment(stream, model.Attachment.FileName);
                        mm.Attachments.Add(attachment);
                    }
                }*/
                mm.IsBodyHtml = true;

                using (var sr = new StreamReader("wwwroot/welcome.txt"))
                {
                    // Read the stream as a string, and write the string to the console.
                    string body = sr.ReadToEnd().Replace("@CLIENTNAME", "ANGEL");
                    body = body.Replace("@IMAGEQR", "https://firebasestorage.googleapis.com/v0/b/booking-1a024.appspot.com/o/ProfilePhotos%2Fdescarga.jpg?alt=media&token=aea6dddf-1902-42c9-83f7-c9f7b6527a08");

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
                return Ok();
            }
        }

        // GET: EmailController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmailController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmailController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmailController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmailController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmailController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
