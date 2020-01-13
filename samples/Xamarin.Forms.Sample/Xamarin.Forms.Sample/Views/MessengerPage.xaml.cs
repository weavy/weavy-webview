using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Sample.Helpers;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MessengerPage : ContentPage
    {
        public MessengerPage()
        {
            InitializeComponent();

            // a persistent demo jwt token that works for the demo site above
            weavyMessenger.AuthenticationToken = AuthenticationHelpers.JwtToken;

            // load the web view after init is complete. Set the url to the /messenger
            weavyMessenger.InitComplete += (sender, args) =>
            {
                weavyMessenger.Load("https://mobiletest.weavycloud.com/messenger");
            };
        }

    }
}