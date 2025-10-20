using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace Shared.Infrastructure.Module.EfCore;

public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Конфигурация колонки для эмбеддинга с индексом hnsw и методом vector_cosine_ops.
    /// </summary>
    /// <param name="builder">Билдер сущности</param>
    /// <typeparam name="TEntity">Сущность</typeparam>
    /// <returns>Билдер сущности</returns>
    public static EntityTypeBuilder<TEntity> ConfigureEmbeddingProperty<TEntity>(
        this EntityTypeBuilder<TEntity> builder
    )
        where TEntity : class
    {
        builder.Property<Vector>("embedding").HasColumnType("vector(1024)").IsRequired(false);
        builder
            .HasIndex("embedding")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasDatabaseName("idx_hnsw_record");
        return builder;
    }
}
