using System;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DATC_tema_lab3
{
    class Program
    {
        public static DriveService _service;
        private static string _token;
        static void Main(string[] args)
        {
            initializer();
        }

        static void initializer()
        
        {
            string[] scopes=new string[]
            {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };
            var clientId="707306504173-eu4h8hm6l36u2m9b6pvfgralaofivn59.apps.googleusercontent.com";
            var clientSecret="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";
            var credential=GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId=clientId,
                    ClientSecret=clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                 new FileDataStore("Dajmato_GoogleDrive.Auth.Store2")
                 ).Result;

                 _service=new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
                 {
                     HttpClientInitializer=credential
                 });

                 _token =credential.Token.AccessToken;
                 Console.Write("Token: " + credential.Token.AccessToken);
                 GetMyFiles();
        }

        static void GetMyFiles()
        {
            var request=(HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization,"Bearer "+ _token);
           

           using (var response= request.GetResponse())
           {
            using( Stream data=response.GetResponseStream())
            using (var reader = new StreamReader(data))
            {
                 string text=reader.ReadToEnd();
                 var myData=JObject.Parse(text);
                 foreach (var file in myData["files"])
                 {
                     if(file["mineType"].ToString()!= "application/vnd.google-apps.folder")
                     {
                         Console.WriteLine("File name: " + file["name"]);
                     }
                 }
            }
           }
        }
    }
}

