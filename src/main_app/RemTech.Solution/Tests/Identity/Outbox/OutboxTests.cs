using System.Text.Json;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Outbox.Shared;
using Tests.ModuleFixtures;

namespace Tests.Identity.Outbox;

public sealed class OutboxTests : IClassFixture<CompositionRootFixture>
{
    private readonly IServiceProvider _sp;
    private const string Schema = "identity_module";
    private readonly CancellationToken _ct = CancellationToken.None;

    public OutboxTests(CompositionRootFixture fixture)
    {
        _sp = fixture.Services;
    }

    [Fact]
    private async Task Write_To_Outbox_Success()
    {
        OutboxMessage message = OutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, Schema);
        await service.Add(message, _ct);
    }

    [Fact]
    private async Task Write_To_Outbox_Ensure_Has()
    {
        OutboxMessage message = OutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, Schema);
        await service.Add(message, _ct);
        Assert.True(await service.HasAny(_ct));
        IEnumerable<OutboxMessage> messages = await service.GetPendingMessages(50, _ct);
        Assert.NotEmpty(messages);
    }

    [Fact]
    private async Task Write_To_Outbox_Ensure_Mapped()
    {
        OutboxMessage message = OutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, Schema);
        await service.Add(message, _ct);
        IEnumerable<OutboxMessage> messages = await service.GetPendingMessages(50, _ct);
        Assert.NotEmpty(messages);
    }
    
    [Fact]
    private async Task Update_In_Outbox_Success()
    {
        OutboxMessage message = OutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, Schema);
        await service.Add(message, _ct);
        IEnumerable<OutboxMessage> messages = await service.GetPendingMessages(50, _ct);
        OutboxMessage first = messages.First();
        await service.UpdateMany([first], _ct);
    }

    [Fact]
    private async Task Delete_From_Outbox_Success()
    {
        OutboxMessage message = OutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        OutboxServicesRegistry registry = _sp.Resolve<OutboxServicesRegistry>();
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        OutboxService service = registry.GetService(session, Schema);
        await service.Add(message, _ct);
        IEnumerable<OutboxMessage> messages = await service.GetPendingMessages(50, _ct);
        OutboxMessage first = messages.First();
        await service.RemoveMany([first], _ct);
        Assert.False(await service.HasAny(_ct));
    }
}