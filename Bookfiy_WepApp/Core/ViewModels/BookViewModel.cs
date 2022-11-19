using Bookfiy_WepApp.Core.Models;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? Author { get; set; }

        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? ImageURL { get; set; }
        public string? ImageThumnailURL { get; set; }
        public string Hall { get; set; } = null!;
        public bool IsAvailabbleForRent { get; set; }
        public string Description { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; } = null!;
    }
}
