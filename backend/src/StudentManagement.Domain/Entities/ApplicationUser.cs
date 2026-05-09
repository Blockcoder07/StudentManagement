using StudentManagement.Domain.Common;
using StudentManagement.Domain.Enums;

namespace StudentManagement.Domain.Entities;

public class ApplicationUser : AuditableEntity
{
    #region Properties

    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    #endregion
}
