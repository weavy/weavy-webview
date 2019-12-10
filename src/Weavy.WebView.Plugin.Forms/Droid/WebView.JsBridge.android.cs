using Android.Webkit;
using Java.Interop;
using System;

namespace Weavy.WebView.Plugin.Forms.Droid
{

    /// <summary>
    /// Js Bridge
    /// </summary>
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<WeavyWebViewRenderer> hybridWebViewRenderer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hybridRenderer"></param>
        public JSBridge(WeavyWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<WeavyWebViewRenderer>(hybridRenderer);
        }

        /// <summary>
        /// Message receiver
        /// </summary>
        /// <param name="data"></param>
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
