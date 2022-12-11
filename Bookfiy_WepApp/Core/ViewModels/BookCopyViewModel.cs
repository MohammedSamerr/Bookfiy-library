using Bookfiy_WepApp.Core.Models;

namespace Bookfiy_WepApp.Core.ViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }
        public string? BookTitle { get; set; }
        public bool IsAvailabbleForRent { get; set; }
        public int EdditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
