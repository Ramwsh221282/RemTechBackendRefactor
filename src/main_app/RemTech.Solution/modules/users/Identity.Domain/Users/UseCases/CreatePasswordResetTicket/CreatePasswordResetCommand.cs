using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.CreatePasswordResetTicket;

public sealed record CreatePasswordResetCommand(
    string? IssuerEmail = null,
    string? IssuerLogin = null
) : ICommand;
