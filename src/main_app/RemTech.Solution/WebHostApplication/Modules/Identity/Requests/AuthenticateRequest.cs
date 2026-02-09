namespace WebHostApplication.Modules.Identity.Requests;

/// <summary>
/// Запрос на аутентификацию пользователя.
/// </summary>
/// <param name="Password">Пароль пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
public sealed record AuthenticateRequest(string Password, string? Email, string? Login);
