using RemTech.Core.Shared.Cqrs;

namespace Telemetry.Domain.UseCases.AddAction.Input;

/// <summary>
/// Команда для выполнения добавления действия.
/// </summary>
public sealed record AddActionCommand : ICommand
{
    public IEnumerable<string> Comments { get; }
    public string Name { get; }
    public string Status { get; }
    public Guid InvokerId { get; }
    public DateTime OccuredAt { get; }

    public AddActionCommand(
        IEnumerable<string> comments,
        string name,
        string status,
        Guid invokerId,
        DateTime? occuredAt = null
    )
    {
        Comments = comments;
        Name = name;
        Status = status;
        InvokerId = invokerId;
        OccuredAt = occuredAt ?? DateTime.UtcNow;
    }
}
