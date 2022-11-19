using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookfiy_WepApp.Core.Models
{
    [Index(nameof(Title), nameof(AuthorId), IsUnique =true)]
    public class Book 
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Title { get; set; } = null!;
        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }

        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        [MaxLength(200)]
        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? ImageURL { get; set; }
        public string? ImageThumnailURL { get; set; }
        public string? ImagePublicId { get; set; }


        [MaxLength(50)]
        public string Hall { get; set; } = null!;

        public bool IsAvailabbleForRent { get; set; }

        public string Description { get; set; } = null!;

        public ICollection<Books_Categories> Categories { get; set; } = new List<Books_Categories>();
    }
}
