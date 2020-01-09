using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Weavy.WebView.Plugin.Forms.Models;

namespace Xamarin.Forms.Sample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SpacesPage : ContentPage
    {
        public SpacesPage()
        {
            InitializeComponent();

            // set persistant test jwt token
            weavyWebView.AuthenticationToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5OTk5IiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJtYWdudXNAd2VhdnkuY29tIiwidXNlcm5hbWUiOiJta3JvbmEiLCJleHAiOiIxNTM0ODIwMjExMDAwIiwiaXNzIjoiTW9iaWxlVGVzdEFwcCJ9.3qU8CRXfLLvzdzhaj35mmJhQzBBkusAUWV1xKLEaG-I";

            weavyWebView.InitComplete += (s, a) =>
            {
                weavyWebView.Load();
            };

            // listen to badge updated event
            weavyWebView.BadgeUpdated += (sender, args) =>
            {
                Console.WriteLine(args.Conversations);
                Console.WriteLine(args.Notifications);
            };

            weavyWebView.Loading += (sender, args) =>
            {
                Console.WriteLine("Loading webview...");
            };

            weavyWebView.LoadFinished += (sender, args) =>
            {
                Console.WriteLine("Load webview finished...");

                weavyWebView.GetUser((data) => {
                    var user = JsonConvert.DeserializeObject<User>(data);                    
                });
            };

            weavyWebView.LoadError += (sender, args) =>
            {
                Console.WriteLine("Error when loading webview!");
            };

            weavyWebView.Theming += (sender, args) =>
            {
                Console.WriteLine("Got theme!");
            };


            // listen to signed in event
            weavyWebView.SignedIn += (sender, args) =>
            {
                // check if successful login
                if(args.Status == AuthenticationStatus.OK)
                {
                    Console.WriteLine("Signed in!", args.Status);
                } else if(args.Status == AuthenticationStatus.NOTAUTHENTICATED || args.Status == AuthenticationStatus.ERROR)
                {
                    Console.WriteLine("Signing in failed!", args.Status, args.Message);
                }                               
            };
        }
    }
}