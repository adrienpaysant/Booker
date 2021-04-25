using Booker.Models;
using Microsoft.EntityFrameworkCore;


namespace Booker.Data
{

    public class BookerContext: DbContext
    {
        public BookerContext(DbContextOptions<BookerContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
        public DbSet<User> User { get; set; }


    }
}