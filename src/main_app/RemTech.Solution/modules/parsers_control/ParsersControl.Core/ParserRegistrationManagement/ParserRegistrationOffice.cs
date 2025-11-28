using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserRegistrationManagement.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserRegistrationManagement;

public sealed class ParserRegistrationOffice(ParserData data)
{
    private readonly IOnParserRegisteredEventListener _onRegistered =
        new NoneOnParserRegisteredEventListener();
    
    public async Task<Result<Unit>> Register(CancellationToken ct = default)
    {
        Result<Unit> validation = ValidateState();
        if (validation.IsFailure) return validation;
        return await _onRegistered.React(data, ct);
    }
    
    private Result<Unit> ValidateState()
    {
        const int maxTypeLength = 128;
        const int maxDomainLength = 128;
        const string idName = "Идентификатор парсера";
        const string typeName = "Тип данных парсера";
        const string domainName = "Домен сервиса, который парсится";
        List<string> errors = [];
        if (data.Id == Guid.Empty) errors.Add(Error.NotSet(idName));
        if (string.IsNullOrWhiteSpace(data.Type)) errors.Add(Error.NotSet(typeName));
        if (string.IsNullOrWhiteSpace(data.Domain)) errors.Add(Error.NotSet(domainName));
        if (data.Type.Length > maxTypeLength) errors.Add(Error.GreaterThan(typeName, maxTypeLength));
        if (data.Domain.Length > maxDomainLength) errors.Add(Error.GreaterThan(domainName, maxDomainLength));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }

    public ParserRegistrationOffice AddListener(IOnParserRegisteredEventListener listener)
    {
        return new ParserRegistrationOffice(data, listener);
    }
    
    private ParserRegistrationOffice(ParserData data, IOnParserRegisteredEventListener onRegistered) : this(data)
    {
        _onRegistered = onRegistered;
    }
}