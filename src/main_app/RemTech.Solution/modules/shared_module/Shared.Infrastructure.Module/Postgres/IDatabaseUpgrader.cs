namespace Shared.Infrastructure.Module.Postgres;

public interface IDatabaseUpgrader
{
    void ApplyMigrations();
}