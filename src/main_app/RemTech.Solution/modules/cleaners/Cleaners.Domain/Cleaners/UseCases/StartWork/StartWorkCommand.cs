using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.StartWork;

public sealed record StartWorkCommand(Guid Id) : ICommand;
