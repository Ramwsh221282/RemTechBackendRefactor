using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParsedAdvertisements.Adapters.Storage.CharacteristicContext.DataModels;
using ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;
using Shared.Infrastructure.Module.EfCore;

namespace ParsedAdvertisements.Adapters.Storage.CharacteristicContext.Configuration;

public sealed class CharacteristicDataModelConfiguration
    : IEntityTypeConfiguration<CharacteristicDataModel>
{
    public void Configure(EntityTypeBuilder<CharacteristicDataModel> builder)
    {
        builder.ToTable("characteristics");
        builder.HasKey(x => x.Id).HasName("pk_characteristics");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(CharacteristicName.MaxLength);
        builder.ConfigureEmbeddingProperty("characteristics");
    }
}
