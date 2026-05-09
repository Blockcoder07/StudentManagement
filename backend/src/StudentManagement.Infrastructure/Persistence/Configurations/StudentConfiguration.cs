using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    #region Public Methods

    public void Configure(EntityTypeBuilder<Student> objBuilder)
    {
        objBuilder.ToTable("Students");

        objBuilder.HasKey(s => s.Id);

        objBuilder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        objBuilder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(150);

        objBuilder.Property(s => s.Course)
            .IsRequired()
            .HasMaxLength(100);

        objBuilder.Property(s => s.Age)
            .IsRequired();

        objBuilder.Property(s => s.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        objBuilder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        objBuilder.HasIndex(s => s.Email).IsUnique().HasDatabaseName("UX_Students_Email");
        objBuilder.HasIndex(s => s.Name).HasDatabaseName("IX_Students_Name");
        objBuilder.HasIndex(s => s.Course).HasDatabaseName("IX_Students_Course");
    }

    #endregion
}
