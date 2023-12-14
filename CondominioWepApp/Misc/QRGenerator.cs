using Firebase.Storage;
using QRCoder;
using CondominioWepApp.FirebaseAuth;
using System;
using System.Drawing;

namespace CondominioWepApp.Misc
{
    public class QRGenerator
    {
        public static string GenerateQR()
        {
            string qrContent = new Random().Next(1000, 9999).ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save("wwwroot/qr/" + qrContent + ".png", System.Drawing.Imaging.ImageFormat.Png);

            return UploadQRToFirebase("wwwroot/qr/" + qrContent + ".png", qrContent + ".png").Result;
        }

        public static async Task<string> UploadQRToFirebase(string qrPath, string qrName)
        {
            var downloadUrl = string.Empty;
            using (var streamToFb = System.IO.File.OpenRead(qrPath))
            {
                //Mandamos la foto a Firebase storage y este nos reponde la URL
                downloadUrl = await new FirebaseStorage($"{FirebaseAuthHelper.firebaseAppId}.appspot.com")
                                 .Child("QR")
                                 .Child(qrName)
                                 .PutAsync(streamToFb);
            }

            return downloadUrl;
        }
    }
}
