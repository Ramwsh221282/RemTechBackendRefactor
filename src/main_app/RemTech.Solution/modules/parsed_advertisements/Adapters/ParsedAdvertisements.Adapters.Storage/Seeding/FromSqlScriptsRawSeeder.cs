using System.Data;
using System.Reflection;
using Dapper;
using ParsedAdvertisements.Adapters.Storage.Migrations;

namespace ParsedAdvertisements.Adapters.Storage.Seeding;

public sealed class FromSqlScriptsRawSeeder : IFromSqlRawSeeder
{
    private readonly Assembly _assembly;
    private readonly Func<string, bool> _scriptsFilter;

    public async Task SeedData(IDbConnection connection)
    {
        foreach (string sql in ReadSqlScripts())
        {
            int rowsCreated = await connection.ExecuteAsync(sql);
            Console.WriteLine($"Rows created: {rowsCreated}");
        }
    }

    private IEnumerable<string> ReadSqlScripts()
    {
        IEnumerable<string> scripts = _assembly.GetManifestResourceNames().Where(_scriptsFilter);
        foreach (string script in scripts)
            yield return ReadSqlScriptAsString(script);
    }

    private string ReadSqlScriptAsString(string script)
    {
        using var stream = _assembly.GetManifestResourceStream(script);
        using var reader = new StreamReader(stream);
        string result = reader.ReadToEnd();
        return result;
    }

    public FromSqlScriptsRawSeeder AddScriptsFilter(Func<string, bool> filter)
    {
        return new FromSqlScriptsRawSeeder(this, filter);
    }

    public FromSqlScriptsRawSeeder(
        FromSqlScriptsRawSeeder origin,
        Func<string, bool> scriptsFilter
    )
        : this(origin._assembly, origin._scriptsFilter)
    {
        _scriptsFilter = scriptsFilter;
    }

    public FromSqlScriptsRawSeeder(
        Assembly assembly,
        Func<string, bool> scriptsFilter
    )
    {
        _assembly = assembly;
        _scriptsFilter = s => s.EndsWith(".sql") && scriptsFilter(s);
    }

    public FromSqlScriptsRawSeeder(Assembly assembly)
    {
        _assembly = assembly;
        _scriptsFilter = s => s.EndsWith(".sql");
    }
}