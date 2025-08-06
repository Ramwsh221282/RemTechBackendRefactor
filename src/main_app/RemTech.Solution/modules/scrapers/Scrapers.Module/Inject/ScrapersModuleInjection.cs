using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scrapers.Module.Features.ChangeParserState.Endpoint;
using Scrapers.Module.Features.CreateNewParser.Inject;
using Scrapers.Module.Features.CreateNewParserLink.Endpoint;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using Scrapers.Module.Features.ReadConcreteScraper.Endpoint;
using Scrapers.Module.Features.RemovingParserLink.Endpoint;
using Scrapers.Module.Features.UpdateWaitDays.Endpoint;

namespace Scrapers.Module.Inject;

public static class ScrapersModuleInjection
{
    public static void InjectScrapersModule(this IServiceCollection services)
    {
        CreateNewParserInjection.Inject(services);
    }

    public static void UpScrapersModuleDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(ScrapersModuleInjection).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create scrapers database.");
    }

    public static void MapScrapersModuleEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/scrapers").RequireCors("FRONTEND");
        ReadAllTransportParsersEndpoint.Map(group);
        ParserStateChangeEndpoint.Map(group);
        ConcreteScraperEndpoint.Map(group);
        ParserWaitDaysUpdateEndpoint.Map(group);
        CreateNewParserLinkEndpoint.Map(group);
        RemoveParserLinkEndpoint.Map(group);
    }
}
