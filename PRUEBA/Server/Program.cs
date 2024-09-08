using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using PRUEBA.Server.Data;
using PRUEBA.Server.Models;

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
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

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
    .AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddControllersWithViews();
builder.Services
    .AddRazorPages();

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
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
