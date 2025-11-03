using System.Data;
using Mailing.Domain.EmailSendingContext.Ports;
using RemTech.Core.Shared.Database;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.ValueObjects;

public sealed class SenderServiceInformation
{
    private readonly string _serviceName;
    private readonly string _password;

    private SenderServiceInformation(string name, string password)
    {
        _serviceName = name;
        _password = password;
    }

    public static Status<SenderServiceInformation> Create(string serviceName, string password) =>
        from valid_service_name in NotEmptyString.New(serviceName)
            .OverrideValidationError("Название сервиса почты было пустым.")
        from valid_password in NotEmptyString.New(password).OverrideValidationError("Пароль сервиса почты был пустым")
        select new SenderServiceInformation(valid_service_name, valid_password);

    public IDbCommand AddParameter(IDbCommand command) =>
        command.AddParameter("@service", _serviceName).AddParameter("@password", _password);
}