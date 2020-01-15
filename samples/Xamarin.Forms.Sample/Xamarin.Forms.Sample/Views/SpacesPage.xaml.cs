using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Weavy.WebView.Plugin.Forms.Models;
using Xamarin.Essentials;
using Xamarin.Forms.Sample.Helpers;

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

            // a persistent demo jwt token that works for the demo site above
            weavyWebView.AuthenticationToken = AuthenticationHelpers.JwtToken;

            // load the web view after init is complete. Note that the uri property is set in the .xaml view
            weavyWebView.InitCompleted += (s, a) =>
            {
                weavyWebView.Load();
            };

            // listen to badge updated event
            weavyWebView.BadgeUpdated += (sender, args) =>
            {
                Console.WriteLine(args.Conversations); // messenger notifications
                Console.WriteLine(args.Notifications); // all other notifications
            };

            // web view is loading page
            weavyWebView.Loading += (sender, args) =>
            {
                Console.WriteLine("Loading webview...");
            };

            // an external link was clicked in the web view
            weavyWebView.LinkClicked += (sender, args) =>
            {
                Console.WriteLine("Link clicked...", args.Url);
                Launcher.OpenAsync(args.Url);
            };

            // web view has finished loading page
            weavyWebView.LoadFinished += (sender, args) =>
            {
                Console.WriteLine("Load webview finished...");

                // example of getting current logged in user
                weavyWebView.GetUser((data) => {
                    var user = JsonConvert.DeserializeObject<User>(data);                    
                });
            };

            // an error occurred when loading the page
            weavyWebView.LoadError += (sender, args) =>
            {
                Console.WriteLine("Error when loading webview!");
            };

            // get the current theme from the Weavy web site
            weavyWebView.Theming += (sender, theme) =>
            {
                Console.WriteLine("Got theme!", theme);
            };

            MessagingCenter.Subscribe<App>(this, "APP_RESUME", (s) => {
                weavyWebView.Resume();
            });

        }
    }
}