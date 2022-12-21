namespace Bookfiy_WepApp.Core.Models
{
    public class BookCopy : BaseModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public bool IsAvailabbleForRent { get; set; }
        public int EdditionNumber { get; set; }
        public int SerialNumber { get; set; }
       
    }
}
