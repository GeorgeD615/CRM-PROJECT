using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, value => LocationId.Create(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(l => l.Name)
            .HasConversion(name => name.Value, value => LocationName.Create(value))
            .HasColumnName("name")
            .HasMaxLength(LocationName.MaxLength)
            .IsRequired();

        // Адрес — составной value object, хранится одной jsonb-колонкой владельца.
        builder.OwnsOne(l => l.Address, address =>
        {
            address.ToJson("address");

            address.Property(a => a.City).HasJsonPropertyName("city");
            address.Property(a => a.Street).HasJsonPropertyName("street");
            address.Property(a => a.House).HasJsonPropertyName("house");
            address.Property(a => a.Apartment).HasJsonPropertyName("apartment");
        });

        builder.Navigation(l => l.Address).IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Локации — справочник: две записи с одним именем неотличимы для пользователя.
        builder.HasIndex(l => l.Name).IsUnique();
    }
}
