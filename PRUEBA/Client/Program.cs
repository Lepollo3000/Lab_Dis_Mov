using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using PRUEBA.Client;
using PRUEBA.Client.Shared;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("PRUEBA.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PRUEBA.ServerAPI"));

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: true,
  reloadOnChange: true);

//builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

builder.Services.AddApiAuthorization();

// 1. Add the LocalStorage Services
builder.Services.AddBlazoredLocalStorage();
// 2. Add the Localization Service
builder.Services.AddLocalization();

var host = builder.Build();

// 3. Set the Default UI Culture
await host.SetDefaultUICulture();

await builder.Build().RunAsync();
