using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.AspNetCore.DataProtection;

namespace CondominioWepApp.FirebaseAuth
{
	public static class FirebaseAuthHelper
	{
        public const string firebaseAppId = "condominio-cc812";
        public const string firebaseApiKey = "AIzaSyAUohhZ_VZsq-JzImrkMOn4ev014pG-bxc";

        public static FirebaseAuthClient setFirebaseAuthClient()
		{
			var response = new FirebaseAuthClient(new FirebaseAuthConfig
			{
				ApiKey = firebaseApiKey,
				AuthDomain = $"{firebaseAppId}.firebaseapp.com",
				Providers = new FirebaseAuthProvider[]
					{
						new EmailProvider()
					}
			});

			return response;
		}
	}
}
