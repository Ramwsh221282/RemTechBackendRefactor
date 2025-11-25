using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.PrimitivesModule;

namespace Identity.Application.Accounts.Decorators;

public sealed class AccountValidationTools
{
    public Result<Unit> ValidateProperty(
        IAccountData data,
        string? email = null,
        string? password = null
    )
    {
        AccountData copied = AccountData.Copy(email: email, password: password, data: data);
        return ValidateData(copied);
    }

    public Result<Unit> ValidateProperty(
        IAccountRepresentation representation,
        string? email = null,
        string? password = null)
    {
        return ValidateProperty(representation.Data, email, password);
    }
    
    public Result<Unit> ValidateData(IAccountData data)
    {
        const int maxNameLength = 128;
        const int maxEmailLength = 128;
        string idName = "Идентификатор учетной записи";
        string nameName = "Название учетной записи";
        string passwordName = "Пароль учетной записи";
        string emailName = "Почта учетной записи";
        List<string> errors = [];
        if (data.Id == Guid.Empty) errors.Add(Error.NotSet(idName));
        if (string.IsNullOrWhiteSpace(data.Name)) errors.Add(Error.NotSet(nameName));
        if (data.Name.Length > maxNameLength) errors.Add(Error.GreaterThan(nameName, maxNameLength));
        if (string.IsNullOrWhiteSpace(data.Password)) errors.Add(Error.NotSet(passwordName));
        if (string.IsNullOrWhiteSpace(data.Email)) errors.Add(Error.NotSet(emailName));
        if (data.Email.Length > maxEmailLength) errors.Add(Error.GreaterThan(emailName, maxEmailLength));
        if (!EmailString.Create(data.Email).IsValid()) errors.Add(Error.InvalidFormat(emailName));
        return errors.Count == 0 
            ? Unit.Value 
            : Result.Failure<Unit>(Error.Validation(string.Join(Environment.NewLine, errors)));
    }
}