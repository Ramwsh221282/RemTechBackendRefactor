using Brands.Module.Features.QueryBrands;
using Brands.Module.Features.QueryBrandsAmount;
using Brands.Module.Features.QueryPopularBrands;
using Categories.Module.Features.QueryCategories;
using Categories.Module.Features.QueryCategoriesAmount;
using Categories.Module.Features.QueryPopularCategories;
using Mailing.Module.Features;
using RemTech.ContainedItems.Module.Features.GetContainedVehiclesAmount;
using RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;
using RemTech.ContainedItems.Module.Features.QueryRecentContainedItemsCount;
using RemTech.Spares.Module.Features.QuerySpare;
using RemTech.Spares.Module.Features.QuerySpareTotals;
using RemTech.Vehicles.Module.Features.QueryBrandsOfCategory;
using RemTech.Vehicles.Module.Features.QueryConcreteVehicle;
using RemTech.Vehicles.Module.Features.QueryModelsOfCategoryBrand;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands;
using RemTech.Vehicles.Module.Features.QueryVehicleCategories;
using RemTech.Vehicles.Module.Features.QueryVehicleModels;
using RemTech.Vehicles.Module.Features.QueryVehicleRegions;
using RemTech.Vehicles.Module.Features.QueryVehicles.Http;
using RemTech.Vehicles.Module.Features.QueryVehiclesAmount;
using RemTech.Vehicles.Module.Features.SimilarVehiclesQuery;
using Scrapers.Module.Features.ChangeLinkActivity.Endpoint;
using Scrapers.Module.Features.ChangeParserState.Endpoint;
using Scrapers.Module.Features.CreateNewParserLink.Endpoint;
using Scrapers.Module.Features.InstantlyEnableParser.Endpoint;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using Scrapers.Module.Features.ReadConcreteScraper.Endpoint;
using Scrapers.Module.Features.RemovingParserLink.Endpoint;
using Scrapers.Module.Features.UpdateParserLink.Endpoint;
using Scrapers.Module.Features.UpdateWaitDays.Endpoint;
using Users.Module.Models.Features.AuthenticatingUserAccount;
using Users.Module.Models.Features.CheckRoot;
using Users.Module.Models.Features.CreateAdmiin;
using Users.Module.Models.Features.CreateRoot;
using Users.Module.Models.Features.CreatingNewAccount;
using Users.Module.Models.Features.VerifyingAdmin;
using Users.Module.Public;

namespace RemTech.Bootstrap.Api.Configuration;

public static class CompositionRoot
{
    public static void MapModulesEndpoints(this WebApplication app)
    {
        app.MapBrandEndpoints();
        app.MapCategoryEndpoints();
        app.MapMailingEndpoints();
        app.MapScraperEndpoints();
        app.MapUsersEndpoints();
        app.MapVehiclesEndpoints();
        app.MapSparesEndpoints();
        app.MapContainedItemsEndpoints();
    }

    private static void MapBrandEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/brands").RequireCors("FRONTEND");
        QueryPopularBrandsEndpoint.Map(group);
        QueryBrandsEndpoint.Map(group);
        QueryBrandsAmountEndpoint.Map(group);
    }

    private static void MapCategoryEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/categories");
        QueryPopularCategoriesEndpoint.Map(group);
        QueryCategoriesEndpoint.Map(group);
        QueryCategoriesAmountEndpoint.Map(group);
    }

    private static void MapMailingEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/mailing")
            .RequireCors("FRONTEND")
            .RequireAdminOrRootAccess();
        CreateMailingSender.Map(builder);
        RemoveMailingSender.Map(builder);
        PingMailingSender.Map(builder);
        ReadMailingSenders.Map(builder);
    }

    private static void MapScraperEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/scrapers")
            .RequireCors("FRONTEND")
            .RequireAdminOrRootAccess();
        ReadAllTransportParsersEndpoint.Map(group);
        ParserStateChangeEndpoint.Map(group);
        ConcreteScraperEndpoint.Map(group);
        ParserWaitDaysUpdateEndpoint.Map(group);
        CreateNewParserLinkEndpoint.Map(group);
        RemoveParserLinkEndpoint.Map(group);
        UpdateParserLinkEndpoint.Map(group);
        ChangeLinkActivityEndpoint.Map(group);
        InstantlyEnableParserEndpoint.Map(group);
    }

    private static void MapUsersEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/users").RequireCors("FRONTEND");
        CreateUserAccountEndpoint.Map(builder);
        UserAuthenticationEndpoint.Map(builder);
        AdminVerificationEndpoint.Map(builder);
        EnsureRootCreatedEndpoint.Map(builder);
        CreateRootAccountEndpoint.Map(builder);
        builder.MapPost("admin-up", CreateAdminAccountEndpoint.HandleFn).RequireAdminOrRootAccess();
    }

    private static void MapVehiclesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/vehicles").RequireCors("FRONTEND");
        group.CatalogueEndpoint();
        QueryVehicleBrandsEndpoint.Map(group);
        QueryVehicleCategoriesEndpoint.Map(group);
        QueryVehicleRegionsEndpoint.Map(group);
        QueryVehicleModelsEndpoint.Map(group);
        VehiclesAmountEndpoint.Map(group);
        ConcreteVehicleEndpoint.Map(group);
        SimilarVehiclesQueryEndpoint.Map(group);
        BrandsByCategoryEndpoint.Map(group);
        ModelsOfCategoryBrandsEndpoint.Map(group);
    }

    private static void MapSparesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/spares").RequireCors("FRONTEND");
        QuerySpareHttpEndpoint.Map(group);
        SparesCountEndpoint.Map(group);
    }

    private static void MapContainedItemsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/contained-items").RequireCors("FRONTEND");
        GetContainedItemsByTypeEndpoint.Map(group);
        QuerySomeRecentItemsEndpoint.Map(group);
        QueryRecentContainedItemsEndpoint.Map(group);
    }
}
