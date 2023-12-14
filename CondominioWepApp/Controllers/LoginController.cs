using CondominioWepApp.Models;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CondominioWepApp.FirebaseAuth;
using Newtonsoft.Json;
using Google.Cloud.Firestore;
using Google.Type;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;
using System.Xml.Linq;
using System.Reflection;


namespace CondominioWepApp.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // Elimina los datos de sesión
            HttpContext.Session.Remove("userSession");

            // Redirige al usuario a la página de inicio de sesión (o a donde desees)
            return RedirectToAction("Index", "Login");
        }

        public IActionResult GetUserName()
        {
            // Leemos de la sesión los datos del usuario
            Models.User? user = JsonConvert.DeserializeObject<Models.User>(HttpContext.Session.GetString("userSession"));

            // Pasamos el nombre de usuario a la vista
            ViewBag.UserName = user?.Name;

            return View("Index");
        }

        public async Task<IActionResult> Login(string loginUsername, string loginPassword)
        {
            try
            {
                //Aqui Firebase Auth nos devuelve si el usuario existe y esta activo
                UserCredential taskUser = await FirebaseAuthHelper.setFirebaseAuthClient().SignInWithEmailAndPasswordAsync(loginUsername, loginPassword);

                // Crea una consulta en Firestore Database que busca el usuario
                Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Users").WhereEqualTo("Id", taskUser.User.Uid);

                // Ejecuta la consulta y obtenemos el dato
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
                Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

                //Aqui creamos el objeto del usuario
                Models.User user = new Models.User
                {
                    DocumentId = data["DocumentId"].ToString(),
                    Id = taskUser.User.Uid,
                    Name = taskUser.User.Info.DisplayName,
                    Email = taskUser.User.Info.Email,
                    PhotoPath = data["PhotoPath"].ToString(),
                    Role = Convert.ToInt16(data["Role"]),
                    Logo = data["Logo"].ToString(),
                    HomeCode = data["HomeCode"].ToString(),
                    Phone = data["Phone"].ToString(),
                    PlacaLibre = data["PlacaLibre"].ToString(),
                    Cedula = data["Cedula"].ToString(),
                };

                //Aqui guardamos los datos del usuario en la session en formato json
                HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(user));

                if (user.Role == 2)
                {
                    return RedirectToAction("Index", "Security");
                }
                else if (user.Role == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    if (user.Role == 0)
                    {
                        //Send email only when role is 0
                        SmtpParams smtp = new SmtpParams
                        {
                            ReceiverEmail = taskUser.User.Info.Email,
                            ClientName = taskUser.User.Info.DisplayName
                        };

                        Email.EmailSender.SendEmail(smtp);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }
        }

        public async Task<IActionResult> SignUp(string signupUsername, string signupPassword, string signupDisplayName)
        {
            try
            {

                //Aqui Firebase crea el usuario en Firebase Auth 
                UserCredential taskUser = await FirebaseAuthHelper.setFirebaseAuthClient().
                    CreateUserWithEmailAndPasswordAsync(signupUsername, signupPassword, signupDisplayName);

                //Aqui creamos el usuario en la collection de Firestore Database
                DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Users").AddAsync(new Dictionary<string, object>
                            {
                                { "DocumentId", "" },
                                { "Id", taskUser.User.Uid },
                                { "Name", taskUser.User.Info.DisplayName },
                                { "Email", taskUser.User.Info.Email },
                                { "PhotoPath", "https://firebasestorage.googleapis.com/v0/b/condominio-cc812.appspot.com/o/ProfilePhotos%2FUser.png?alt=media&token=5bcaab2c-aeb1-4184-9955-fe91218f8b75" },
                                {"Role", 0},
                                {"Logo", "" },
                                {"HomeCode", "" },
                                {"Phone", "" },
                                {"PlacaLibre", "" },
                                {"Cedula", "" },
                            });

                //Aqui actualizamos el usuario en Firestore Database para actualizar el DocumentId
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Users").Document(addedDocRef.Id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    { "DocumentId", addedDocRef.Id },
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                ////Send email
                //SmtpParams smtp = new SmtpParams
                //{
                //    ReceiverEmail = taskUser.User.Info.Email,
                //    ClientName = taskUser.User.Info.DisplayName
                //};

                //Email.EmailSender.SendEmail(smtp);

                return View("Index");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }
        }

        public async Task<IActionResult> ForgotPwd(string signupUsername)
        {
            try
            {
                await FirebaseAuthHelper.setFirebaseAuthClient().
                    ResetEmailPasswordAsync(signupUsername);

                return View("Index");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return FirebaseAuthHttpExceptionHandler(ex);
            }
        }

        public IActionResult FirebaseAuthHttpExceptionHandler(FirebaseAuthHttpException ex)
        {
            ViewBag.Error = new ErrorHandler()
            {
                Title = ex.Reason.ToString(),
                ErrorMessage = ex.InnerException?.Message,
                ActionMessage = "Go to login",
                Path = "/Login"
            };

            return View("ErrorHandler");
        }
    }
}