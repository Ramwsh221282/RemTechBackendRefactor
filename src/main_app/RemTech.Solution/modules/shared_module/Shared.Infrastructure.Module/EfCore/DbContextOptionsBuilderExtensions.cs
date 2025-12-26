using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Module.EfCore;

public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Конфигурирование работы DbContext с PgVector.
    /// </summary>
    /// <param name="builder">Builder</param>
    /// <param name="connectionString">Строка подключения</param>
    public static void ConfigureForPgVector(
        this DbContextOptionsBuilder builder,
        string connectionString
    ) => builder.UseNpgsql(connectionString, o => o.UseVector());
}
