using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Core.Models
{
    [Index(nameof(Email) , IsUnique= true)]
    [Index(nameof(UserName) , IsUnique= true)]
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        public string? CreatedById { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }

        public string? LastUpdateById { get; set; }
    }
}
