namespace Bookfiy_WepApp.Core.Models
{
    public class Books_Categories 
    {
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int BookId { get; set; }
        public Book? Books { get; set; }
    }
}
