using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Module.EfCore;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Конфигурация базы данных с использованием PgVector.
    /// </summary>
    /// <param name="builder">Билдер.</param>
    /// <param name="actions">Дополнительные действия при конфигурации.</param>
    public static void ConfigureWithPgVectorExtension(
        this ModelBuilder builder,
        params Action<ModelBuilder>[] actions
    )
    {
        builder.HasPostgresExtension("vector");
        foreach (var action in actions)
            action(builder);
    }
}
