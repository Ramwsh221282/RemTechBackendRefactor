using Identity.Adapter.Storage.DataModels;
using Identity.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Adapter.Storage.Configurations;

public sealed class IdentityUserDataModelConfiguration
    : IEntityTypeConfiguration<IdentityUserDataModel>
{
    public void Configure(EntityTypeBuilder<IdentityUserDataModel> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id).HasName("pk_users");
        builder.Property(x => x.Id).HasColumnName("id");
        builder
            .Property(x => x.Login)
            .HasColumnName("name")
            .HasMaxLength(UserLogin.MaxLength)
            .IsRequired();
        builder.Property(x => x.Password).IsRequired().HasColumnName("password").HasMaxLength(100);
        builder
            .Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(UserEmail.MaxLength);
        builder.Property(x => x.EmailConfirmed).HasColumnName("email_confirmed").IsRequired();
        builder
            .HasMany<IdentityUserRoleDataModel>()
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
