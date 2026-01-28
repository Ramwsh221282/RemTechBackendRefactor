using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.VerifyToken;

/// <summary>
/// Команда для проверки токена пользователя.
/// </summary>
/// <param name="Token">Токен для проверки.</param>
public sealed record VerifyTokenCommand(string Token) : ICommand;
