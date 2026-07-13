using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.Id)
            .HasConversion(id => id.Value, value => DepartmentLocationId.Create(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(dl => dl.DepartmentId)
            .HasConversion(id => id.Value, value => DepartmentId.Create(value))
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasConversion(id => id.Value, value => LocationId.Create(value))
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(dl => dl.IsPrimary)
            .HasColumnName("is_primary")
            .IsRequired();

        // Связи удаляются вместе с подразделением, но не дают удалить локацию,
        // пока на неё ссылается хотя бы одно подразделение.
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dl => new { dl.DepartmentId, dl.LocationId }).IsUnique();
    }
}
