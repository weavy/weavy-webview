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
    public partial class EmbeddedPage : ContentPage
    {
        public EmbeddedPage()
        {
            InitializeComponent();

            // url to demo site
            var baseUrl = "https://mobiletest.weavycloud.com";
            
            // a persistent demo jwt token that works for the demo site above
            var demoToken = AuthenticationHelpers.JwtToken;

            // set the url for the different web views
            weavyWebView.Uri = $"{baseUrl}/e/apps/4";
            weavyWebView2.Uri = $"{baseUrl}/e/apps/1";
            weavyWebView3.Uri = $"{baseUrl}/e/apps/3";

            // set the authentication token
            weavyWebView.AuthenticationToken = weavyWebView2.AuthenticationToken = weavyWebView3.AuthenticationToken = demoToken;
            
            // load the web view after init is complete.
            weavyWebView.InitComplete += (sender, args) => { weavyWebView.Load(); };
            weavyWebView2.InitComplete += (sender, args) => { weavyWebView2.Load(); };
            weavyWebView3.InitComplete += (sender, args) => { weavyWebView3.Load(); };

            // handle external link clicks in Weavy web view
            weavyWebView.LinkClicked += (sender, args) =>
            {
                Launcher.OpenAsync(new Uri(args.Url));
            };


            // -------------------------------------------------------------------------------------------
            // example below shows how you can inject custom script and register a callback event.
            // -------------------------------------------------------------------------------------------

            //weavyWebView.RegisterCallback("customCallback", (args) =>
            //{
            //    DisplayAlert("My alert", $"The message from the web view is {args}", "Oki");
            //});

            //weavyWebView.LoadFinished += (sender, args) =>
            //{
            //                weavyWebView.InjectJavaScript(@"
            //function myCustomFunc(value){
            //    Native('customCallback', value);
            //}
            //");
            //                weavyWebView.CallJsFunction("myCustomFunc", new { value = 123 });
            //};


        }

    }
}