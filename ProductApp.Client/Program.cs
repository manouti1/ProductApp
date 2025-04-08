using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ProductApp.Client.Services;

namespace ProductApp.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");


        // Load configuration from appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        // Get the BaseApiUrl from the configuration
        var baseApiUrl = builder.Configuration["ApiSettings:BaseApiUrl"];

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(baseApiUrl)  // Set BaseAddress dynamically
        });

        builder.Services.AddScoped<IProductService, ProductService>();

        builder.Services.AddMudServices();
        await builder.Build().RunAsync();
    }
}
