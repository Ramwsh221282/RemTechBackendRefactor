using RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Database;

public sealed class DatabaseTests
{
    [Fact]
    private async Task Create_Db_And_Drop_Success()
    {
        ParsersManagementDbUp up = new(
            new ParsersManagementDatabaseConfiguration("appsettings.json")
        );
        up.Up();
        await up.Down();
    }
}
