using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfCoreNestedTransactions
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Integrated Security=SSPI;Initial Catalog=BlogDb;Data Source=LT-R90S3YAQ\SQLEXPRESS");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasKey(f => f.BlogId);

            modelBuilder.Entity<Blog>()
                .Property(f => f.BlogId).IsRequired().ValueGeneratedOnAdd();

            modelBuilder.Entity<Post>()
                .HasKey(f => f.PostId);

            modelBuilder.Entity<Post>()
                .Property(f => f.PostId).IsRequired().ValueGeneratedOnAdd();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public List<Post> Posts { get; } = new List<Post>();
        public string Url { get; set; }
    }

    public class Post
    {
        public Blog Blog { get; set; }
        public int BlogId { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public string Title { get; set; }
    }
}