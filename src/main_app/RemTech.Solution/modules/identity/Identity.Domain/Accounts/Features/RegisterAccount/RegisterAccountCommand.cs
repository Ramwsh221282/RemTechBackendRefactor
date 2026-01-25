using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed record RegisterAccountCommand(string Email, string Login, string Password) : ICommand;
