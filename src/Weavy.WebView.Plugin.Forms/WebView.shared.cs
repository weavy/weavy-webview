using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Weavy.WebView.Plugin.Forms.Models;
using System.Text;
using Weavy.WebView.Plugin.Forms.Helpers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Net;

namespace Weavy.WebView.Plugin.Forms
{
    public static class WeavyWebViewConfiguration
    {
        public static string AuthenticationToken { get; set; }
    }

    public class WeavyWebView : View
    {
        #region event handlers
        public static CookieContainer CookieJar = new CookieContainer();
        public EventHandler InitCompleted;
        public EventHandler LoadFinished;
        public EventHandler Loading;
        public EventHandler LoadError;
        public EventHandler SSOError;
        public EventHandler<BadgeEventArgs> BadgeUpdated;        
        public EventHandler SignedOut;
        public EventHandler<ThemingEventArgs> Theming;
        public EventHandler<LinkEventArgs> LinkClicked;
        public EventHandler<string> JavaScriptLoadRequested;
        internal event EventHandler GoBackRequested;
        internal event EventHandler GoForwardRequested;
        internal event EventHandler LoadRequested;
        internal event EventHandler ReloadRequested;

        #endregion

        #region private props

        private readonly object injectLock = new object();
        private readonly Dictionary<string, Action<string>> registeredActions;
        private readonly Dictionary<string, Func<string, object[]>> registeredFunctions;
        
        #endregion

        #region bindable props

        public static readonly BindableProperty CookiesProperty = BindableProperty.Create(
            propertyName: "Cookies",
            returnType: typeof(CookieContainer),
            declaringType: typeof(WeavyWebView),
            defaultValue: default(string));

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
          propertyName: "Uri",
          returnType: typeof(string),
          declaringType: typeof(WeavyWebView),
          defaultValue: default(string));
                
        public static readonly BindableProperty HeadersProperty = BindableProperty.Create(
          propertyName: "Headers",
          returnType: typeof(List<KeyValuePair<string, string>>),
          declaringType: typeof(WeavyWebView),
          defaultValue: default(string));

        public static readonly BindableProperty AuthenticationTokenProperty = BindableProperty.Create(
          propertyName: "AuthenticationToken",
          returnType: typeof(string),
          declaringType: typeof(WeavyWebView),
          defaultValue: default(string));

        public static readonly BindableProperty CanGoBackProperty = BindableProperty.Create(
         propertyName: "CanGoBack",
         returnType: typeof(bool),
         declaringType: typeof(WeavyWebView),
         defaultValue: default(bool));

        public static readonly BindableProperty CanGoForwardProperty = BindableProperty.Create(
          propertyName: "CanGoForward",
          returnType: typeof(bool),
          declaringType: typeof(WeavyWebView),
          defaultValue: default(bool));

        #endregion

        public WeavyWebView()
        {            
            Cookies = new CookieContainer();

            // make sure the web view fills its container
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            // setup dictionaries for registered actions and functions
            registeredActions = new Dictionary<string, Action<string>>();
            registeredFunctions = new Dictionary<string, Func<string, object[]>>();

            // register all script callbacks that should be handled
            RegisterCallbacks();
        }

        #region public props

        public CookieContainer Cookies
        {
            get { return (CookieContainer)GetValue(CookiesProperty); }
            set { SetValue(CookiesProperty, value); }
        }

