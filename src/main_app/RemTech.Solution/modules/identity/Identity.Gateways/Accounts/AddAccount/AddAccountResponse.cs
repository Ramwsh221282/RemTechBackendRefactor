using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.AddAccount;

public sealed record AddAccountResponse(Guid Id) : IResponse;