using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.CreatePasswordResetTicket;

public sealed record CreatePasswordResetCommand(
    Guid? IssuerId = null,
    string? IssuerEmail = null,
    string? IssuerLogin = null
) : ICommand;
