using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.Disable;

public sealed record DisableCommand(Guid Id) : ICommand;
