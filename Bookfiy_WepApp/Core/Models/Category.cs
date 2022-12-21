using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Core.Models
{
    [Index(nameof(Name) , IsUnique = true)]
    public class Category : BaseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;


        public ICollection<Books_Categories> Books { get; set; } = new List<Books_Categories>();
    }
}
