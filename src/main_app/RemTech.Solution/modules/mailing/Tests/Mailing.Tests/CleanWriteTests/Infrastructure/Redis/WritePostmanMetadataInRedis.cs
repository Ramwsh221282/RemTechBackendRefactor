namespace Mailing.Tests.CleanWriteTests.Infrastructure.Redis;

public sealed class WritePostmanMetadataInRedis(JsonPostman postman) : IWritePostmanMetadataInfrastructureCommand
{
    public void Execute(in Guid id, in string email, in string password)
    {
        postman.AddId(id);
        postman.AddEmail(email);
        postman.AddPassword(password);
    }
}