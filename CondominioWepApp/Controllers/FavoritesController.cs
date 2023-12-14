using Microsoft.AspNetCore.Mvc;
using CondominioWepApp.Models;
using Firebase.Storage;
using Newtonsoft.Json;
using CondominioWepApp.FirebaseAuth;
using Google.Cloud.Firestore;

namespace CondominioWepApp.Controllers
{
    public class FavoritesController : Controller
    {
        // GET: FavoritosController
        public IActionResult Index()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 0)
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

            //Muestra el get en la vista
            return View();
        }

        private IActionResult GetFavorites()
        {
            FavoritesHandler favoritesHandler = new FavoritesHandler();

            ViewBag.Favorites = favoritesHandler.GetFavoritesCollection().Result;

            return View();
        }

        public IActionResult Favorites()
        {
            ViewBag.User = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                return RedirectToAction("Index", "Error");

            if (ViewBag.User is Models.User user)
            {
                if (user.Role != 0)
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

            return GetFavorites();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string cedula, string name, string vehicle, string brand, string model, string color)
        {
            try
            {
                // Obtiene el usuario actual de la sesión
                Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));
                if (user == null)
                {
                    // Manejar el escenario donde el usuario no está autenticado
                    // Puedes redirigir a una página de error u otra acción apropiada
                    return RedirectToAction("Index", "Error");
                }

                // Obtiene el nombre de usuario del objeto User
                string userName = user.Name;

                FavoritesHandler favoritesHandler = new FavoritesHandler();

                bool result = favoritesHandler.Create(cedula, name, vehicle, brand, model, color, userName).Result;

                return View("Index");
            }

            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go back",
                    Path = "/Favorites"
                };

                return View("ErrorHandler");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string FavId)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas eliminar en Firebase
                var cardDocRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                    .Collection("Favorites")
                    .Document(FavId);

                // Borra el documento de la tarjeta
                await cardDocRef.DeleteAsync();

                // Redirige a la vista principal (Index) después de eliminar la tarjeta
                return RedirectToAction("Favorites", "Favorites");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine("Error al eliminar tarjeta: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFavorites(string FavoriteId, string cedula, string name, string vehicle, string brand, string model, string color)
        {
            try
            {
                // First, get a reference to the document of the visit you want to edit in Firebase
                var visitDocRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                    .Collection("Favorites")
                    .Document(FavoriteId);

                // Create a dictionary to hold the updated visit data
                var updatedFavoriteData = new Dictionary<string, object>()
                {
                    { "Cedula", cedula },
                    { "Name", name },
                    { "Vehicle", vehicle },
                    { "Brand", brand },
                    { "Model", model },
                    { "Color", color },
                };

                // Update the visit document with the updated data
                await visitDocRef.UpdateAsync(updatedFavoriteData);

                // Redirect to the List view after editing the visit
                return RedirectToAction("Favorites", "Favorites");
            }
            catch (FirebaseStorageException ex)
            {
                ViewBag.Error = new ErrorHandler()
                {
                    Title = ex.Message,
                    ErrorMessage = ex.InnerException?.Message,
                    ActionMessage = "Go back",
                    Path = "/Favorites"
                };

                return View("ErrorHandler");
            }
        }

    }
}