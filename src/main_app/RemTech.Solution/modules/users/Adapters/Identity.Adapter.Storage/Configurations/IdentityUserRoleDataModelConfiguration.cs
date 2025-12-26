using Identity.Adapter.Storage.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Adapter.Storage.Configurations;

public sealed class IdentityUserRoleDataModelConfiguration
    : IEntityTypeConfiguration<IdentityUserRoleDataModel>
{
    public void Configure(EntityTypeBuilder<IdentityUserRoleDataModel> builder)
    {
        builder.ToTable("user_roles");
        builder.HasKey(x => new { x.UserId, x.RoleId }).HasName("pk_user_roles");
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder
            .HasOne<IdentityUserDataModel>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne<IdentityRoleDataModel>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
