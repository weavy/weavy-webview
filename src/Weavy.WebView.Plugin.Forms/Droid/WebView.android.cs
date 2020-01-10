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
        WebViewClient _webViewClient;
        WeavyWebChromeClient _webChromeClient;
        protected internal WeavyWebView ElementController => Element;
        bool _isDisposed = false;

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

                _webViewClient = GetWebViewClient();
                webView.SetWebViewClient(_webViewClient);
                
                _webChromeClient = new WeavyWebChromeClient();
                _webChromeClient.SetContext(Context);
                webView.SetWebChromeClient(_webChromeClient);

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
                var newElementController = e.NewElement as WeavyWebView;

                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                //Control.LoadUrl(Element.Uri);

                // handle load requests
                newElementController.LoadRequested += OnLoadRequested;

                // handle reload requests
                newElementController.ReloadRequested += OnReloadRequested;

                // handle javascript injection requests
                newElementController.JavaScriptLoadRequested += OnJavaScriptLoadRequested;

                // handle go back requests
                newElementController.GoBackRequested += OnGoBackRequested;

                // handle go formward requests
                newElementController.GoForwardRequested += OnGoForwardRequested;

                newElementController.OnInitFinished(this, EventArgs.Empty);
            }
        }

        void OnLoadRequested(object sender, EventArgs args)
        {
            LoadRequest();
        }

        void OnJavaScriptLoadRequested(object sender, string script)
        {
            InjectJS(script);
        }

        void OnGoBackRequested(object sender, EventArgs args)
        {
            if (!Control.CanGoBack()) return;

            Control.GoBack();

            UpdateCanGoBackForward();
        }

        void OnGoForwardRequested(object sender, EventArgs args)
        {
            if (!Control.CanGoForward()) return;

            Control.GoForward();

            UpdateCanGoBackForward();
        }

        void OnReloadRequested(object sender, EventArgs args)
        {
            Control.Reload();
        }

        /// <summary>
        /// Load an url
        /// </summary>
        private void LoadRequest()
        {
            if (Element == null) return;

            var cookieManager = CookieManager.Instance;
            cookieManager.SetAcceptCookie(true);
            cookieManager.SetAcceptThirdPartyCookies(Control, true);
            cookieManager.RemoveAllCookie();
            var cookies = ElementController.Cookies.GetCookies(new System.Uri(ElementController.BaseUrl));
            for (var i = 0; i < cookies.Count; i++)
            {
                string cookieValue = cookies[i].Value;
                string cookieDomain = cookies[i].Domain;
                string cookieName = cookies[i].Name;
                cookieManager.SetCookie(cookieDomain, cookieName + "=" + cookieValue);
            }

            Control.LoadUrl(Element.Uri);
        }

        /// <summary>
        /// Dispose web view
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            if (disposing)
            {
                if (Element != null)
                {
                    Control?.StopLoading();

                    ElementController.JavaScriptLoadRequested -= OnJavaScriptLoadRequested;                    
                    ElementController.GoBackRequested -= OnGoBackRequested;
                    ElementController.GoForwardRequested -= OnGoForwardRequested;
                    ElementController.LoadRequested-= OnLoadRequested;
                    ElementController.ReloadRequested -= OnReloadRequested;
                }
            }

            base.Dispose(disposing);
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

        /// <summary>
        /// Check if we can go back or forward in the webview
        /// </summary>
        protected internal void UpdateCanGoBackForward()
        {
            if (Element == null || Control == null)
                return;
            ElementController.CanGoBack = Control.CanGoBack();
            ElementController.CanGoForward = Control.CanGoForward();
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
