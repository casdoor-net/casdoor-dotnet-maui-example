using IdentityModel.OidcClient;

namespace Casdoor.MauiOidcClient;

/// <summary>
/// Signs users in with Casdoor over OpenID Connect: authorization code flow with PKCE, without a client secret.
/// </summary>
public class CasdoorClient
{
    private readonly OidcClient client;

    public CasdoorClient(CasdoorClientOptions options)
    {
        client = new OidcClient(new OidcClientOptions
        {
            Authority = $"https://{options.Domain}",
            ClientId = options.ClientId,
            Scope = options.Scope,
            RedirectUri = options.RedirectUri,
            Browser = options.Browser,
            PostLogoutRedirectUri = options.RedirectUri
        });
    }

    public IdentityModel.OidcClient.Browser.IBrowser Browser
    {
        get
        {
            return client.Options.Browser;
        }
        set
        {
            client.Options.Browser = value;
        }
    }

    /// <summary>
    /// Opens the Casdoor sign-in page, then exchanges the code for the tokens and verifies the ID token.
    /// </summary>
    public async Task<LoginResult> LoginAsync()
    {
        return await client.LoginAsync();
    }

    /// <summary>
    /// Ends the Casdoor session of the user.
    /// </summary>
    /// <param name="idToken">The ID token of the sign-in, <see cref="LoginResult.IdentityToken"/>.</param>
    public async Task<LogoutResult> LogoutAsync(string idToken)
    {
        return await client.LogoutAsync(new LogoutRequest { IdTokenHint = idToken });
    }
}
