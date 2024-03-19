using BlogEF3.Data.Mappings;
using BlogEF3.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogEF3.Data;
public class BlogDataContext : DbContext
{
    public BlogDataContext(DbContextOptions<BlogDataContext> options)
    : base(options)
    {

    }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryMap());
        modelBuilder.ApplyConfiguration(new PostMap());
        modelBuilder.ApplyConfiguration(new UserMap());
    }
}