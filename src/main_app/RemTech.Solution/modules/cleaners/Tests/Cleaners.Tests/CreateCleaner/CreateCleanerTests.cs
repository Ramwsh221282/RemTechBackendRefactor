using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.CreateCleaner;

public sealed class CreateCleanerTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly CleanersTestHostFactory _factory;

    public CreateCleanerTests(CleanersTestHostFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    private async Task First_cleaner_can_be_created()
    {
        var command = new CreateCleanerCommand();

        await using var scope = _factory.Services.CreateAsyncScope();

        var cleaner = await scope
            .GetService<
                ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);
        Assert.True(cleaner.IsSuccess);

        await using var scope2 = _factory.Services.CreateAsyncScope();
        var created = await scope2.GetService<ICleanersStorage>().Get();
        Assert.True(created.IsSuccess);

        await using var scope3 = _factory.Services.CreateAsyncScope();
        var cached = await scope3.GetService<ICleanersCachedStorage>().Get(created.Value.Id);
        Assert.True(cached.IsSuccess);

        Assert.Equal(cleaner.Value.Id, created.Value.Id);
        Assert.Equal(cleaner.Value.CleanedAmount, created.Value.CleanedAmount);
        Assert.Equal(cleaner.Value.ItemsDateDayThreshold, created.Value.ItemsDateDayThreshold);
        Assert.Equal(cleaner.Value.Schedule, created.Value.Schedule);
        Assert.Equal(cleaner.Value.WorkTime, created.Value.WorkTime);
    }

    [Fact]
    private async Task Second_cleaner_cannot_be_created()
    {
        var command = new CreateCleanerCommand();
        await using var scope = _factory.Services.CreateAsyncScope();

        await scope
            .GetService<
                ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);

        var cleaner = await scope
            .GetService<
                ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);

        Assert.True(cleaner.IsFailure);
    }
}
