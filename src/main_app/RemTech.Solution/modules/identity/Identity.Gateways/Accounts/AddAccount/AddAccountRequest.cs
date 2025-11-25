using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.AddAccount;

public sealed record AddAccountRequest(
    string Name, 
    string Email, 
    string Password, 
    CancellationToken Ct) : IRequest;