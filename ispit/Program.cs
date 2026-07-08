using Lab5.Data;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfiguracija
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "Logs/app-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);

var croatianCulture = CultureInfo.GetCultureInfo("hr-HR");
CultureInfo.DefaultThreadCurrentCulture = croatianCulture;
CultureInfo.DefaultThreadCurrentUICulture = croatianCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Register memory cache
builder.Services.AddMemoryCache();

// Register DbContext with SQLite
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatalogDbContext")));

builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
        var clientId = googleAuthSection["ClientId"];
        var clientSecret = googleAuthSection["ClientSecret"];

        // Only configure Google if credentials are provided (skip in test environment)
        if (!string.IsNullOrEmpty(clientId) && !clientId.Contains("YOUR_") &&
            !string.IsNullOrEmpty(clientSecret) && !clientSecret.Contains("YOUR_"))
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
        }
    });

builder.Services
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CatalogDbContext>();

// Register repositories
builder.Services.AddScoped<IBookCatalogService, BookCatalogService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookAccessService, BookAccessService>();

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    dbContext.Database.Migrate();
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Redirect root URL to Home controller (serve '/' as '/Home')
app.MapGet("/", () => Results.Redirect("/Home"));

app.MapStaticAssets();

// Attribute routing will be used from controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();

public partial class Program;

