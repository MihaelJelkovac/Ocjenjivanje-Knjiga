using Lab5.Data;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfiguracija - koristi appsettings (Console za Production)
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
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

var authBuilder = builder.Services.AddAuthentication();

var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
var googleClientId = googleAuthSection["ClientId"];
var googleClientSecret = googleAuthSection["ClientSecret"];

// Only configure Google if credentials are provided (skip in test/production without keys)
if (!string.IsNullOrEmpty(googleClientId) && !googleClientId.Contains("YOUR_") &&
    !string.IsNullOrEmpty(googleClientSecret) && !googleClientSecret.Contains("YOUR_"))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

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

// Register AI Service
var anthropicApiKey = builder.Configuration["Anthropic:ApiKey"] ?? "dummy-key-for-now";
builder.Services.AddScoped<IAIService>(sp =>
    new AIService(anthropicApiKey, sp.GetRequiredService<ILogger<AIService>>())
);

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

