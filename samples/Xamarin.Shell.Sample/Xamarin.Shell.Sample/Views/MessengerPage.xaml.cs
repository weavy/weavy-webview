using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Shell.Sample.Helpers;

namespace Xamarin.Shell.Sample.Views {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MessengerPage : ContentPage {
        public MessengerPage()
        {
            InitializeComponent();

            weavyWebView.AuthenticationToken = AuthenticationHelpers.JwtToken;
            weavyWebView.InitCompleted += (s, a) => {
                weavyWebView.Load();
            };
        }
    }
}