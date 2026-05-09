using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    #region Constructor

    public AppDbContext(DbContextOptions<AppDbContext> objOptions) : base(objOptions)
    {
    }

    #endregion

    #region DbSets

    public DbSet<Student> Students => Set<Student>();

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    #endregion

    #region Protected Methods

    protected override void OnModelCreating(ModelBuilder objModelBuilder)
    {
        objModelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(objModelBuilder);
    }

    #endregion
}
