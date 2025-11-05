using Mailing.Domain.General;

namespace Mailing.Domain.Postmans.Factories.Metadata;

public sealed class PostmanLoggingMetadataFactory(Serilog.ILogger logger, IPostmanMetadataFactory factory)
    : PostmanMetadataFactoryEnvelope(factory)
{
    public override IPostmanMetadata Construct(Guid id, string email, string password)
    {
        logger.Information("Создание Postman Metadata.");
        try
        {
            IPostmanMetadata metadata = factory.Construct(id, email, password);
            logger.Information("Postman metadata создан.");
            return metadata;
        }
        catch (InvalidObjectStateException ex)
        {
            logger.Error("Не удается создать Postman metadata. Ошибка: {message}.", ex.Message);
            throw;
        }
    }

    public override IPostmanMetadata Construct(string email, string password)
    {
        logger.Information("Создание Postman Metadata.");
        try
        {
            IPostmanMetadata metadata = factory.Construct(email, password);
            logger.Information("Postman metadata создан.");
            return metadata;
        }
        catch (InvalidObjectStateException ex)
        {
            logger.Error("Не удается создать Postman metadata. Ошибка: {message}.", ex.Message);
            throw;
        }
    }
}