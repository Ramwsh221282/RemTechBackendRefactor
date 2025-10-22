namespace Identity.Integrational.Tests.Common;

public class BaseIdentityModuleTestClass(IdentityTestApplicationFactory factory)
    : IClassFixture<IdentityTestApplicationFactory>,
        IAsyncLifetime
{
    protected readonly IdentityModuleUseCases UseCases = new(factory);

    public async Task InitializeAsync() => await UseCases.SeedRoles();

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
