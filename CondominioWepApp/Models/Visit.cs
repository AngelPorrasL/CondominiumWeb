using CondominioWepApp.FirebaseAuth;
using Firebase.Storage;
using Google.Cloud.Firestore;
using Google.Type;
using System.Xml.Linq;

namespace CondominioWepApp.Models
{
    public class Visit
    {
        public string? Id { get; set; }
        public string? Cedula { get; set; }
        public string? Name { get; set; }
        public string? Vehicle { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public string? Date { get; set; }
        public int? Acceso { get; set; }
        public string? UserName { get; set; }
    }


    public class VisitsHandler
    {

        public async Task<List<Visit>> GetVisitsCollection()
        {
            List<Visit> visitsList = new List<Visit>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Visits");
            QuerySnapshot querySnaphot = await query.GetSnapshotAsync();

            foreach (var item in querySnaphot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                visitsList.Add(new Visit
                {
                    Id = item.Id,
                    Cedula = data["Cedula"].ToString(),
                    Name = data["Name"].ToString(),
                    Vehicle = data["Vehicle"].ToString(),
                    Brand = data["Brand"].ToString(),
                    Model = data["Model"].ToString(),
                    Color = data["Color"].ToString(),
                    Date = data["Date"].ToString(),
                    UserName = data["UserName"].ToString(),
                    Acceso = Convert.ToInt16(data["Acceso"])
                });
            }

            return visitsList;
        }

        public async Task<bool> Create(string cedula, string name, string vehicle, string brand, string model, string color, string date, int acceso, string userName)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                        .Collection("Visits").AddAsync(new Dictionary<string, object>
                        {
                            { "Cedula", cedula },
                            { "Name", name },
                            { "Vehicle", vehicle },
                            { "Brand",  brand },
                            { "Model", model },
                            { "Color", color },
                            { "Date", date },
                            { "Acceso", acceso },
                            { "UserName", userName } // Agrega el nombre de usuario
                        });

                return true;
            }
            catch (FirebaseStorageException ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Edit(string id, string cedula, string name, string vehicle, string brand, string model, string color, string date, int acceso)
        {
            try
            {
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                DocumentReference docRef = db.Collection("Visits").Document(id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                   { "Cedula", cedula },
                   { "Name", name },
                   { "Vehicle", vehicle },
                   { "Brand",  brand },
                   { "Model", model },
                   { "Color", color },
                   { "Date", date },
                   { "Acceso", acceso }
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