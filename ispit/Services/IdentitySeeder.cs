using Lab5.Models;
using Microsoft.AspNetCore.Identity;

namespace Lab5.Services;

public static class IdentitySeeder
{
    /// <summary>
    /// Ovaj email uvijek dobiva Admin ulogu, bez obzira prijavljuje li se lokalno ili preko Googlea.
    /// </summary>
    public const string DesignatedAdminEmail = "mihaelj10@gmail.com";

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in new[] { "Admin", "Manager" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Ensure a default admin user exists
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                OIB = "00000000000",
                JMBG = "0000000000000",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Ensure a default manager user exists (za testiranje Manager razine pristupa)
        var managerEmail = "manager@example.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        if (managerUser is null)
        {
            managerUser = new AppUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                OIB = "11111111111",
                JMBG = "1111111111111",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(managerUser, "Manager123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }

        // Ako je designirani admin (npr. Google račun) već registriran (prijavio se prije ove izmjene),
        // osiguraj da ima Admin ulogu i sad
        var designatedAdmin = await userManager.FindByEmailAsync(DesignatedAdminEmail);
        if (designatedAdmin != null && !await userManager.IsInRoleAsync(designatedAdmin, "Admin"))
        {
            await userManager.AddToRoleAsync(designatedAdmin, "Admin");
        }
    }
}
