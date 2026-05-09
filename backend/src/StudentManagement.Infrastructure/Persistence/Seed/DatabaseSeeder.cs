using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.Interfaces.Authentication;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;

namespace StudentManagement.Infrastructure.Persistence.Seed;

public class DatabaseSeeder
{
    #region Fields

    private readonly AppDbContext _objDbContext;
    private readonly IPasswordHasher _objPasswordHasher;
    private readonly ILogger<DatabaseSeeder> _objLogger;

    #endregion

    #region Constructor

    public DatabaseSeeder(
        AppDbContext objDbContext,
        IPasswordHasher objPasswordHasher,
        ILogger<DatabaseSeeder> objLogger)
    {
        _objDbContext = objDbContext;
        _objPasswordHasher = objPasswordHasher;
        _objLogger = objLogger;
    }

    #endregion

    #region Public Methods

    public async Task SeedAsync(CancellationToken objCancellationToken = default)
    {
        await _objDbContext.Database.MigrateAsync(objCancellationToken);

        await SeedAdminAsync(objCancellationToken);
    }

    #endregion

    #region Private Methods

    private async Task SeedAdminAsync(CancellationToken objCancellationToken)
    {
        bool isAnyUser = await _objDbContext.Users.AnyAsync(objCancellationToken);
        if (isAnyUser)
        {
            return;
        }

        (string stHash, string stSalt) = _objPasswordHasher.Hash("Admin@123");

        ApplicationUser objAdmin = new()
        {
            Username = "admin",
            Email = "admin@studentmanagement.local",
            PasswordHash = stHash,
            PasswordSalt = stSalt,
            Role = UserRole.Admin,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _objDbContext.Users.AddAsync(objAdmin, objCancellationToken);
        await _objDbContext.SaveChangesAsync(objCancellationToken);

        _objLogger.LogInformation("Default admin user has been seeded with username 'admin' and password 'Admin@123'.");
    }

    #endregion
}
