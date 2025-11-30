using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLogActionsManagement;

public sealed record ParserLogAction(
    Guid Id,
    string Domain,
    string Type,
    DateTime Occured,
    ParserLogSeverity Severity,
    string Information)
{
    public Result<Unit> Validate()
    {
        const string idName = "Идентификатор лога действия парсера";
        const string domainName = "Идентификатор домена сервиса парсера";
        const string typeName = "Тип обрабатываемых данных парсером";
        const string dateName = "Дата возникновения действия";
        const string infoName = "Информация действия парсера";
        const int maxDomainLength = 128;
        const int maxTypeLength = 128;
        List<string> errors = [];
        if (Id ==  Guid.Empty) errors.Add(Error.NotSet(idName));
        if (string.IsNullOrWhiteSpace(Domain)) errors.Add(Error.NotSet(domainName));
        if (Domain.Length > maxDomainLength)  errors.Add(Error.GreaterThan(domainName, maxDomainLength));
        if (string.IsNullOrWhiteSpace(Type)) errors.Add(Error.NotSet(typeName));
        if (Type.Length > maxTypeLength) errors.Add(Error.GreaterThan(typeName, maxTypeLength));
        if (string.IsNullOrWhiteSpace(Information)) errors.Add(Error.NotSet(infoName));
        if (Occured == DateTime.MinValue || Occured == DateTime.MaxValue) errors.Add(Error.InvalidFormat(dateName));
        return Unit.ValidationUnit(errors);
    }
}