using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.CleanItem;

public sealed record CleanItemCommand(Guid Id) : ICommand;
