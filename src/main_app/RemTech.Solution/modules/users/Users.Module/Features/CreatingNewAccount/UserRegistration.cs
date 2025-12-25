using Npgsql;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.CreatingNewAccount;

internal sealed class UserRegistration
{
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
        new PasswordValidation(_password).Validate();
    }

    public void EmailDifficultySatisfied()
    {
        new EmailValidation().ValidateEmail(_email);
    }

    public void NameLengthSatisfied()
    {
        if (_name.Length > 50)
            throw new NameExceesLengthException();
    }
}
