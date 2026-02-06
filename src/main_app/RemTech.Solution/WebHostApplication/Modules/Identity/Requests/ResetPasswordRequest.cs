namespace WebHostApplication.Modules.identity.Requests;

/// <summary>
/// Запрос на сброс пароля пользователя.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
public record ResetPasswordRequest(string? Login, string? Email);
