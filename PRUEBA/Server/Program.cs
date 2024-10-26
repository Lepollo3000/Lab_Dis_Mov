using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PRUEBA.Server.Data;
using PRUEBA.Server.Hubs;
using PRUEBA.Server.Models;
using PRUEBA.Server.Options;
using PRUEBA.Server.Services;
using PRUEBA.Shared;
using System.IdentityModel.Tokens.Jwt;

using static System.Environment;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    });

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

builder.Services
    .AddAuthentication()
    .AddIdentityServerJwt()
    .AddGoogle(options =>
    {
        IConfigurationSection googleSection = builder.Configuration
            .GetSection("Authentication:Google");

        options.ClientId = googleSection
            .GetValue<string>("ClientId") ?? string.Empty;
        options.ClientSecret = googleSection
            .GetValue<string>("ClientSecret") ?? string.Empty;
    });

builder.Services
    .AddSingleton<TwilioService>();

builder.Services
        .AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        })
        .AddMessagePackProtocol();

builder.Services
   .Configure<TwilioSettings>(options =>
    {
        var twilio = builder.Configuration.GetSection("Twilio");

        options.AccountSid = twilio.GetValue<string?>("TwilioAccountSid");
        options.ApiSecret = twilio.GetValue<string?>("TwilioApiSecret");
        options.ApiKey = twilio.GetValue<string?>("TwilioApiKey");
    });

builder.Services
    .AddResponseCompression(options =>
    {
        options.MimeTypes = ResponseCompressionDefaults
            .MimeTypes.Concat(["application/octet-stream"]);

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddControllersWithViews();
builder.Services
    .AddRazorPages();
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles(new StaticFileOptions
{
    HttpsCompression = HttpsCompressionMode.Compress,
    OnPrepareResponse = context =>
        context.Context.Response.Headers[HeaderNames.CacheControl] =
            $"public,max-age={86_400}"
});

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");
app.MapHub<NotificationHub>(HubEndpoints.NotificationHub);

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
