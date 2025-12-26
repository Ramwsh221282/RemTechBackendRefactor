using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.Ports.Storage;

namespace Telemetry.Adapter.Storage.Tests;

public sealed class ActionRecordsAdapterTests : IClassFixture<ActionRecordsTestsFixture>
{
    private readonly ActionRecordsTestsFixture _fixture;

    public ActionRecordsAdapterTests(ActionRecordsTestsFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Add_Action_Record_Success()
    {
        Guid id = Guid.NewGuid();
        Guid invokerId = Guid.NewGuid();
        string name = "Database insert";
        DateTime occured = DateTime.UtcNow;
        string status = ActionStatus.Success.Value;
        IEnumerable<string> comments = ["Test Action Comment"];

        ActionRecord record = new()
        {
            Id = ActionId.Create(id),
            InvokerId = ActionInvokerId.Create(invokerId),
            Name = ActionName.Create(name),
            OccuredAt = ActionDate.Create(occured),
            Status = ActionStatus.Create(status),
            Comments = ActionComment.Create(comments).Value.ToList(),
        };

        await using (AsyncServiceScope scope = _fixture.CreateScope())
        {
            IActionRecordsStorage storage = scope.GetService<IActionRecordsStorage>();
            await storage.Add(record);
        }

        ActionRecord? created = null;
        await using (AsyncServiceScope scope = _fixture.CreateScope())
        {
            IActionRecordsStorage storage = scope.GetService<IActionRecordsStorage>();
            await storage.Add(record);
            Status<ActionRecord> entry = await storage.GetById(ActionId.Create(id));
            created = entry.IsFailure ? null : entry.Value;
        }

        Assert.NotNull(created);
        Assert.Equal(id, created.Id.Value);
        Assert.Equal(invokerId, created.InvokerId.Value);
        Assert.Equal(name, created.Name.Value);
        Assert.Equal(status, created.Status.Value);
        Assert.NotEmpty(created.Comments);
        Assert.True(created.Comments.Any(c => comments.Any(s => s == c.Value)));
    }
}
