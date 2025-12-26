using System.Text;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed class UpdateUserContext
{
    private string _email = string.Empty;
    private string _name = string.Empty;
    private string _role = string.Empty;
    private string _password = string.Empty;

    public void AddPassword(string password) => _password = password;

    public void AddEmail(string email) => _email = email;

    public void AddName(string name) => _name = _name = name;

    public void AddRole(string role) => _role = role;

    public UpdateUserProfileResult PrintResult(UpdateUserProfileCommand command)
    {
        UpdateUserProfileResult result = new(
            command.PreviousDetails.UserId,
            command.PreviousDetails.UserEmail,
            command.PreviousDetails.UserName,
            command.PreviousDetails.UserRole
        );
        if (!string.IsNullOrWhiteSpace(_email))
            result = result with { UserEmail = _email };
        if (!string.IsNullOrWhiteSpace(_name))
            result = result with { UserName = _name };
        if (!string.IsNullOrWhiteSpace(_role))
            result = result with { UserRole = _role };
        return result;
    }

    public async Task SendEmailMessage(MailingBusPublisher publisher, CancellationToken ct)
    {
        MailingBusMessage message = GenerateMessage();
        await publisher.Send(message, ct);
    }

    private MailingBusMessage GenerateMessage()
    {
        string to = _email;
        string subject = "Изменение данных учетной записи";
        StringBuilder bodyBuilder = new StringBuilder(
                "Ваши данные учетной записи были изменены. Рекомендуем снова авторизоваться в системе, чтобы изменения вступили в силу."
            )
            .AppendLine($"Почта: {_email}")
            .AppendLine($"Псевдоним: {_name}");
        if (!string.IsNullOrWhiteSpace(_password))
            bodyBuilder.AppendLine($"Ваш новый пароль для входа: {_password}");
        else
            bodyBuilder.AppendLine("Пароль для входа в систему не изменился.");
        return new MailingBusMessage(to, bodyBuilder.ToString(), subject);
    }
}