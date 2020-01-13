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
</tbody>
</table>