namespace Mailing.Module.Domain.Models.ValueObjects;

internal sealed class Metadata(Guid id, string email, string password)
{
    private readonly Guid _id = id;
    private readonly string _email = email;
    private readonly string _password = password;

    public bool EqualById(Metadata metadata) => _id == metadata._id;
}