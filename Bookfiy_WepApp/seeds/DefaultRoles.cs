using Bookfiy_WepApp.Core.Const;
using Microsoft.AspNetCore.Identity;

namespace Bookfiy_WepApp.seeds
{
    public static class DefaultRoles
    {
        public static async Task seedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AddRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AddRoles.Archive));
                await roleManager.CreateAsync(new IdentityRole(AddRoles.Reception));
            }
        }
    }
}
