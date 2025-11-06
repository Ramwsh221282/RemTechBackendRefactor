using Mailing.Domain.General;
using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Domain.Implementations;

public sealed class WritePostmanMetadataDomainCommand : IWritePostmanMetadataDomainCommand
{
    private const string IdEmpty = "Идентификатор postman пустой.";
    private const string EmailEmpty = "Почта postman пустая.";
    private const string PasswordEmpty = "Пароль postman пустой.";

    public void Execute(in Guid id, in string email, in string password) =>
        Invalidate(id, email, password);

    private void Invalidate(in Guid id, in string email, in string password)
    {
        if (id == Guid.Empty) throw new InvalidObjectStateException(IdEmpty);
        if (string.IsNullOrWhiteSpace(email)) throw new InvalidObjectStateException(EmailEmpty);
        if (string.IsNullOrWhiteSpace(password)) throw new InvalidObjectStateException(PasswordEmpty);
    }
}