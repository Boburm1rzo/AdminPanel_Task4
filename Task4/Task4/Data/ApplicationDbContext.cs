using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Task4.Models;

namespace Task4.Data;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
