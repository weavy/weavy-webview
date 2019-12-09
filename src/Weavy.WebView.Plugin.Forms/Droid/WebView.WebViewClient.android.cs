using Android.Content;
using Android.Graphics;
using Android.Webkit;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using Weavy.WebView.Plugin.Forms;
using Weavy.WebView.Plugin.Forms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Weavy.WebView.Plugin.Forms.Droid
{
    public class WeavyWebViewClient : WebViewClient
    {
        private const string INTERNET_DISCONNECTED = "net::ERR_INTERNET_DISCONNECTED";
        private const string CONNECTION_ABORTED = "net::ERR_CONNECTION_ABORTED";
        protected readonly WeakReference<WeavyWebViewRenderer> WebHybrid;

        public WeavyWebViewClient(WeavyWebViewRenderer webHybrid)
        {
            this.WebHybrid = new WeakReference<WeavyWebViewRenderer>(webHybrid);

        }

        /// <summary>
        /// When web page has finished loading
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);

            WeavyWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid))
            {
                hybrid.OnPageFinished();
            }
        }

        /// <summary>
        /// When web page starts loading
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="favicon"></param>
        public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);

            WeavyWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid))
            {
                hybrid.OnLoading();
            }
        }

        /// <summary>
        /// Handle errors in web view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="request"></param>
        /// <param name="error"></param>
        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);

            WeavyWebViewRenderer hybrid;
            if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid))
            {

                if (!error.Description.Equals(INTERNET_DISCONNECTED) && !error.Description.Equals(CONNECTION_ABORTED))
                {
                    hybrid.OnError(error.ErrorCode, error.Description);
                }
            }
        }
    }

}
