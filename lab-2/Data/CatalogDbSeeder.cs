using Lab2.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data;

public static class CatalogDbSeeder
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (await context.Authors.AnyAsync())
        {
            return;
        }

        var store = new CatalogMockStore();
        await context.Authors.AddRangeAsync(store.Authors);
        await context.Publishers.AddRangeAsync(store.Publishers);
        await context.Genres.AddRangeAsync(store.Genres);
        await context.Users.AddRangeAsync(store.Users);
        await context.Books.AddRangeAsync(store.Books);
        await context.BookGenres.AddRangeAsync(store.BookGenres);
        await context.Reviews.AddRangeAsync(store.Reviews);
        await context.SaveChangesAsync();
    }
}
