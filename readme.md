# Weavy WebView

Weavy WebView for Xamarin.Forms. A view for displaying Weavy content in your app.

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
<em>Adding a Weavy WebView in a .xaml view</em>


```
var weavyWebView = new WeavyWebView{
    Uri = "https://myweavy.weavycloud.com"
};
```
<em>Adding a Weavy WebView in code</em>

```
weavyWebView.InitComplete += (s, a) => { 
    weavyWebView.Load(); 
};
```
<em>Load the initial request when the WebView is initiated</em>

## Weavy WebView properties
<table>
<thead>
    <tr>
        <th>Property</th>
        <th>Description</th>
    </tr>
</thead>
<tbody>
    <tr>
        <td>Uri</td>
        <td>The uri that the WebView should display</td>
    </tr>
    <tr>
        <td>AuthenticationToken</td>
        <td>A JWT authentication token to enable SSO. For more information, please check out the [Weavy Docs](https://docs.weavy.com/sdk/server/authentication/external#the-json-web-token-(jwt)), </td>
    </tr>
    <tr>
        <td>CanGoBack</td>
        <td>If the WebView can go back in history</td>
    </tr>
    <tr>
        <td>CanGoForward</td>
        <td>If the WebView can go forward in history</td>
    </tr>
</tbody>
</table>

## Weavy WebView methods
<table>
<thead>
    <tr>
        <th>Method</th>
        <th>Description</th>
    </tr>
</thead>
<tbody>
    <tr>
        <td>Load(string uri = null, string authenticationToken = null)</td>
        <td>Load and display the Uri in the WebView. Optionally pass in  uri and authenticationToken. Otherwise these will be read from the Properties</td>
    </tr>
    <tr>
        <td>Reload</td>
        <td>Reload the current url in the WebView</td>
    </tr>
    <tr>
        <td>GoBack()</td>
        <td>Go back one step in page history</td>
    </tr>
    <tr>
        <td>GoForward()</td>
        <td>Go forward one step in page history</td>
    </tr>
    <tr>
        <td>GetUser(Action<string> callback)</td>
        <td>Get the current logged in user. A json string of the user is returned to the callback</td>
    </tr>
    <tr>
        <td>RegisterCallback(string name, Action<string> callback)</td>
        <td>Register a custom callback with a specific name and the callback that should be called from javascript</td>
    </tr>
    <tr>
        <td>InjectJavaScript(string script)</td>
        <td>Inject a custom javascript to the WebView. If you want to call a callback that you have registered (check out method above) you shoud call this in the script like this <code>Native('myCallbackName', myArgs)</code></td>
    </tr>
</tbody>
</table>


## Weavy WebView events
<table>
<thead>
    <tr>
        <th>Event</th>
        <th>Description</th>
    </tr>
</thead>
<tbody>
    <tr>
        <td>InitComplete</td>
        <td>When the WebView is initiated and ready to load a uri. Listen to this event before Loading the uri using <code>Load()</code></td>
    </tr>
    <tr>
        <td>Loading</td>
        <td>The Weavy WebView is loading the requested uri</td>
    </tr>
    <tr>
        <td>LoadFinished</td>
        <td>The Weavy WebView has finished loading the uri</td>
    </tr>
    <tr>
        <td>LoadError</td>
        <td>An error occurred when loading the uri</td>
    </tr>
    <tr>
        <td>SSOError</td>
        <td>If <code>AuthenticationToken</code> is specified and an error occurred when using the token, this event will trigger</td>
    </tr>
    <tr>
        <td>BadgeUpdated</td>
        <td>The Weavy badge is updated. Either a new message or a new notification. You can get the badge values from the <code>BadgeEventArgs</code> parameter.</td>
    </tr>
    <tr>
        <td>Theming</td>
        <td>The Weavy web site has a theme. This will be retured on each request and triggers the Theming event. You can get the themeing values from the <code>ThemingEventArgs</code> parameter.</td>
    </tr>
    <tr>
        <td>LinkClicked</td>
        <td>An external link has been clicked in the Weavy WebView. If you want to open extrnal links in the mobile web browser, you can listen for this event and call <code>Launcher.OpenAsync(new Uri(args.Url));</code> in Xamarin.Forms. The url can be read from the <code>LinkEventArgs</code>.</td>
    </tr>
    <tr>
        <td>SignedOut</td>
        <td>Triggered when the user signs out from Weavy.</td>
    </tr>
</tbody>
</table>