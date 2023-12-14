using CondominioWepApp.FirebaseAuth;
using Firebase.Storage;
using Google.Cloud.Firestore;

namespace CondominioWepApp.Models
{
    public class Favorite
    {
        public string? Id { get; set; }
        public string? Cedula { get; set; }
        public string? Name { get; set; }
        public string? Vehicle { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public string? UserName { get; set; }
    }

    public class FavoritesHandler
    {
        public async Task<List<Favorite>> GetFavoritesCollection()
        {
            List<Favorite> favoritesList = new List<Favorite>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Favorites");
            QuerySnapshot querySnaphot = await query.GetSnapshotAsync();

            foreach (var item in querySnaphot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                favoritesList.Add(new Favorite
                {
                    Id = item.Id,
                    Cedula = data["Cedula"].ToString(),
                    Name = data["Name"].ToString(),
                    Vehicle = data["Vehicle"].ToString(),
                    Brand = data["Brand"].ToString(),
                    Model = data["Model"].ToString(),
                    UserName = data["UserName"].ToString(),
                    Color = data["Color"].ToString()
                });
            }

            return favoritesList;
        }

        public async Task<bool> Create(string cedula, string name, string vehicle, string brand, string model, string color, string userName)
        {
            try
            {
                DocumentReference addedDocRef =
                 await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                     .Collection("Favorites").AddAsync(new Dictionary<string, object>
                         {
                                    { "Cedula", cedula },
                                    { "Name", name },
                                    { "Vehicle", vehicle },
                                    { "Brand",  brand },
                                    { "Model", model },
                                    { "Color", color },
                                    { "UserName", userName } // Agrega el nombre de usuario
                         });

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Edit(string id, string cedula, string name, string vehicle, string brand, string model, string color)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Favorites").Document(id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    { "Cedula", cedula },
                    { "Name", name },
                    { "Vehicle", vehicle },
                    { "Brand",  brand },
                    { "Model", model },
                    { "Color", color },
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

    }

}