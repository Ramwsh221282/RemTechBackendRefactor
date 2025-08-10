namespace Parsing.RabbitMq;

public static class RabbitMqScraperConstants
{
    public const string ScrapersExchange = "scrapers";
    public const string ScrapersCreateQueue = "new_scraper";
    public const string AdvertisementsExchange = "advertisements";
    public const string VehiclesCreateQueue = "vehicles";
    public const string SparesCreateQueue = "spares";
    public const string ScrapersFinishQueue = "finish_scraper";
    public const string ScrapersFinishLinkQueue = "finish_scraper_link";
}
