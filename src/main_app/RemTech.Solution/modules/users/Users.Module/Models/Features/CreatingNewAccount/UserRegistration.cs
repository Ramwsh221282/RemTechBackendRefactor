using System.Text.RegularExpressions;
using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Models.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Models.Features.CreatingNewAccount;

internal sealed class UserRegistration
{
    private static Regex EmailValidityRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled
    );
    private readonly Guid _id;
    private readonly string _name;
    private readonly string _password;
    private readonly string _email;

    public UserRegistration(Guid id, string name, string password, string email)
    {
        _name = name;
        _password = password;
        _email = email;
        _id = id;
    }

    public UserRegistrationDetails FormDetails()
    {
        return new UserRegistrationDetails(this);
    }

    public void FillDetails(NpgsqlCommand command, StringHash hash, bool isRoot = false)
    {
        command.Parameters.Add(new NpgsqlParameter<Guid>("@user_id", _id));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", _name));
        command.Parameters.Add(new NpgsqlParameter<string>("@password", hash.Hash(_password)));
        command.Parameters.Add(new NpgsqlParameter<string>("@email", _email));
        command.Parameters.Add(new NpgsqlParameter<bool>("@email_confirmed", false));
        command.CommandText = """
            INSERT INTO users_module.users(id, name, password, email, email_confirmed) 
            VALUES (@user_id, @name, @password, @email, @email_confirmed);
            """;
    }

    public void FillDetails(UserRegistrationJwtDetails details)
    {
        details.AddId(_id);
        details.AddName(_name);
        details.AddPassword(_password);
        details.AddEmail(_email);
    }

    public void PasswordDifficultySatisfied()
    {
        if (_password.Length < 8)
            throw new PasswordLengthIsNotSatisfiedException();
        if (!_password.Any(char.IsUpper))
            throw new PasswordDoesNotContainUpperCharacterException();
        if (!_password.Any(char.IsLower))
            throw new PasswordDoesNotContainLowerCharacterException();
        if (!_password.Any(char.IsDigit))
            throw new PasswordShouldContainDigitException();
        if (!_password.Any(c => !char.IsLetterOrDigit(c)))
            throw new PasswordDoesNotContainSpecialSymbolException();
    }

    public void EmailDifficultySatisfied()
    {
        if (!EmailValidityRegex.IsMatch(_email))
            throw new InvalidEmailFormatException();
        if (_email.StartsWith(".") || _email.EndsWith("."))
            throw new InvalidEmailFormatException();
        if (_email.Contains(".."))
            throw new InvalidEmailFormatException();
        string localPart = _email.Split('@')[0];
        if (localPart.Length > 64)
            throw new InvalidEmailFormatException();
        if (_email.Length > 254)
            throw new InvalidEmailFormatException();
    }

    public void NameLengthSatisfied()
    {
        if (_name.Length > 50)
            throw new NameExceesLengthException();
    }
}
