using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Serilog;

#nullable disable

namespace ParsedAdvertisements.Adapters.Storage.Migrations
{
    public interface IAssemblyResourceSqlModification
    {
        void Apply(MigrationBuilder builder);
    }

    public sealed class MultiAssemblyResourceSqlScriptModification
        : IAssemblyResourceSqlModification
    {
        private readonly Queue<IAssemblyResourceSqlModification> _queue = [];

        public MultiAssemblyResourceSqlScriptModification Add(
            IAssemblyResourceSqlModification modification
        )
        {
            _queue.Enqueue(modification);
            return this;
        }

        public void Apply(MigrationBuilder builder)
        {
            while (_queue.Count > 0)
            {
                IAssemblyResourceSqlModification mod = _queue.Dequeue();
                mod.Apply(builder);
            }
        }
    }

    public sealed class LoggingAssemblyResourceSqlScriptModification(
        AssemblyResourceSqlScriptsModification modification,
        string? context = null
    ) : IAssemblyResourceSqlModification
    {
        private readonly string _actualContext = context ?? "UNKNOWN_CONTEXT";

        public void Apply(MigrationBuilder builder)
        {
            var logger = CreateLogger();
            try
            {
                logger.Information("{Context}. Attempt to apply seeding.", context);
                modification.Apply(builder);
                logger.Information("{Context}. Seeding has been applied.", context);
            }
            catch (InvalidOperationException exception)
            {
                logger.Error("{Context}. Error: {Ex}", context, exception);
            }
            catch (Exception ex)
            {
                logger.Fatal("{Context}. Exception: {Message}", context, ex.Message);
            }
        }

        private Serilog.ILogger CreateLogger()
        {
            return new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }
    }

    public sealed class AssemblyResourceSqlScriptsModification : IAssemblyResourceSqlModification
    {
        private readonly Assembly _assembly;
        private readonly Func<string, bool> _scriptsFilter;

        public void Apply(MigrationBuilder builder)
        {
            string? script = ReadSqlScriptResourceName();
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new InvalidOperationException("Could not find sql script.");
            }

            ApplySqlScriptForBuilder(script, builder);
        }

        private void ApplySqlScriptForBuilder(string script, MigrationBuilder builder)
        {
            string sqlContent = ReadSqlScriptAsString(script);
            builder.Sql(sqlContent);
        }

        private string? ReadSqlScriptResourceName()
        {
            return _assembly.GetManifestResourceNames().FirstOrDefault(_scriptsFilter);
        }

        private string ReadSqlScriptAsString(string script)
        {
            using var stream = _assembly.GetManifestResourceStream(script);
            using var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            return result;
        }

        public AssemblyResourceSqlScriptsModification AddScriptsFilter(Func<string, bool> filter)
        {
            return new AssemblyResourceSqlScriptsModification(this, filter);
        }

        public AssemblyResourceSqlScriptsModification(
            AssemblyResourceSqlScriptsModification origin,
            Func<string, bool> scriptsFilter
        )
            : this(origin._assembly, origin._scriptsFilter)
        {
            _scriptsFilter = scriptsFilter;
        }

        public AssemblyResourceSqlScriptsModification(
            Assembly assembly,
            Func<string, bool> scriptsFilter
        )
        {
            _assembly = assembly;
            _scriptsFilter = s => s.EndsWith(".sql") && _scriptsFilter(s);
        }

        public AssemblyResourceSqlScriptsModification(Assembly assembly)
        {
            _assembly = assembly;
            _scriptsFilter = s => s.EndsWith(".sql");
        }
    }

    /// <inheritdoc />
    public partial class ParsedAdvertisements_Brands_Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Assembly assembly = typeof(ParsedAdvertisementsDbContext).Assembly;
            new MultiAssemblyResourceSqlScriptModification()
                .Add(new LoggingAssemblyResourceSqlScriptModification(
                    new AssemblyResourceSqlScriptsModification(assembly)
                        .AddScriptsFilter(s => s.Contains("brands")),
                    "Brands Seeding")) // <-- сидирование начальных brands
                .Add(new LoggingAssemblyResourceSqlScriptModification(
                    new AssemblyResourceSqlScriptsModification(assembly)
                        .AddScriptsFilter(s => s.Contains("categories")),
                    "Categories Seeding")) // <-- сидирование начальных categories
                .Add(new LoggingAssemblyResourceSqlScriptModification(
                    new AssemblyResourceSqlScriptsModification(assembly)
                        .AddScriptsFilter(s => s.Contains("regions")),
                    "Regions Seeding")) // <-- сидирование начальных regions
                .Apply(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
