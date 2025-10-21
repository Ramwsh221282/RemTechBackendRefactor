using Identity.Adapter.Storage.DataModels;
using Identity.Domain.Roles.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Adapter.Storage.Configurations;

public sealed class IdentityRoleDataModelConfiguration
    : IEntityTypeConfiguration<IdentityRoleDataModel>
{
    public void Configure(EntityTypeBuilder<IdentityRoleDataModel> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id).HasName("pk_roles");
        builder.Property(x => x.Id).HasColumnName("id");

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(RoleName.MaxLength);

        builder
            .HasMany<IdentityUserRoleDataModel>()
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("idx_roles_name");
    }
}
