using IdentityModel.OidcClient.Browser;

namespace Casdoor.MauiOidcClient;

/// <summary>
/// Shows the sign-in page in a WebView of the app, for the platforms without a system browser flow (Windows).
/// </summary>
public class WebViewBrowserAuthenticator : IdentityModel.OidcClient.Browser.IBrowser
{
    private readonly WebView webView;

    public WebViewBrowserAuthenticator(WebView webView)
    {
        this.webView = webView;
    }

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<BrowserResult>();

        // the redirect to the end URL (the callback) carries the result, it is caught instead of being loaded
        void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith(options.EndUrl, StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                tcs.TrySetResult(new BrowserResult
                {
                    ResultType = BrowserResultType.Success,
                    Response = e.Url
                });
            }
        }

        using var registration = cancellationToken.Register(() => tcs.TrySetResult(new BrowserResult
        {
            ResultType = BrowserResultType.UserCancel,
            ErrorDescription = "Login canceled by the user."
        }));

        // some platforms report the redirect only after trying to load it
        void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.StartsWith(options.EndUrl, StringComparison.OrdinalIgnoreCase))
            {
                tcs.TrySetResult(new BrowserResult
                {
                    ResultType = BrowserResultType.Success,
                    Response = e.Url
                });
            }
        }

        webView.Navigating += OnNavigating;
        webView.Navigated += OnNavigated;
        webView.WidthRequest = 600;
        webView.HeightRequest = 600;
        webView.Source = new UrlWebViewSource { Url = options.StartUrl };

        try
        {
            return await tcs.Task;
        }
        finally
        {
            webView.Navigating -= OnNavigating;
            webView.Navigated -= OnNavigated;
            webView.WidthRequest = 0;
            webView.HeightRequest = 0;
        }
    }
}
