using System.Data;

namespace ParsedAdvertisements.Adapters.Storage.Seeding;

public interface IFromSqlRawSeeder
{
    Task SeedData(IDbConnection connection);
}