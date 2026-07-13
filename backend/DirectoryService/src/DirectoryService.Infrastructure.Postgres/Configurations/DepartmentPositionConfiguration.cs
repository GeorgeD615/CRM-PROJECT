using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.Id)
            .HasConversion(id => id.Value, value => DepartmentPositionId.Create(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(dp => dp.DepartmentId)
            .HasConversion(id => id.Value, value => DepartmentId.Create(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dp => dp.PositionId)
            .HasConversion(id => id.Value, value => PositionId.Create(value))
            .HasColumnName("position_id")
            .IsRequired();

        // Связи удаляются вместе с подразделением, но не дают удалить должность,
        // пока на неё ссылается хотя бы одно подразделение.
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Position>()
            .WithMany()
            .HasForeignKey(dp => dp.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dp => new { dp.DepartmentId, dp.PositionId }).IsUnique();
    }
}
