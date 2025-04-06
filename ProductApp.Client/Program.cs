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

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7237/")
        });

        builder.Services.AddScoped<IProductService, ProductService>();

        builder.Services.AddMudServices();
        await builder.Build().RunAsync();
    }
}
