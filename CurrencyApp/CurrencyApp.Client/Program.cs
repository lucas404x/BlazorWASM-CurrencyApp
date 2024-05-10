using CurrencyApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
{
    var currentApiConfig = builder.Configuration.GetRequiredSection("CurrencyApi");
    
    var client = new HttpClient
    {
        BaseAddress = new Uri(currentApiConfig.GetValue<string>("Url") ?? "https://localhost:5002"),
    };

    client.DefaultRequestHeaders.Add("apikey", currentApiConfig.GetValue<string>("Key") ?? string.Empty);
    return client;
});
builder.Services.AddMudServices();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

await builder.Build().RunAsync();
