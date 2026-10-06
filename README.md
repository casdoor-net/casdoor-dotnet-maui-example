# Casdoor .NET MAUI Example

[![Build](https://github.com/casdoor-net/casdoor-dotnet-maui-example/actions/workflows/build.yml/badge.svg)](https://github.com/casdoor-net/casdoor-dotnet-maui-example/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/casdoor-net/casdoor-dotnet-maui-example)](https://github.com/casdoor-net/casdoor-dotnet-maui-example/blob/master/LICENSE)
[![Discord](https://img.shields.io/discord/1022748306096537660?logo=discord&label=discord&color=5865F2)](https://discord.gg/5rPsrAzK7S)

An example [.NET MAUI](https://dotnet.microsoft.com/apps/maui) app (Android, iOS, macOS, Windows) that signs users in with [Casdoor](https://casdoor.ai/) over OpenID Connect, with the authorization code flow and PKCE.

| Project                                                          | Description                                                                 |
|------------------------------------------------------------------|-----------------------------------------------------------------------------|
| [Casdoor.MauiOidcClient](Casdoor.MauiOidcClient)                 | A small MAUI library: `CasdoorClient` with `LoginAsync()` and `LogoutAsync()` |
| [Casdoor.MauiOidcClient.Example](Casdoor.MauiOidcClient.Example) | The app using it                                                            |

| Android | Windows |
|---------|---------|
| <img src="images/android.gif" alt="Android" height="500"/> | <img src="images/windows.gif" alt="Windows" height="300"/> |

## How it works

1. `CasdoorClient` wraps [IdentityModel.OidcClient](https://github.com/IdentityModel/IdentityModel.OidcClient). It reads the endpoints of Casdoor from `https://<Domain>/.well-known/openid-configuration`.
2. `LoginAsync()` opens the Casdoor sign-in page with a PKCE code challenge and a random state:
   - on Android, iOS and macOS in the system browser (`WebBrowserAuthenticator`, based on MAUI's `WebAuthenticator`), Casdoor redirects back to `casdoor://callback`;
   - on Windows in a `WebView` of the page (`WebViewBrowserAuthenticator`), Casdoor redirects back to `http://localhost/callback`, which the app catches instead of loading.
3. The library checks the state, exchanges the code for the tokens with the PKCE code verifier (no client secret is stored in the app) and verifies the ID token. `LoginResult.User` holds the claims of the user.
4. `LogoutAsync(idToken)` ends the Casdoor session through Casdoor's end session endpoint.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) with the MAUI workload: `dotnet workload install maui`
- The tools of your target platforms, see [.NET MAUI installation](https://learn.microsoft.com/dotnet/maui/get-started/installation)
- A Casdoor server reachable over HTTPS. The example is preconfigured for the public demo server https://door.casdoor.com, so it runs as is. To use your own, see [Casdoor installation](https://casdoor.ai/docs/basic/server-installation).

## Configuration

Skip this section to try the example with the public demo server.

In your Casdoor, create (or reuse) an organization and an application, and add `casdoor://callback` and `http://localhost/callback` to the application's **Redirect URLs**. Then fill in [MauiProgram.cs](Casdoor.MauiOidcClient.Example/MauiProgram.cs):

```csharp
builder.Services.AddSingleton(new CasdoorClient(new()
{
    Domain = "door.casdoor.com", // host of the Casdoor server
    ClientId = "014ae4bd048734ca2dea", // client ID of the application
    Scope = "openid profile email",

#if WINDOWS
    RedirectUri = "http://localhost/callback"
#else
    RedirectUri = "casdoor://callback"
#endif
}));
```

## Run

```shell
git clone https://github.com/casdoor-net/casdoor-dotnet-maui-example
cd casdoor-dotnet-maui-example
```

Windows:

```shell
dotnet build Casdoor.MauiOidcClient.Example -t:Run -f net10.0-windows10.0.19041.0
```

Android (an emulator or a device must be connected):

```shell
dotnet build Casdoor.MauiOidcClient.Example -t:Run -f net10.0-android
```

Or open `casdoor-dotnet-maui-example.sln` in Visual Studio, pick the target and press `Ctrl + F5`.

Click **Log In**. On the demo server, sign in with username `admin` and password `123`.

## Use it in your app

1. Reference the `Casdoor.MauiOidcClient` project and register `CasdoorClient` as above.
2. Call it from your page, see [MainPage.xaml.cs](Casdoor.MauiOidcClient.Example/MainPage.xaml.cs):

   ```csharp
   var loginResult = await client.LoginAsync();
   if (!loginResult.IsError)
   {
       var name = loginResult.User.Identity?.Name;
       var email = loginResult.User.FindFirst("email")?.Value;
   }
   ```

3. On Windows, put a `WebView` on the page and use it for signing in: `client.Browser = new WebViewBrowserAuthenticator(WebViewInstance);`
4. On Android, the callback scheme `casdoor` is registered by `WebAuthenticationCallbackActivity` of the library. Let the app find the browser by adding this to `Platforms/Android/AndroidManifest.xml`:

   ```xml
   <queries>
       <intent>
           <action android:name="android.support.customtabs.action.CustomTabsService" />
       </intent>
   </queries>
   ```

## Resources

- [Casdoor documentation](https://casdoor.ai/docs/overview)
- [casdoor-dotnet-sdk](https://github.com/casdoor-net/casdoor-dotnet-sdk)
- [.NET MAUI WebAuthenticator](https://learn.microsoft.com/dotnet/maui/platform-integration/communication/authentication)

## License

[Apache-2.0](LICENSE)
