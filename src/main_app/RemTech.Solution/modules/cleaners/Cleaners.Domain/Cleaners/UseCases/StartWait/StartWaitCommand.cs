using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.StartWait;

public sealed record StartWaitCommand(Guid Id) : ICommand;
