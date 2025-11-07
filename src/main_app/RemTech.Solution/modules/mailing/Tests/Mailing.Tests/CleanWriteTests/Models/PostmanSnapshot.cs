namespace Mailing.Tests.CleanWriteTests.Models;

public sealed record PostmanSnapshot(Guid Id, string Email, string Password, int LimitSend, int CurrentSend)
{
    public static PostmanSnapshot Empty() => new PostmanSnapshot(Guid.Empty, string.Empty, string.Empty, 0, 0);
}