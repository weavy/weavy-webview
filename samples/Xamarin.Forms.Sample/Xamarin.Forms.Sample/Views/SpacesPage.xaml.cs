﻿using System;
using System.ComponentModel;

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
            
            weavyWebView.AuthenticationToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5OTk5IiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJtYWdudXNAd2VhdnkuY29tIiwidXNlcm5hbWUiOiJta3JvbmEiLCJleHAiOiIxNTM0ODIwMjExMDAwIiwiaXNzIjoiTW9iaWxlVGVzdEFwcCJ9.3qU8CRXfLLvzdzhaj35mmJhQzBBkusAUWV1xKLEaG-I";

            weavyWebView.BadgeUpdated += (sender, args) =>
            {
                Console.WriteLine(args.Conversations);
                Console.WriteLine(args.Notifications);
            };

            weavyWebView.SignInCompleted += (sender, args) =>
            {
                var status = args.Status;                
                Console.WriteLine("Authentication status = ", args.Status);
            };
        }
    }
}