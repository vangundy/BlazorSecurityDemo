using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;

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
                            authorizedUrls: new[] { "https://localhost:5002" },
                            scopes: new[] { "weatherapi" });

                    return handler;
                });
            
            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.DefaultScopes.Clear();
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}
