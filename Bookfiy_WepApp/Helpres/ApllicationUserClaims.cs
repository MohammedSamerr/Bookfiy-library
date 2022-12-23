using Bookfiy_WepApp.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Bookfiy_WepApp.Helpres
{
    public class ApllicationUserClaims : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApllicationUserClaims(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            return identity;
        }
    }
}
