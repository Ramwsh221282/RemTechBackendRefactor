using Mailing.Tests.CleanWriteTests.Presenter.Implementation;

namespace Mailing.Tests.CleanWriteTests.Presenter;

public sealed class WritePostmanMetadataPresenter(PostmanDto postmanDto) : IWritePostmanMetadataPresenter
{
    public void Execute(in Guid id, in string email, in string password)
    {
        postmanDto.Id = id;
        postmanDto.Email = email;
        postmanDto.Password = password;
    }
}