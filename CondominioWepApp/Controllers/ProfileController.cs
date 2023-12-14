using CondominioWepApp.FirebaseAuth;
using CondominioWepApp.Models;
using Firebase.Storage;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CondominioWepApp.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            //Leemos de la sesion los datos del usuario y se lo pasamos a la vista
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }

		private IActionResult GetUsers()
		{
			UsersHandler usersHandler = new UsersHandler();

			ViewBag.Users = usersHandler.GetUsersCollection().Result;

			return View("Index");
		}

		public async Task<IActionResult> uploadPhoto(IFormFile photo)
        {
            //Leemos de la sesion los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            //Creamos la ruta donde se va a guardar la foto en el servidor local
            string photoPath = Directory.GetCurrentDirectory() + "\\wwwroot\\" + Path.Combine("Images\\", photo.FileName);

            //Copiamos la foto en la ruta del servidor local
            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            //Abrimos la foto de servidor local para mandarla a Firebase Storage
            var downloadUrl = string.Empty;
            using (var streamToFb = System.IO.File.OpenRead(photoPath))
            {
                //Mandamos la foto a Firebase storage y este nos reponde la URL
                downloadUrl = await new FirebaseStorage($"{FirebaseAuthHelper.firebaseAppId}.appspot.com")
                                 .Child("ProfilePhotos")
                                 .Child(photo.FileName)
                                 .PutAsync(streamToFb);
            }

            ////Subir cedula
            //private IActionResult GetInfo()
            //{
            //    ProfilesHandler profilesHandler = new ProfilesHandler();

            //    ViewBag.Profile = ProfilesHandler.GetInfoCollection().Result;

            //    return View();
            //}


            //Actualizamos en Firestore Database el campo PhotoPath con la URL que nos devolvio Firebase Storage
            FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
            DocumentReference docRef = db.Collection("Users").Document(user?.DocumentId);
            Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
            {
                { "PhotoPath", downloadUrl },
            };
            WriteResult result = await docRef.UpdateAsync(dataToUpdate);

            //Pasamos la URl de la foto a la vista
            user.PhotoPath = downloadUrl;
            ViewBag.User = user;

            return View("Index");
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
        {
            try
            {
                UsersHandler usersHandler = new UsersHandler();

                bool result = usersHandler.Edit(id, name, email, photopath, role, logo, homecode, phone, placalibre, cedula).Result;

                return GetUsers();
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go to Home",
                    Path = "/Admin"
                };

                return View("ErrorHandler");
            }
        }

		public IActionResult Edit(string id, string name, string email, string photopath, int role, string logo, string homecode, string phone, string placalibre, string cedula)
		{

			User edited = new User
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

			ViewBag.Edited = edited;
			return View();
		}

	}
}