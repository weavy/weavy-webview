using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xamarin.Forms.Platform.Android;

namespace Weavy.WebView.Plugin.Forms.Droid
{
    /// <summary>
    /// Custom web view client implementation
    /// </summary>
    public class WeavyWebChromeClient : WebChromeClient
    {
        Activity _activity;                
        List<int> _requestCodes;

        public override bool OnShowFileChooser(global::Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);
            return ChooseFile(filePathCallback, fileChooserParams.CreateIntent(), fileChooserParams.Title);
        }

        public static void HandleActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            WeavyActivityResultCallbackRegistry.InvokeCallback(requestCode, resultCode, data);
        }

        protected bool ChooseFile(IValueCallback filePathCallback, Intent intent, string title)
        {
            if (_activity == null)
                return false;

            Action<Result, Intent> callback = (resultCode, intentData) =>
            {
                if (filePathCallback == null)
                    return;

                Android.Net.Uri[] result = ParseResult(resultCode, intentData);
                filePathCallback.OnReceiveValue(result);
            };

            _requestCodes = _requestCodes ?? new List<int>();

            int newRequestCode = WeavyActivityResultCallbackRegistry.RegisterActivityResultCallback(callback);

            _requestCodes.Add(newRequestCode);

            _activity.StartActivityForResult(Intent.CreateChooser(intent, title), newRequestCode);

            return true;
        }

        protected static Android.Net.Uri[] ParseResult(Result resultCode, Intent data)
        {
            return FileChooserParams.ParseResult((int)resultCode, data);
        }

        internal void SetContext(Context thisActivity)
        {
            _activity = thisActivity as Activity;            
        }
    }    
}
