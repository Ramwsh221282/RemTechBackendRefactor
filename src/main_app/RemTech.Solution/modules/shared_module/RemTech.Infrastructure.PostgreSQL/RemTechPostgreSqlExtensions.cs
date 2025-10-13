using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.Infrastructure.PostgreSQL.Vector;
using RemTech.UseCases.Shared.Database;

namespace RemTech.Infrastructure.PostgreSQL;

public static class RemTechPostgreSqlExtensions
{
    public static void ConfigureVector<T>(this EntityTypeBuilder<T> builder, string table)
        where T : class
    {
        builder
            .Property<Pgvector.Vector>("embedding")
            .HasColumnType("vector(1024)")
            .IsRequired(false);
        builder
            .HasIndex("embedding")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasDatabaseName($"idx_hnsw_{table}");
    }

    public static void UsePgVector(this DbContextOptionsBuilder builder, NpgsqlOptions options)
    {
        builder.UseNpgsql(options.FormConnectionString(), o => o.UseVector());
    }

    public static void UsePgVectorExtension(this ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");
    }

    public static void InjectDatabaseDependencies<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<TContext>();
        services.AddScoped<ITransactionSource, TransactionSource<TContext>>();
        services.InjectEmbeddingGenerator();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
    }

    private static void InjectEmbeddingGenerator(this IServiceCollection services)
    {
        IEmbeddingGenerator generator = new OnnxEmbeddingGenerator();
        services.TryAddSingleton(generator);
    }
}
