using Lab4.Data;
using Lab4.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var croatianCulture = CultureInfo.GetCultureInfo("hr-HR");
CultureInfo.DefaultThreadCurrentCulture = croatianCulture;
CultureInfo.DefaultThreadCurrentUICulture = croatianCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register memory cache
builder.Services.AddMemoryCache();

// Register DbContext with SQLite
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatalogDbContext")));

// Register repositories
builder.Services.AddScoped<IBookCatalogService, BookCatalogService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Redirect root URL to Home controller (serve '/' as '/Home')
app.MapGet("/", () => Results.Redirect("/Home"));

app.MapStaticAssets();

// Attribute routing will be used from controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
