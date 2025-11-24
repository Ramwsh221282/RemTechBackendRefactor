namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public interface IDbUpgrader
{
    void ApplyMigrations();
}