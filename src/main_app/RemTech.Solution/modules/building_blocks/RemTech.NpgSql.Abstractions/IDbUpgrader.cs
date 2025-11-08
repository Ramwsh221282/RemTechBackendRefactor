namespace RemTech.NpgSql.Abstractions;

public interface IDbUpgrader
{
    void ApplyMigrations();
}