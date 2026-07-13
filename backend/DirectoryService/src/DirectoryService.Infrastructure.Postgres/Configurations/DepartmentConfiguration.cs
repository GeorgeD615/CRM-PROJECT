using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(id => id.Value, value => DepartmentId.Create(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(d => d.Name)
            .HasConversion(name => name.Value, value => DepartmentName.Create(value))
            .HasColumnName("name")
            .HasMaxLength(DepartmentName.MaxLength)
            .IsRequired();

        builder.Property(d => d.Slug)
            .HasConversion(slug => slug.Value, value => DepartmentSlug.Create(value))
            .HasColumnName("slug")
            .HasMaxLength(DepartmentSlug.MaxLength)
            .IsRequired();

        builder.Property(d => d.Path)
            .HasConversion(path => path.Value, value => DepartmentPath.Create(value))
            .HasColumnName("path")
            .IsRequired();

        builder.Property(d => d.ParentId)
            .HasConversion(id => id!.Value, value => DepartmentId.Create(value))
            .HasColumnName("parent_id");

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Ignore(d => d.IsRoot);

        // Подразделение нельзя удалить, пока у него есть дочерние подразделения.
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Путь собирается из slug'ов родителей: его уникальность гарантирует
        // уникальность slug'а среди соседей по уровню, включая корни.
        builder.HasIndex(d => d.Path).IsUnique();
    }
}
