
using DataAccessWithEF.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessWithEF
{
    public class UnitOfWorkDemoDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<BlogComment> BlogComments { get; set; }


        public UnitOfWorkDemoDbContext(DbContextOptions<UnitOfWorkDemoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   

        }
    }
}
