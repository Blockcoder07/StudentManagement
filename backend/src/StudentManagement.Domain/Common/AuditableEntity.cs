namespace StudentManagement.Domain.Common;

public abstract class AuditableEntity
{
    #region Properties

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    #endregion
}
