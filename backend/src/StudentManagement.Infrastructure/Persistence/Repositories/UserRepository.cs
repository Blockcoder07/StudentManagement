using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    #region Fields

    private readonly AppDbContext _objDbContext;

    #endregion

    #region Constructor

    public UserRepository(AppDbContext objDbContext)
    {
        _objDbContext = objDbContext;
    }

    #endregion

    #region Public Methods

    public Task<ApplicationUser?> GetByUsernameAsync(string stUsername, CancellationToken objCancellationToken = default)
        => _objDbContext.Users.FirstOrDefaultAsync(u => u.Username == stUsername, objCancellationToken);

    public async Task UpdateAsync(ApplicationUser objUser, CancellationToken objCancellationToken = default)
    {
        _objDbContext.Users.Update(objUser);
        await _objDbContext.SaveChangesAsync(objCancellationToken);
    }

    #endregion
}
