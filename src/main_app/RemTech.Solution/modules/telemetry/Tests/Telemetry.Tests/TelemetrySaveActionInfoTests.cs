using RemTech.Core.Shared.Result;
using Telemetry.Domain.TelemetryContext;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Tests;

public sealed class TelemetrySaveActionInfoTests : IClassFixture<TestApplicationFactory>
{
    private readonly TelemetryModuleTestHelper _helper;

    public TelemetrySaveActionInfoTests(TestApplicationFactory factory)
    {
        _helper = new TelemetryModuleTestHelper(factory);
    }

    [Fact]
    private async Task Create_Record_Success()
    {
        IEnumerable<string> comments = ["Success", "Operation", "Completed"];
        string name = "Success operation";
        string status = TelemetryActionStatus.Success.Value;
        Guid invokerId = Guid.NewGuid();

        Status<TelemetryRecord> result = await _helper.CreateRecord(
            name,
            status,
            invokerId,
            comments
        );
        Assert.True(result.IsSuccess);

        Status<TelemetryRecord> createdResult = await _helper.GetById(result.Value.RecordId.Value);
        Assert.True(createdResult.IsSuccess);

        TelemetryRecord record = createdResult;
        Assert.Equal(record.InvokerId.Value, invokerId);
        Assert.Equal(record.Status.Value, status);
        Assert.Equal(record.Details.Name.Value, name);
        Assert.True(
            comments.Any(c => record.Details.Comments.Select(c => c.Value).Any(v => v == c))
        );
    }

    [Fact]
    private async Task Create_Records_From_Rabbit_Mq_Success()
    {
        IEnumerable<string> comments = ["Success", "Operation", "Completed"];
        string name = "Success operation";
        string status = TelemetryActionStatus.Success.Value;
        Guid invokerId = Guid.NewGuid();
        Status<string> result = await _helper.CreateRecordFromRabbitMq(
            name,
            status,
            invokerId,
            comments
        );
        Assert.True(result.IsSuccess);

        IEnumerable<TelemetryRecord> records = await GetCreatedRecord(result.Value);
        Assert.True(records.Any());
    }

    private async Task<IEnumerable<TelemetryRecord>> GetCreatedRecord(
        string name,
        int repeatTimes = 10
    )
    {
        int current = 0;
        while (current < repeatTimes)
        {
            IEnumerable<TelemetryRecord> records = await _helper.GetByName(name);
            if (!records.Any())
            {
                current += 1;
                await Task.Delay(500);
                continue;
            }

            return records;
        }

        return [];
    }
}
