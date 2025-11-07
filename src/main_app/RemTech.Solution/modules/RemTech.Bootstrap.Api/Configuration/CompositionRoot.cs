using Brands.Module.Features.QueryBrands;
using Brands.Module.Features.QueryBrandsAmount;
using Brands.Module.Features.QueryPopularBrands;
using Categories.Module.Features.QueryCategories;
using Categories.Module.Features.QueryCategoriesAmount;
using Categories.Module.Features.QueryPopularCategories;
using Cleaners.Module.Endpoints;
using Mailing.Moduled.Features;
using RemTech.ContainedItems.Module.Features.DropSpares;
using RemTech.ContainedItems.Module.Features.DropVehicles;
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
using Scrapers.Module.Domain.JournalsContext.Features.GetScraperJournalRecordsCount;
using Scrapers.Module.Domain.JournalsContext.Features.GetScraperJournalsCount;
using Scrapers.Module.Domain.JournalsContext.Features.ReadScraperJournalRecords;
using Scrapers.Module.Domain.JournalsContext.Features.ReadScraperJournals;
using Scrapers.Module.Domain.JournalsContext.Features.RemoveScraperJournal;
using Scrapers.Module.Features.ChangeLinkActivity.Endpoint;
using Scrapers.Module.Features.ChangeParserState.Endpoint;
using Scrapers.Module.Features.CreateNewParserLink.Endpoint;
using Scrapers.Module.Features.InstantlyEnableParser.Endpoint;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using Scrapers.Module.Features.ReadConcreteScraper.Endpoint;
using Scrapers.Module.Features.RemovingParserLink.Endpoint;
using Scrapers.Module.Features.UpdateParserLink.Endpoint;
using Scrapers.Module.Features.UpdateWaitDays.Endpoint;
using Users.Module.Features.AddUserByAdmin;
using Users.Module.Features.AuthenticatingUserAccount;
using Users.Module.Features.ChangingEmail;
using Users.Module.Features.CheckRoot;
using Users.Module.Features.ConfirmUserEmail;
using Users.Module.Features.CreateAdmiin;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.CreateRoot;
using Users.Module.Features.CreatingNewAccount;
using Users.Module.Features.GetUserInfo;
using Users.Module.Features.ReadRoles;
using Users.Module.Features.ReadUsers;
using Users.Module.Features.ReadUsersCount;
using Users.Module.Features.RemoveUserByAdmin;
using Users.Module.Features.SessionRefreshing;
using Users.Module.Features.SignOut;
using Users.Module.Features.UpdateUserPassword;
using Users.Module.Features.UpdateUserProfile;
using Users.Module.Features.UserPasswordRecovering.Endpoint;
using Users.Module.Features.UserPasswordRecoveryConfirmation.Endpoint;
using Users.Module.Features.VerifyingAdmin;
using Users.Module.Features.VerifyingToken;
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
        app.MapCleanersEndpoints();
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
        ReadScraperJournalsEndpoint.Map(group);
        ReadScraperJournalRecordsEndpoint.Map(group);
        GetScraperJournalsCountEndpoint.Map(group);
        GetScraperJournalRecordsCountEndpoint.Map(group);
        RemoveScraperJournalEndpoint.Map(group);
    }

    private static void MapUsersEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/users").RequireCors("FRONTEND");
        UserPasswordRecoveringEndpoint.Map(builder);
        CreateUserAccountEndpoint.Map(builder);
        UserAuthenticationEndpoint.Map(builder);
        AdminVerificationEndpoint.Map(builder);
        EnsureRootCreatedEndpoint.Map(builder);
        CreateRootAccountEndpoint.Map(builder);
        SessionRefreshingEndpoint.Map(builder);
        UserPasswordRecoveryConfirmationEndpoint.Map(builder);
        VerifyTokenEndpoint.Map(builder);
        SignOutEndpoint.Map(builder);
        UpdateUserEmailEndpoint.Map(builder);
        ConfirmUserEmailEndpoint.Map(builder);
        GetUserInfoEndpoint.Map(builder);
        CreateEmailConfirmationEndpoint.Map(builder);
        UpdateUserPasswordEndpoint.Map(builder);
        builder.MapGet("roles", ReadRolesEndpoint.HandleFn).RequireAdminOrRootAccess();
        builder.MapGet("list", ReadUsersEndpoint.HandleFn).RequireAdminOrRootAccess();
        builder.MapPost("admin-up", CreateAdminAccountEndpoint.HandleFn).RequireAdminOrRootAccess();
        builder
            .MapPost("user-by-admin", AddUserByAdminEndpoint.HandleFn)
            .RequireAdminOrRootAccess();
        builder.MapPut("profile", UpdateUserProfileEndpoint.HandleFn).RequireAdminOrRootAccess();
        builder.MapDelete("profile", RemoveUserByAdminEndpoint.HandleFn).RequireAdminOrRootAccess();
        builder.MapGet("count", ReadUsersCountEndpoint.HandleFn).RequireAdminOrRootAccess();
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

    private static void MapCleanersEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/cleaners")
            .RequireCors("FRONTEND")
            .RequireAdminOrRootAccess();
        ChangeWaitDaysEndpoint.Map(group);
        EnableEndpoint.Map(group);
        ReadCleanerEndpoint.Map(group);
        DisableCleanerEndpoint.Map(group);
        PermantlyEnableCleanerEndpoint.Map(group);
        ChangeCleanerThresholdEndpoint.Map(group);
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
        DropSparesEndpoint.Map(group);
        DropVehiclesEndpoint.Map(group);
    }
}