using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    #region Public Methods

    public void Configure(EntityTypeBuilder<ApplicationUser> objBuilder)
    {
        objBuilder.ToTable("Users");

        objBuilder.HasKey(u => u.Id);

        objBuilder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        objBuilder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        objBuilder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        objBuilder.Property(u => u.PasswordSalt)
            .IsRequired()
            .HasMaxLength(200);

        objBuilder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        objBuilder.Property(u => u.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        objBuilder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        objBuilder.HasIndex(u => u.Username).IsUnique().HasDatabaseName("UX_Users_Username");
        objBuilder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UX_Users_Email");
    }

    #endregion
}