        /// <summary>
        /// The uri to the Weavy page that should be displayed in the Weavy Web View
        /// </summary>
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set
            { SetValue(UriProperty, value); }
        }
        
        /// <summary>
        /// A JWT token that will be passed along to Weavy. 
        /// A valid JWT token enables the SSO authentication flow and the user will be signed in automatically.
        /// For more information on how to setup, goto https://docs.weavy.com/sdk/server/authentication/external#the-json-web-token-(jwt)
        /// </summary>
        public string AuthenticationToken
        {
            get { return (string)GetValue(AuthenticationTokenProperty); }
            set
            { SetValue(AuthenticationTokenProperty, value); }
        }

        /// <summary>
        /// True if you can go back in the Weavy Web View
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        /// <summary>
        /// True if you can go forward in the Weavy Web View
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }

        public string BaseUrl => new Uri(Uri).GetLeftPart(UriPartial.Authority);

        #endregion

        #region public methods

        /// <summary>
        /// Load the Weavy Web View with the specified uri and authentication token. 
        /// </summary>
        public void Load(string uri = null, string authenticationToken = null)
        {
            AuthenticationToken = authenticationToken ?? AuthenticationToken;

            Uri = uri ?? Uri;

            if (!string.IsNullOrEmpty(AuthenticationToken))
            {
                HandleAuthentication(AuthenticationToken);
            }

            var handler = this.LoadRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Reload the Weavy Web View. 
        /// </summary>
        public void Reload()
        {
            var handler = this.ReloadRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Go back in the Weavy Web View. You can check <see cref="CanGoBack"/> if it's possible to go back.
        /// </summary>
        public void GoBack()
        {
            var handler = this.GoBackRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Go forward in the Weavy Web View. You can check <see cref="CanGoForward"/> if it's possible to go forward.
        /// </summary>
        public void GoForward()
        {
            var handler = this.GoForwardRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Get the current user
        /// </summary>
        public void GetUser(Action<string> callback)
        {
            RegisterCallback("userCallback", callback);
            CallJsFunction("weavyAppScripts.user.get");
        }

        /// <summary>
        /// Resume web view. Updates badge and connectin state
        /// </summary>
        public void Resume()
        {
            InjectJavaScript(ScriptHelper.UpdateBadgeScript);
        }

        /// <summary>
        /// Inject a script snippet into the Weavy Web View that will be executed.
        /// </summary>
        /// <param name="script"></param>
        public void InjectJavaScript(string script)
        {
            lock (injectLock)
            {
                JavaScriptLoadRequested?.Invoke(this, script);
            }
        }

        /// <summary>
        /// Register a callback methods that will be run when called from javascript. Use Native('[nameofcallback]', data) when adding script through <see cref="InjectJavaScript(string)"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterCallback(string name, Action<string> callback)
        {
            if (registeredActions.ContainsKey(name))
            {
                registeredActions.Remove(name);
            }

            registeredActions.Add(name, callback);
        }

        private void RegisterNativeFunction(string name, Func<string, object[]> func)
        {
            this.registeredFunctions.Add(name, func);
        }

        /// <summary>
        /// A message is received from the web page. Try call the C# method or function
        /// </summary>
        /// <param name="message"></param>
        public void MessageReceived(string message)
        {
            var m = JsonConvert.DeserializeObject<Message>(message);

            if (m == null || m.a == null) return;

            if (TryGetAction(m.a, out Action<string> action))
            {
                action.Invoke(m.d.ToString());
                return;
            }

            if (TryGetFunc(m.a, out Func<string, object[]> func))
            {
                Task.Run(() =>
                {
                    var result = func.Invoke(m.d.ToString());
                    CallJsFunction(string.Format("NativeFuncs[{0}]", m.c), result);
                });
            }
        }

        /// <summary>
        /// Call a javascript function in the web page
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="parameters"></param>
        public void CallJsFunction(string funcName, params object[] parameters)
        {
            var builder = new StringBuilder();

            builder.Append(funcName);
            builder.Append("(");

            for (var n = 0; n < parameters.Length; n++)
            {
                builder.Append(JsonConvert.SerializeObject(parameters[n]));
                if (n < parameters.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");

            InjectJavaScript(builder.ToString());
        }

        #endregion

        #region private and internal methods

        internal void OnInitFinished(object sender, EventArgs e)
        {
            var handler = this.InitCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnLoadFinished(object sender, EventArgs e)
        {
            // inject the scripts 
            InjectJavaScript(ScriptHelper.Scripts);

            var handler = this.LoadFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnLoading(object sender, EventArgs e)
        {
            var handler = this.Loading;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnLoadError(object sender, EventArgs e)
        {
            var handler = this.LoadError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnBadgeUpdate(object sender, BadgeEventArgs e)
        {
            var handler = this.BadgeUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnSignedOut(object sender, EventArgs e)
        {
            var handler = this.SignedOut;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnTheming(object sender, ThemingEventArgs e)
        {
            var handler = this.Theming;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnLinkClicked(object sender, LinkEventArgs e)
        {
            var handler = this.LinkClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal bool TryGetAction(string name, out Action<string> action)
        {
            return this.registeredActions.TryGetValue(name, out action);
        }

        internal bool TryGetFunc(string name, out Func<string, object[]> func)
        {
            return this.registeredFunctions.TryGetValue(name, out func);
        }


        private void HandleAuthentication(string token)
        {            
            var baseUrl = new Uri(BaseUrl);

            // hadler with cookie container
            HttpClientHandler httpHandler = new HttpClientHandler
            {
                CookieContainer = CookieJar
            };

            // use token to get a new auth cookie
            using var client = new HttpClient(httpHandler)
            {
                BaseAddress = baseUrl
            };
            var content = new StringContent($"{{jwt: '{token}' }}", Encoding.UTF8, "application/json");
            var result = client.PostAsync("/sign-in-token", content).Result;

            // get cookies 
            IEnumerable<Cookie> responseCookies = CookieJar.GetCookies(baseUrl).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                Cookies.Add(baseUrl, cookie);                
            }

            if (!result.IsSuccessStatusCode)
            {
                var handler = this.SSOError;
                if (handler != null)
                {
                    handler(this, new EventArgs());
                }

            }
        }
        private void RegisterCallbacks()
        { 

            //Callback from sign out script
            RegisterCallback("signOutCallback", (args) =>
            {
                OnSignedOut(this, EventArgs.Empty);
            });


            // callback for theming
            RegisterCallback("themeCallback", (theme) =>
            {
                OnTheming(this, JsonConvert.DeserializeObject<ThemingEventArgs>(theme));
            });

            //Callback for badge update
            RegisterCallback("badgeCallback", (args) =>
            {
                //notify about badge update
                var badgeArgs = JsonConvert.DeserializeObject<BadgeEventArgs>(args);
                OnBadgeUpdate(this, badgeArgs);
            });

            //Callback when a link has been clicked
            RegisterCallback("linkCallback", (args) =>
            {
                //notify about link clicked                
                var linkArgs = JsonConvert.DeserializeObject<LinkEventArgs>(args);
                OnLinkClicked(this, linkArgs);
            });

            //Callback when page is readt
            RegisterCallback("readyCallback", (args) =>
            {
                //notify about page ready                                
                OnLoadFinished(this, EventArgs.Empty);
            });

        }

        #endregion

    }
}
