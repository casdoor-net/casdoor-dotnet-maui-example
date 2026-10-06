using Microsoft.Extensions.Logging;

namespace Casdoor.MauiOidcClient.Example
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		    builder.Logging.AddDebug();
#endif
            // The Casdoor application to sign in with, the defaults are the public demo server https://door.casdoor.com
            builder.Services.AddSingleton(new CasdoorClient(new()
            {
                Domain = "door.casdoor.com",
                ClientId = "014ae4bd048734ca2dea",
                Scope = "openid profile email",

#if WINDOWS
			RedirectUri = "http://localhost/callback"
#else
                RedirectUri = "casdoor://callback"
#endif
            }));
            builder.Services.AddSingleton<MainPage>();
            return builder.Build();
        }
    }
}