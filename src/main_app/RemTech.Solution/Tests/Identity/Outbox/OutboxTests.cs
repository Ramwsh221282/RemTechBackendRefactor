using System.Text.Json;
using Identity.Outbox;
using RemTech.BuildingBlocks.DependencyInjection;
using Tests.ModuleFixtures;

namespace Tests.Identity.Outbox;

public sealed class OutboxTests : IClassFixture<CompositionRootFixture>
{
    private readonly IServiceProvider _sp;

    public OutboxTests(CompositionRootFixture fixture)
    {
        _sp = fixture.Services;
    }

    [Fact]
    private async Task Write_To_Outbox_Success()
    {
        await using var scope = _sp.CreateAsyncScope();
        CancellationToken ct = CancellationToken.None;
        IdentityOutboxStorage storage = scope.Resolve<IdentityOutboxStorage>();
        IdentityOutboxMessage message = IdentityOutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        await storage.Add(message, ct);
    }

    [Fact]
    private async Task Write_To_Outbox_Ensure_Has()
    {
        await using var scope = _sp.CreateAsyncScope();
        CancellationToken ct = CancellationToken.None;
        IdentityOutboxStorage storage = scope.Resolve<IdentityOutboxStorage>();
        IdentityOutboxMessage message = IdentityOutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        await storage.Add(message, ct);
        Assert.True(await storage.HasAny(ct));
        IEnumerable<IdentityOutboxMessage> messages = await storage.GetPendingMessages(50, ct);
        Assert.NotEmpty(messages);
    }

    [Fact]
    private async Task Write_To_Outbox_Ensure_Mapped()
    {
        await using var scope = _sp.CreateAsyncScope();
        CancellationToken ct = CancellationToken.None;
        IdentityOutboxStorage storage = scope.Resolve<IdentityOutboxStorage>();
        IdentityOutboxMessage message = IdentityOutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        await storage.Add(message, ct);
        IEnumerable<IdentityOutboxMessage> messages = await storage.GetPendingMessages(50, ct);
        Assert.NotEmpty(messages);
    }
    
    [Fact]
    private async Task Update_In_Outbox_Success()
    {
        await using var scope = _sp.CreateAsyncScope();
        CancellationToken ct = CancellationToken.None;
        IdentityOutboxStorage storage = scope.Resolve<IdentityOutboxStorage>();
        IdentityOutboxMessage message = IdentityOutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        await storage.Add(message, ct);
        IEnumerable<IdentityOutboxMessage> messages = await storage.GetPendingMessages(50, ct);
        IdentityOutboxMessage first = messages.First();
        await storage.UpdateMany([first], ct);
    }

    [Fact]
    private async Task Delete_From_Outbox_Success()
    {
        await using var scope = _sp.CreateAsyncScope();
        CancellationToken ct = CancellationToken.None;
        IdentityOutboxStorage storage = scope.Resolve<IdentityOutboxStorage>();
        IdentityOutboxMessage message = IdentityOutboxMessage.New(
            "Test Queue",
            "Test Type", 
            "My Exchange", 
            "My Routing key",
            JsonSerializer.Serialize(new { property = "test_property" }));
        await storage.Add(message, ct);
        IEnumerable<IdentityOutboxMessage> messages = await storage.GetPendingMessages(50, ct);
        IdentityOutboxMessage first = messages.First();
        await storage.RemoveMany([first], ct);
        Assert.False(await storage.HasAny(ct));
    }
}