using BlazorAppClient;
using BlazorAppClient.MapperProfiles;
using BlazorAppClient.Service;
using BlazorAppClient.Service.ErrorHelper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp =>
{
    var handler = new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new System.Net.CookieContainer()
    };
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7063")
    };
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7063");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("ApiClient"));


builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IDialogService, DialogService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
           .RequireAuthenticatedUser()
           .Build();
});

await builder.Build().RunAsync();