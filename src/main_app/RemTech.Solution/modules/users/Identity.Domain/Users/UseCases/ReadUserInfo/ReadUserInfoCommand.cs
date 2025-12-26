using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ReadUserInfo;

public sealed record ReadUserInfoCommand(Guid Id) : ICommand;
