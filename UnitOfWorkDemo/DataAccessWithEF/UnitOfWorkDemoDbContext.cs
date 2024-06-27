
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
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DOB)
                    .IsRequired();

                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CreatedBy)
                    .IsRequired();

                entity.Property(e => e.IsPublished)
                    .IsRequired();

                entity.Property(e => e.Content)
                    .HasMaxLength(12000);

                entity.HasOne(b => b.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(b => b.ReviewedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.ReviewedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(b => b.Comments)
                    .WithOne(c => c.ParentBlog)
                    .HasForeignKey(c => c.ParentBlogId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BlogComment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ParentBlogId)
                    .IsRequired();

                entity.Property(e => e.CommentedBy)
                    .IsRequired();

                entity.Property(e => e.CommentedOn)
                    .IsRequired();

                entity.Property(e => e.CommentContent)
                    .HasMaxLength(1000);

                entity.HasOne(c => c.CommentedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CommentedBy)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
