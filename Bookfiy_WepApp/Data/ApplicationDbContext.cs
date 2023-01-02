using Bookfiy_WepApp.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence<int>("SerialNumber", schema: "shared").StartsAt(145786);
            builder.Entity<BookCopy>().Property(b => b.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            builder.Entity<Books_Categories>().HasKey(e => new {e.BookId , e.CategoryId});

            var cascadeFKs = builder.Model.GetEntityTypes().SelectMany(t=>t.GetForeignKeys()).
                Where(fk=>fk.DeleteBehavior ==DeleteBehavior.Cascade && !fk.IsOwnership);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Books_Categories> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<Subscriper> Subscripers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        
    }
}