using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Weavy.WebView.Plugin.Forms.Models;
using Xamarin.Essentials;

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

            var baseUrl = "https://mobiletest.weavycloud.com";
            var demoToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5OTk5IiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJtYWdudXNAd2VhdnkuY29tIiwidXNlcm5hbWUiOiJta3JvbmEiLCJleHAiOiIxNTM0ODIwMjExMDAwIiwiaXNzIjoiTW9iaWxlVGVzdEFwcCJ9.3qU8CRXfLLvzdzhaj35mmJhQzBBkusAUWV1xKLEaG-I";

            weavyWebView.Uri = $"{baseUrl}/e/apps/4";
            weavyWebView2.Uri = $"{baseUrl}/e/apps/1";
            weavyWebView3.Uri = $"{baseUrl}/e/apps/3";

            weavyWebView.AuthenticationToken = weavyWebView2.AuthenticationToken = weavyWebView3.AuthenticationToken = demoToken;
            
            weavyWebView.InitComplete += (sender, args) => { weavyWebView.Load(); };
            weavyWebView2.InitComplete += (sender, args) => { weavyWebView2.Load(); };
            weavyWebView3.InitComplete += (sender, args) => { weavyWebView3.Load(); };

            weavyWebView.LinkClicked += (sender, args) =>
            {
                Launcher.OpenAsync(new Uri(args.Url));
            };

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