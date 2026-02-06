namespace WebHostApplication.Modules.identity.Requests;

/// <summary>
/// Запрос на изменение пароля пользователя.
/// </summary>
/// <param name="NewPassword">Новый пароль пользователя.</param>
/// <param name="CurrentPassword">Текущий пароль пользователя.</param>
public record ChangePasswordRequest(string NewPassword, string CurrentPassword);
