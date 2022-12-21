using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Bookfiy_WepApp.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : BaseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        
    }
}
