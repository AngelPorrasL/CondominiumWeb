using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CondominioWepApp.FirebaseAuth;
using CondominioWepApp.Models;

namespace CondominioWepApp.Controllers
{
	public class AdminController : Controller
	{
		public IActionResult Index()
		{
			if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
				return RedirectToAction("Index", "Error");

			ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 1)
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

			return View("Index");
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string UserId)
        {
			if (ViewBag.User is Models.User user)
			{
				if (user.Role != 1)
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

			try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas eliminar en Firebase
                var cardDocRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                    .Collection("Users")
                    .Document(UserId);

                // Borra el documento de la tarjeta
                await cardDocRef.DeleteAsync();

                // Redirige a la vista principal (Index) después de eliminar la tarjeta
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string UserId, string cedula, string name, string role, string homecode, string phone, string placalibre)
        {
            try
            {
                // First, get a reference to the document of the visit you want to edit in Firebase
                var cardDocRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                    .Collection("Users")
                    .Document(UserId);

                // Create a dictionary to hold the updated visit data
                var updatedUserData = new Dictionary<string, object>()
                {
                    { "Cedula", cedula },
                    { "Name", name },
                    { "Role", role },
                    { "HomeCode", homecode },
                    { "Phone", phone },
                    { "PlacaLibre", placalibre }
                };

                // Update the visit document with the updated data
                await cardDocRef.UpdateAsync(updatedUserData);

                // Redirect to the List view after editing the visit
                return RedirectToAction("Index", "Admin");
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go back",
                    Path = "/Visitas"
                };

                return View("ErrorHandler");
            }
        }

        //public IActionResult Edit(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
        //{

        //     User edited = new User
        //     {
        //         Id = id,
        //         Name = name,
        //         Email = email,
        //         PhotoPath = photopath,
        //         Role = role,
        //         Logo = logo,
        //         HomeCode = homecode,
        //         Phone = phone,
        //         PlacaLibre = placalibre,
        //         Cedula = cedula
        //     };

        //     ViewBag.Edited = edited;
        //     return View();
        //}

        //// POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult EditUser(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
        //{
        //     try
        //     {
        //         UsersHandler usersHandler = new UsersHandler();

        //         bool result = usersHandler.Edit(id, name, email, photopath, role, logo, homecode, phone, placalibre, cedula).Result;

        //         return GetUsers();
        //     }
        //     catch (FirebaseStorageException ex)
        //     {
        //         ViewBag.Error = new ErrorHandler()
        //         {
        //             Title = ex.Message,
        //             ErrorMessage = ex.InnerException?.Message,
        //             ActionMessage = "Go to Home",
        //             Path = "/Admin"
        //         };

        //         return View("ErrorHandler");
        //     }
        //}

    }
}
