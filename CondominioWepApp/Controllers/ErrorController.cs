using CondominioWepApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using Firebase.Storage;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using CondominioWepApp.FirebaseAuth;

namespace CondominioWepApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ErrorController
        public ActionResult Index()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            ViewBag.Error = new ErrorHandler()
            {
                Title = "You must login to access this resource",
                ErrorMessage = "Session is inactive",
                ActionMessage = "Go to login",
                Path = "/Login"
            };

            return View("ErrorHandler");
        }

        public IActionResult GetUserName()
        {
            // Leemos de la sesión los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            // Pasamos el nombre de usuario a la vista
            ViewBag.UserName = user?.Name;

            return View("Index");
        }

        // GET: ErrorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ErrorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ErrorController/Create
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

        // GET: ErrorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ErrorController/Edit/5
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

        // GET: ErrorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ErrorController/Delete/5
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