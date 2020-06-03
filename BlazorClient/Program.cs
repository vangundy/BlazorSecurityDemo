using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// See https://medium.com/@marcodesanctis2/securing-blazor-webassembly-with-identity-server-4-ee44aa1687ef

namespace BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("api")
                .AddHttpMessageHandler(sp => 
                {
                    var handler = sp.GetService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: builder.Configuration.GetSection("IdentityServer:MessageHandler:AuthorizedURLs").Get<List<string>>(),
                            scopes: builder.Configuration.GetSection("IdentityServer:MessageHandler:Scopes").Get<List<string>>()
                            );
                    return handler;
                });
            
            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.DefaultScopes.Clear();
                builder.Configuration.Bind("IdentityServer", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}
