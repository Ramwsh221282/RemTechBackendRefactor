using Mailers.Application.Features.CreateMailer;
using Mailers.Application.Features.DeleteMailer;
using Mailers.Core.MailersModule;
using Tests.ModuleFixtures;

namespace Tests.Mailing;

public sealed class RemoveMailerTests(CompositionRootFixture services) : IClassFixture<CompositionRootFixture>
{
    [Fact]
    private async Task Remove_Mailer_Success()
    {
        Mailer created = await CreatedMailer();
        await using AsyncServiceScope scope = services.Scope();
        DeleteMailerUseCase delete = scope.ServiceProvider.GetRequiredService<DeleteMailerUseCase>();
        DeleteMailerArgs args = new(created.Metadata.Id.Value, CancellationToken.None);
        Result<Unit> deletion = await delete.Invoke(args);
        Assert.True(deletion.IsSuccess);
    }

    [Fact]
    private async Task Remove_Mailer_Not_Exists()
    {
        await using AsyncServiceScope scope = services.Scope();
        DeleteMailerUseCase delete = scope.ServiceProvider.GetRequiredService<DeleteMailerUseCase>();
        DeleteMailerArgs args = new(Guid.NewGuid(), CancellationToken.None);
        Result<Unit> deletion = await delete.Invoke(args);
        Assert.True(deletion.IsFailure);
    }

    private async Task<Mailer> CreatedMailer()
    {
        await using AsyncServiceScope scope = services.Scope();
        CreateMailerUseCase create = scope.ServiceProvider.GetRequiredService<CreateMailerUseCase>();
        CreateMailerArgs args = new("validemail@mail.com", "some-test-password", CancellationToken.None);
        return await create.Invoke(args);
    }
}