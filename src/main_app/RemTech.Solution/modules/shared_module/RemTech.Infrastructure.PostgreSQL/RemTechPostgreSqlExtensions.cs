using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace RemTech.Infrastructure.PostgreSQL;

public static class RemTechPostgreSqlExtensions
{
    public static void ConfigureVector<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        builder.Property<Vector>("embedding").HasColumnType("vector(1024)").IsRequired(false);
        builder
            .HasIndex("embedding")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasDatabaseName("idx_hnsw_record");
    }

    public static void UsePgVector(this DbContextOptionsBuilder builder, NpgsqlOptions options)
    {
        builder.UseNpgsql(options.FormConnectionString(), o => o.UseVector());
    }

    public static void UsePgVectorExtension(this ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");
    }
}
