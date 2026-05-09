using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string stUsername, CancellationToken objCancellationToken = default);

    Task UpdateAsync(ApplicationUser objUser, CancellationToken objCancellationToken = default);
}
