using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Booker.Models;

namespace Booker.Data
{
    public class BookerContextId: IdentityDbContext<BookerUser>
    {
        public BookerContextId(DbContextOptions<BookerContextId> options)
            : base(options)
        {
        }
        public DbSet<Book> Book { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Comments> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
