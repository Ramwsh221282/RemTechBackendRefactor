namespace WebHostApplication.Modules.Identity.Requests;

/// <summary>
/// Запрос на подтверждение сброса пароля пользователя.
/// </summary>
/// <param name="NewPassword">Новый пароль пользователя.</param>
public sealed record CommitPasswordResetRequest(string NewPassword);
