# Weavy WebView

Weavy WebView for Xamarin.Forms. A view for displaying [Weavy](https://weavy.com) content in your app.

## Quick start

```
xmlns:w="clr-namespace:Weavy.WebView.Plugin.Forms;assembly=Weavy.WebView.Plugin.Forms"

...

<w:WeavyWebView 
    x:Name="weavyWebView" 
    Uri="https://myweavy.weavycloud.com"
    VerticalOptions="FillAndExpand" 
    HorizontalOptions="FillAndExpand">
</w:WeavyWebView>
```
Figure 1. *Adding a Weavy WebView in a .xaml view*


```
var weavyWebView = new WeavyWebView{
    Uri = "https://myweavy.weavycloud.com"
};
```
Figure 2. *Adding a Weavy WebView in code*

```
weavyWebView.InitCompleted += (s, a) => { 
    weavyWebView.Load(); 
};
```
Figure 3. *Load the initial request when the WebView is initiated*

## Weavy WebView properties
| Property | Description |
|----------|-------------|
|`Uri` | The uri that the WebView should display|
| `AuthenticationToken` | A JWT authentication token to enable SSO. For more information, please check out the [Weavy Docs](docs.weavy.com/sdk/server/authentication/external#the-json-web-token-(jwt)) |
| `CanGoBack`  |If the WebView can go back in history  |
| `CanGoForward` | If the WebView can go forward in history |

## Weavy WebView methods
| Method | Description |
|----------|-------------|
| `Load(string uri = null, string authenticationToken = null)` | Load and display the Uri in the WebView. Optionally pass in  uri and authenticationToken. Otherwise these will be read from the properties `Uri` and `AuthenticationToken` |
| `Reload()` | Reload the current url in the WebView |
| `GoBack()` | Go back one step in page history |
| `GoForward()` | Go forward one step in page history |
| `GetUser(Action<string> callback)` | Get the current logged in user. A json string of the user is returned to the callback |
| `RegisterCallback(string name, Action<string> callback)` | Register a custom callback with a specific name and the callback that should be called from javascript |
| `InjectJavaScript(string script)`  | Inject a custom javascript to the WebView. If you want to call a callback that you have registered (check out method above) you shoud call this in the script like this `Native('myCallbackName', myArgs)` |


## Weavy WebView events
| Event | Description |
|----------|-------------|
| `InitCompleted` | When the WebView is initiated and ready to load a uri. Listen to this event before Loading the `Uri` using `Load()` |
| `Loading` | The Weavy WebView is loading the requested `Uri` |
| `LoadFinished` | The Weavy WebView has finished loading the `Uri` |
| `LoadError` | An error occurred when loading the `Uri` |
| `SSOError` | If `AuthenticationToken` is specified and an error occurred when using the token, this event will be triggered. |
| `BadgeUpdated` | The Weavy badge is updated. Either a new message or a new notification. You can get the badge values from the `BadgeEventArgs` parameter. |
| `Theming` | The Weavy web site has a theme. This will be retured on each request and triggers the Theming event. You can get the themeing values from the `ThemingEventArgs` parameter. |
| `LinkClicked` | An external link has been clicked in the Weavy WebView. If you want to open extrnal links in the mobile web browser, you can listen for this event and call `Launcher.OpenAsync(new Uri(args.Url));` in Xamarin.Forms. The url can be read from the `LinkEventArgs`. |
| `SignedOut` | Triggered when the user signs out from Weavy. |


## Using SSO

If you are using this plugin in an existing app you have most certainly already an authentication flow for the users. Weavy uses JWT to enable a SSO flow between the host app and the Weavy integration. You can read more about how JWT works with Weavy in the [Weavy Docs](docs.weavy.com/sdk/server/authentication/external#the-json-web-token-(jwt)). 

When you have generated the JWT according to the specs in the Docs, you can pass this to a Weavy WebView and the user will automatically be signed in.

```
var weavyWebView = new WeavyWebView{
    Uri = "https://myweavy.weavycloud.com/e/apps/10",
    AuthenticationToken = myGeneratedJWT
};

weavyWebView.InitCompleted += (s, a) => { weavyWebView.Load(); };
```
Figure 4. *Add a Weavy WebView with an AuthenticationToken.*

## Sample app

A sample app is included which shows the most common usages of the Weavy WebView. 