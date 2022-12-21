using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookfiy_WepApp.seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = new()
            {
                UserName ="admin",
                Email = "admin@bookify.com",
                FullName = "Admin",
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(admin.Email);
            if (user is null)
            {
                await userManager.CreateAsync(admin,"Admin@123");
                await userManager.AddToRoleAsync(admin, AddRoles.Admin);
            }
        }
    }
}
