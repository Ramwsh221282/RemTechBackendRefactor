namespace WebHostApplication.Modules.identity.Requests;

/// <summary>
/// Запрос на регистрацию учетной записи пользователя.
/// </summary>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
public sealed record RegisterAccountRequest(string Email, string Login, string Password);
