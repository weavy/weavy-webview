using Android.Content;
using Android.Webkit;
using System;
using System.Text;
using Weavy.WebView.Plugin.Forms;
using Weavy.WebView.Plugin.Forms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WeavyWebView), typeof(WeavyWebViewRenderer))]
namespace Weavy.WebView.Plugin.Forms.Droid
{
    /// <summary>
    /// Droid implementation of the weavy web view
    /// </summary>
    public class WeavyWebViewRenderer : ViewRenderer<WeavyWebView, Android.Webkit.WebView>
    {
        private const string NativeFuncCall = "jsBridge.call";
        private const string NativeFunction = "function Native(action, data){jsBridge.call(JSON.stringify({ a: action, d: data }));}";
        Context _context;

        public static Func<WeavyWebViewRenderer, WeavyWebViewClient> GetWebViewClientDelegate;
        
        public WeavyWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        #region protected methods

        protected override void OnElementChanged(ElementChangedEventArgs<WeavyWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {                
                var webView = new Android.Webkit.WebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;
                webView.LayoutParameters = new Android.Widget.LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                webView.SetWebViewClient(GetWebViewClient());
                //webView.SetWebChromeClient(new FileChooserWebChromeClient(Context as MainActivity));                
                CookieManager.Instance.SetAcceptThirdPartyCookies(webView, true);
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as WeavyWebView;
                //hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl(Element.Uri);

                // handle javascript injection requests
                e.NewElement.JavaScriptLoadRequested += (sender, script) => {
                    InjectJS(script);
                };

                // handle go back requests
                e.NewElement.GoBackRequested += (sender, args) => {
                    if (!Control.CanGoBack()) return;

                    Control.GoBack();
                };

                // handle go formward requests
                e.NewElement.GoForwardRequested += (sender, args) => {
                    if (!Control.CanGoForward()) return;

                    Control.GoForward();
                };
            }
        }

        /// <summary>
        /// Get the custom web view client implementation
        /// </summary>
        /// <returns></returns>
        protected virtual WeavyWebViewClient GetWebViewClient()
        {
            var d = GetWebViewClientDelegate;

            return d != null ? d(this) : new WeavyWebViewClient(this);
        }

        #endregion

        #region public methods
        /// <summary>
        /// On loading
        /// </summary>
        public void OnLoading()
        {
            var hybridWebView = Element as WeavyWebView;
            if (hybridWebView == null) return;

            hybridWebView.OnLoading(this, EventArgs.Empty);
        }

        /// <summary>
        /// On error
        /// </summary>
        public void OnError(ClientError code, string description)
        {
            var hybridWebView = Element as WeavyWebView;
            if (hybridWebView == null) return;

            hybridWebView.OnLoadError(this, EventArgs.Empty);
        }

        /// <summary>
        /// On page finished
        /// </summary>
        public void OnPageFinished()
        {
            var hybridWebView = Element as WeavyWebView;
            if (hybridWebView == null) return;

            InjectJS(NativeFunction);
            InjectJS(GetFuncScript());
            hybridWebView.OnLoadFinished(this, EventArgs.Empty);
            hybridWebView.CanGoBack = Control.CanGoBack();
            hybridWebView.CanGoForward = Control.CanGoForward();
        }

        #endregion
        
        #region private methods

        /// <summary>
        /// Inject script to the page
        /// </summary>
        /// <param name="script">the script to execute</param>
        private void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        private string GetFuncScript()
        {
            var builder = new StringBuilder();
            builder.Append("NativeFuncs = [];");
            builder.Append("function NativeFunc(action, data, callback){");

            builder.Append("  var callbackIdx = NativeFuncs.push(callback) - 1;");
            builder.Append(NativeFuncCall);
            builder.Append("(JSON.stringify({ a: action, d: data, c: callbackIdx }));}");
            builder.Append(" if (typeof(window.NativeFuncsReady) !== 'undefined') { ");
            builder.Append("   window.NativeFuncsReady(); ");
            builder.Append(" } ");

            return builder.ToString();
        }

        #endregion
    }
}
