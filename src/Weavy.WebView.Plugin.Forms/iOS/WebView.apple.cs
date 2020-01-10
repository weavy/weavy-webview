using Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Weavy.WebView.Plugin.Forms;
using Weavy.WebView.Plugin.Forms.iOS;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WeavyWebView), typeof(WeavyWebViewRenderer))]
namespace Weavy.WebView.Plugin.Forms.iOS
{
    /// <summary>
    /// iOS implementation of the weavy web view
    /// </summary>
    public class WeavyWebViewRenderer : ViewRenderer<WeavyWebView, WKWebView>, IWKScriptMessageHandler
    {
        private const string NativeFuncCall = "window.webkit.messageHandlers.native.postMessage";
        private const string NativeFunction = "function Native(action, data){window.webkit.messageHandlers.native.postMessage(JSON.stringify({ a: action, d: data }));}";

        WKUserContentController userController;

        #region private methods

        /// <summary>
        /// When element changes
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<WeavyWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(NativeFunction + GetFuncScript()), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "native");

                var config = new WKWebViewConfiguration { UserContentController = userController };

                var webView = new WKWebView(Frame, config)
                {
                    AllowsBackForwardNavigationGestures = true,
                    WeakNavigationDelegate = this,
                    AllowsLinkPreview = false,
                };
                webView.ScrollView.Bounces = false;

                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as WeavyWebView;                
            }
            if (e.NewElement != null)
            {                
                // set js handler
                e.NewElement.JavaScriptLoadRequested += (sender, js) => {
                    Inject(js);
                };

                // handle load requests
                e.NewElement.LoadRequested += (sender, args) => {
                    Request(LoadRequestComplete);
                };

                // handle go back requests
                e.NewElement.GoBackRequested += (sender, args) => {
                    if (!Control.CanGoBack) return;
                    Control.GoBack();
                };

                // handle go formward requests
                e.NewElement.GoForwardRequested += (sender, args) => {
                    if (!Control.CanGoForward) return;
                    Control.GoForward();
                };

                // handle go formward requests
                e.NewElement.ReloadRequested += (sender, args) => {                    
                    Control.Reload();
                };

                e.NewElement.OnInitFinished(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Request to load an uri
        /// </summary>
        /// <param name="completion"></param>
        private void Request(Action completion)
        {
            if (!string.IsNullOrEmpty(Element.AuthenticationToken))
            {

                var uri = new Uri(Element.BaseUrl);
                string domain = uri.Host;

                // Set cookies here
                var cookieJar = NSHttpCookieStorage.SharedStorage;
                cookieJar.AcceptPolicy = NSHttpCookieAcceptPolicy.Always;

                //clean up old cookies
                foreach (var aCookie in cookieJar.Cookies)
                {
                    cookieJar.DeleteCookie(aCookie);
                }

                //set up the new cookies
                var jCookies = Element.Cookies.GetCookies(uri);
                IList<NSHttpCookie> eCookies =
                    (from object jCookie in jCookies
                     where jCookie != null
                     select (Cookie)jCookie
                     into netCookie
                     select new NSHttpCookie(netCookie)).ToList();

                cookieJar.SetCookies(eCookies.ToArray(), uri, uri);
                                
                // create wk cookie
                WKHttpCookieStore cookieStore = Control.Configuration.WebsiteDataStore.HttpCookieStore;

                if (cookieJar.Cookies.Any())
                {
                    foreach (var c in cookieJar.Cookies)
                    {
                        cookieStore.SetCookie(c, () =>
                        {                            
                            completion();
                            return;
                            
                        });
                    }   
                }
                else
                {
                    completion();
                }
            }
            else
            {
                completion();
            }
        }


        /// <summary>
        /// Load uri 
        /// </summary>
        private void LoadRequestComplete()
        {
            Control.LoadRequest(new NSUrlRequest(new NSUrl(new Uri(Element.Uri).AbsoluteUri)));
        }

        /// <summary>
        /// Generates the script to inject into the web view
        /// </summary>
        /// <returns></returns>
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

        #region public methods

        /// <summary>
        /// A script message is received from the web view
        /// </summary>
        /// <param name="userContentController"></param>
        /// <param name="message"></param>
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (Element == null) return;

            Element.MessageReceived(message.Body.ToString());
        }
                
        /// <summary>
        /// Called when navigation is complete in the webview
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="navigation"></param>
        [Export("webView:didFinishNavigation:")]
        public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            var element = Element as WeavyWebView;

            if (element == null) return;

            // inject scripts
            Inject(NativeFunction);
            Inject(GetFuncScript());

            // call load finished on the web view
            element.OnLoadFinished(this, EventArgs.Empty);

            // set if we can navigate back/forward
            element.CanGoBack = Control.CanGoBack;
            element.CanGoForward = Control.CanGoForward;

        }


        /// <summary>
        /// inject a javascript string into to the web view
        /// </summary>
        /// <param name="script"></param>
        public void Inject(string script)
        {
            if (Control == null) return;

            InvokeOnMainThread(() => Control.EvaluateJavaScript((string)new NSString(script), (r, e) => {
                if (e !=
                    null)
                {
                    //Debug.WriteLine(e);
                }
            }));
        }

        #endregion

    }
}
