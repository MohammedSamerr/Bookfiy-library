namespace Bookfiy_WepApp.Core.Models
{
    public class BookCopy
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public bool IsAvailabbleForRent { get; set; }
        public int EdditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdateOn { get; set; }
    }
}
