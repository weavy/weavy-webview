using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Weavy.WebView.Plugin.Forms.Models;
using System.Text;
using Weavy.WebView.Plugin.Forms.Helpers;
using System.Threading.Tasks;

namespace Weavy.WebView.Plugin.Forms
{
    public class WeavyWebView : View
    {
        #region event handlers

        public EventHandler LoadFinished;
        public EventHandler Loading;
        public EventHandler LoadError;
        public EventHandler<BadgeEventArgs> BadgeUpdated;
        public EventHandler<AuthenticationEventArgs> SignInCompleted;
        public EventHandler<string> JavaScriptLoadRequested;
        internal event EventHandler GoBackRequested;
        internal event EventHandler GoForwardRequested;

        #endregion

        #region private props

        private readonly object injectLock = new object();
        private readonly Dictionary<string, Action<string>> registeredActions;
        private readonly Dictionary<string, Func<string, object[]>> registeredFunctions;
        private readonly JsonSerializer jsonSerializer;

        #endregion


        #region bindable props

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
          propertyName: "Uri",
          returnType: typeof(string),
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

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public string AuthenticationToken
        {
            get { return (string)GetValue(AuthenticationTokenProperty); }
            set { SetValue(AuthenticationTokenProperty, value); }
        }

        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }

        #endregion
        
        #region public methods

        public void GoBack()
        {
            var handler = this.GoBackRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        public void GoForward()
        {
            var handler = this.GoForwardRequested;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        public void InjectJavaScript(string script)
        {
            lock (injectLock)
            {
                JavaScriptLoadRequested?.Invoke(this, script);
            }
        }

        public void RegisterCallback(string name, Action<string> callback)
        {
            registeredActions.Add(name, callback);
        }

        public void RegisterNativeFunction(string name, Func<string, object[]> func)
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

        #region private methods

        internal void OnLoadFinished(object sender, EventArgs e)
        {

            InjectJavaScript(ScriptHelper.ScriptChecker);

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

        internal void OnSignInCompleted(object sender, AuthenticationEventArgs e)
        {
            var handler = this.SignInCompleted;
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

        private void RegisterCallbacks()
        {
            // Callback for injecting base script
            RegisterCallback("injectScriptCallback", (userGuid) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    InjectJavaScript(ScriptHelper.Scripts);

                    if (!string.IsNullOrEmpty(AuthenticationToken))
                    {
                        InjectJavaScript(ScriptHelper.SignInTokenScript(AuthenticationToken));
                        AuthenticationToken = "";
                    }
                });
            });

            RegisterCallback("signInCompleteCallback", (args) =>
            {
                var authArgs = JsonConvert.DeserializeObject<AuthenticationEventArgs>(args);
                OnSignInCompleted(this, authArgs);
            });

            // callback for theming
            //RegisterCallback("themeCallback", (color) =>
            //{
            //    //var themeColor = CrossSettings.Current.Get<string>("themecolor");
            //    var themeColor = "#000";

            //    // update theme color if different
            //    if (color != themeColor)
            //    {
            //        //SetThemeColor(color);
            //    }
            //});

            //Callback for badge update
            RegisterCallback("badgeCallback", (args) =>
            {
                //notify about badge update
                var badgeArgs = JsonConvert.DeserializeObject<BadgeEventArgs>(args);
                OnBadgeUpdate(this, badgeArgs);
            });
        }

        #endregion
               
    }
}
