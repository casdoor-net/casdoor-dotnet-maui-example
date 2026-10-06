namespace Casdoor.MauiOidcClient;

public class CasdoorClientOptions
{
    public CasdoorClientOptions()
    {
        Scope = "openid";
        RedirectUri = "casdoor://callback";
        Browser = new WebBrowserAuthenticator();
    }

    /// <summary>
    /// Host of the Casdoor server, without the scheme, for example door.casdoor.com. HTTPS is used.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Client ID of the Casdoor application.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Where Casdoor redirects back after signing in, must be in the Redirect URLs of the application.
    /// </summary>
    public string RedirectUri { get; set; }

    public string Scope { get; set; }

    /// <summary>
    /// How the sign-in page is shown: <see cref="WebBrowserAuthenticator"/> (the system browser, default)
    /// or <see cref="WebViewBrowserAuthenticator"/> (a WebView of the app).
    /// </summary>
    public IdentityModel.OidcClient.Browser.IBrowser Browser { get; set; }
}
