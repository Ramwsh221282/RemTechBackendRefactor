namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostmanMetadata(Guid id, string email, string password)
{
    internal PostmanSnapshot Supply(PostmanSnapshot snapshot) =>
        snapshot with { Id = id, Email = email, Password = password };
}