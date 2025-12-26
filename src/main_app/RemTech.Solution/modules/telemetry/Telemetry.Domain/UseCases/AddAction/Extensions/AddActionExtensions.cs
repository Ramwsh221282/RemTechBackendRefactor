using Telemetry.Domain.Models;
using Telemetry.Domain.UseCases.AddAction.Output;

namespace Telemetry.Domain.UseCases.AddAction.Extensions;

public static class AddActionExtensions
{
    public static ActionRecordOutput ToOutput(this ActionRecord record)
    {
        Guid id = record.Id.Value;
        Guid invokerId = record.InvokerId.Value;
        string status = record.Status.Value;
        string name = record.Status.Value;
        DateTime occuredAt = record.OccuredAt.OccuredAt;
        IEnumerable<string> comments = [.. record.Comments.Select(c => c.Value)];
        ActionRecordOutput output = new(id, invokerId, status, name, occuredAt, comments);
        return output;
    }
}
