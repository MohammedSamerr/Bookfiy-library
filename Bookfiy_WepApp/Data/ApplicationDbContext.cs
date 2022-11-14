using Bookfiy_WepApp.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Books_Categories>().HasKey(e => new {e.BookId , e.CategoryId});
            base.OnModelCreating(builder);
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Books_Categories> BookCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        
    }
}