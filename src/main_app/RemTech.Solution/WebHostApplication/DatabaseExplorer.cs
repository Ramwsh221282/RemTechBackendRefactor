using System.Reflection;
using ContainedItems.Infrastructure.Migrations;
using FluentMigrator;
using Identity.Infrastructure.Migrations;
using Notifications.Infrastructure.PendingEmails;
using ParsersControl.Infrastructure.Parsers.Repository;
using Serilog;
using Spares.Infrastructure.Repository;
using Sprache;
using Telemetry.Infrastructure;
using TimeZones.Infrastructure;
using Vehicles.Infrastructure.Vehicles.PersisterImplementation;

namespace WebHostApplication;

public static class DatabaseExplorer
{
	public static void LogLastMigrationLevel(Serilog.ILogger? logger = null)
	{
		Assembly[] assemblies =
		[
			typeof(ContainedItemsMigration).Assembly,
			typeof(AccessTokensTableMigration).Assembly,
			typeof(PendingEmailNotificationsRepository).Assembly,
			typeof(SubscribedParsersRepository).Assembly,
			typeof(SparesRepository).Assembly,
			typeof(RedisTelemetryActionsStorage).Assembly,
			typeof(TimeZonesProviderOptions).Assembly,
			typeof(NpgSqlVehiclesPersister).Assembly,
		];

		LogLastMigrationLevel(assemblies, logger);
	}

	public static void LogLastMigrationLevel(Assembly[] assemblies, Serilog.ILogger? logger = null)
	{
		Type inheritedType = typeof(Migration);
		Type[] migrationClasses = [.. GetTypesDerivedFrom(assemblies, inheritedType)];
		Type lastMigration = GetTypeOfLastMigration(migrationClasses);
		Type preLastMigration = GetTypeOfPreLastMigration(migrationClasses);
		Serilog.ILogger loggerToUse = CreateLoggerIfNotProvided(logger);
		PrintLastMigrationInformation(loggerToUse, lastMigration);
		loggerToUse.Information("Pre last migration information:");
		PrintLastMigrationInformation(loggerToUse, preLastMigration);
	}

	private static IEnumerable<Type> GetTypesDerivedFrom(Assembly[] assemblies, Type baseType)
	{
		return assemblies.SelectMany(a => a.GetTypes()).Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);
	}

	private static Type GetTypeOfPreLastMigration(IEnumerable<Type> types)
	{
		return types
			.Where(t => t.GetCustomAttribute<MigrationAttribute>() != null)
			.OrderBy(t => t.GetCustomAttribute<MigrationAttribute>()!.Version)
			.SkipLast(1)
			.Last();
	}

	private static Type GetTypeOfLastMigration(IEnumerable<Type> types)
	{
		return types
			.Where(t => t.GetCustomAttribute<MigrationAttribute>() != null)
			.OrderBy(t => t.GetCustomAttribute<MigrationAttribute>()!.Version)
			.Last();
	}

	private static Serilog.ILogger CreateLoggerIfNotProvided(Serilog.ILogger? logger)
	{
		return logger is not null ? logger : new Serilog.LoggerConfiguration().WriteTo.Console().CreateLogger();
	}

	private static void PrintLastMigrationInformation(Serilog.ILogger logger, Type type)
	{
		MigrationAttribute attribute = type.GetCustomAttribute<MigrationAttribute>()!;
		long version = attribute.Version;
		string? typeName = type.FullName;
		logger.Information(
			"""
			Last migration information:
			Type name (class): {TypeName}
			Version: {Version}
			""",
			typeName,
			version
		);
	}
}
