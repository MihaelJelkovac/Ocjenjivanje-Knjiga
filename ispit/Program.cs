using Lab5.Data;
using Lab5.Mcp;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfiguracija - appsettings.Production.json definira Console sink;
// Development/base appsettings nemaju Serilog sekciju pa dobivaju Console fallback + file sink.
const string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();

    if (!context.Configuration.GetSection("Serilog").Exists())
    {
        configuration.WriteTo.Console(outputTemplate: logOutputTemplate);
    }

    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration.WriteTo.File(
            path: "Logs/app-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: logOutputTemplate);
    }
});

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
builder.Services.AddScoped<IGlobalSearchService, GlobalSearchService>();

// Register MCP server - izlaže katalog knjiga kao alate dostupne kroz agentic IDE (npr. Claude Code)
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Register AI Service
builder.Services.AddHttpClient();
var mistralApiKey = builder.Configuration["Mistral:ApiKey"] ?? "dummy-key-for-now";
builder.Services.AddScoped<IAIService>(sp =>
    new AIService(
        mistralApiKey,
        sp.GetRequiredService<ILogger<AIService>>(),
        sp.GetRequiredService<IHttpClientFactory>().CreateClient())
);

var app = builder.Build();

// Railway (i slični PaaS provideri) terminiraju HTTPS na edge-u i prosljeđuju zahtjeve
// interno preko HTTP-a - bez ovoga app misli da je zahtjev http:// pa generira pogrešan
// (http umjesto https) redirect_uri za Google OAuth.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

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

app.MapMcp("/mcp");

app.Run();

public partial class Program;

