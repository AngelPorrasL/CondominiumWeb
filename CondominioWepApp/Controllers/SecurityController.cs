using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CondominioWepApp.Models;

namespace CondominioWepApp.Controllers
{
    public class SecurityController : Controller
    {
        //Index se encarga de mostrar los condominos y seleccionarlos para trabajar en su proyecto habitacional
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 2)
                {
                    //Redirige a la página de error si el usuario no tiene un rol válido

                    return RedirectToAction("Index", "Error");
                }

                ViewBag.Role = user.Role;
            }
            else
            {
                //Redirigir a la pagina que se selecciono

                return RedirectToAction("Index", "Admin");
            }

            return GetUsers();
        }

        public IActionResult Vehicles()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 2)
                {
                    //Redirige a la página de error si el usuario no tiene un rol válido

                    return RedirectToAction("Index", "Error");
                }

                ViewBag.Role = user.Role;
            }
            else
            {
                //Redirigir a la pagina que se selecciono

                return RedirectToAction("Index", "Admin");
            }

            return GetUsers();
        }

        private IActionResult GetVisits()
        {
            VisitsHandler visitsHandler = new VisitsHandler();

            ViewBag.Visits = visitsHandler.GetVisitsCollection().Result;

            return View();
        }

        public IActionResult VisitCheck()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 2)
                {
                    //Redirige a la página de error si el usuario no tiene un rol válido

                    return RedirectToAction("Index", "Error");
                }

                ViewBag.Role = user.Role;
            }
            else
            {
                //Redirigir a la pagina que se selecciono

                return RedirectToAction("Index", "Admin");
            }

            return GetVisits();
        }

        public IActionResult GetUserName()
        {


            // Leemos de la sesión los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            // Pasamos el nombre de usuario a la vista
            ViewBag.UserName = user?.Name;

            return View();
        }

        private IActionResult GetUsers()
        {
            UsersHandler usersHandler = new UsersHandler();

            ViewBag.Users = usersHandler.GetUsersCollection().Result;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewVisit(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
        {
            try
            {
                UsersHandler usersHandler = new UsersHandler();

                bool result = usersHandler.View(id, name,  email, photopath, role, logo, homecode, phone, placalibre, cedula).Result;

                return GetUsers();
            }

            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to index",
                    Path = "/Vehiculos"
                };

                return View("ErrorHandler");
            }
        }
        //View se encarga de mostrar el condomino seleccionado y mostrar las visitras creadas del mismo
        public IActionResult View(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            User viewed = new User
             {
                Id = id,
                Name = name,
                Email = email,
                PhotoPath = photopath,
                Role = role,
                Logo = logo,
                HomeCode = homecode,
                Phone = phone,
                PlacaLibre = placalibre,
                Cedula = cedula
             };

            ViewBag.Viewed = viewed;

			//Muestra el get en la vista
			return GetVisits();
		}
    }
}
