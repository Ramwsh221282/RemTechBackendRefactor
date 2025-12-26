using Cleaners.Adapter.Storage.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cleaners.Adapter.Storage.Configurations;

public sealed class CleanerDataModelEntityConfiguration : IEntityTypeConfiguration<CleanerDataModel>
{
    public void Configure(EntityTypeBuilder<CleanerDataModel> builder)
    {
        builder.ToTable("cleaners");
        builder.HasKey(c => c.Id).HasName("pk_cleaners");
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.CleanedAmount).HasColumnName("cleaned_amount").IsRequired();
        builder.Property(c => c.LastRun).HasColumnName("last_run").IsRequired(false);
        builder.Property(c => c.NextRun).HasColumnName("next_run").IsRequired(false);
        builder.Property(c => c.WaitDays).HasColumnName("wait_days").IsRequired();
        builder.Property(c => c.State).HasColumnName("state").IsRequired().HasMaxLength(255);
        builder.Property(c => c.Hours).HasColumnName("hours").IsRequired();
        builder.Property(c => c.Minutes).HasColumnName("minutes").IsRequired();
        builder.Property(c => c.Seconds).HasColumnName("seconds").IsRequired();
        builder.Property(c => c.ItemsDateDayThreshold).HasColumnName("items_date_day_threshold");
        builder.HasIndex(c => c.NextRun).HasDatabaseName("idx_cleaners_next_run");
    }
}
