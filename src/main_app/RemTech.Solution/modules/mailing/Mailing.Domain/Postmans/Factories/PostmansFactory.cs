using Mailing.Domain.Postmans.Factories.Metadata;
using Mailing.Domain.Postmans.Factories.Statistics;

namespace Mailing.Domain.Postmans.Factories;

public sealed class PostmansFactory(IPostmanStatisticsFactory statistics, IPostmanMetadataFactory metadata)
    : IPostmansFactory
{
    public IPostman Construct(Guid id, string email, string password, int sendLimit, int currentSend) =>
        new Postman(metadata.Construct(id, email, password), statistics.Construct(sendLimit, currentSend));

    public IPostman Construct(string email, string password, int sendLimit, int currentSend) =>
        new Postman(metadata.Construct(email, password), statistics.Construct(sendLimit, currentSend));

    public IPostman Construct(string email, string password) =>
        new Postman(metadata.Construct(email, password), statistics.Construct());

    public IPostman Construct(Guid id, string email, string password) =>
        new Postman(metadata.Construct(id, email, password), statistics.Construct());
}