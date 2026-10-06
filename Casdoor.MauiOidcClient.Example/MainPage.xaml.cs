namespace Casdoor.MauiOidcClient.Example
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly CasdoorClient client;
        private string idToken;

        public MainPage(CasdoorClient client)
        {
            InitializeComponent();
            this.client = client;

#if WINDOWS
            // Windows has no system browser flow, show the sign-in page in the WebView of the page
            client.Browser = new WebViewBrowserAuthenticator(WebViewInstance);
#endif
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var loginResult = await client.LoginAsync();
            if (loginResult.IsError)
            {
                await DisplayAlertAsync("Error", loginResult.ErrorDescription ?? loginResult.Error, "OK");
                return;
            }

            // the ID token ends the Casdoor session on logout
            idToken = loginResult.IdentityToken;
            NameLabel.Text = loginResult.User.Identity?.Name
                ?? loginResult.User.FindFirst("preferred_username")?.Value
                ?? loginResult.User.FindFirst("name")?.Value;
            EmailLabel.Text = loginResult.User.FindFirst("email")?.Value;

            LoginView.IsVisible = false;
            HomeView.IsVisible = true;
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var logoutResult = await client.LogoutAsync(idToken);
            if (logoutResult.IsError)
            {
                await DisplayAlertAsync("Error", logoutResult.ErrorDescription ?? logoutResult.Error, "OK");
                return;
            }

            idToken = null;
            HomeView.IsVisible = false;
            LoginView.IsVisible = true;
        }
    }
}
