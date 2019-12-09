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
    

    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<WeavyWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(WeavyWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<WeavyWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("call")]
        public void Call(string data)
        {
            WeavyWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                if (hybridRenderer != null && hybridRenderer.Element != null)
                {
                    hybridRenderer.Element.MessageReceived(data);
                }

            }
        }
    }
}
