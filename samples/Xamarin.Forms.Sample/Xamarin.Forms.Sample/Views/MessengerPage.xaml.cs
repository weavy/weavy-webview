using System;
using System.ComponentModel;
using Xamarin.Forms;
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

            weavyMessenger.AuthenticationToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5OTk5IiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJtYWdudXNAd2VhdnkuY29tIiwidXNlcm5hbWUiOiJta3JvbmEiLCJleHAiOiIxNTM0ODIwMjExMDAwIiwiaXNzIjoiTW9iaWxlVGVzdEFwcCJ9.3qU8CRXfLLvzdzhaj35mmJhQzBBkusAUWV1xKLEaG-I";

        }
    }
}