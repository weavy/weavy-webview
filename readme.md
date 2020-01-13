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