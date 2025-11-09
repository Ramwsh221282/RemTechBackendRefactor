using Mailing.Module.MailersContext.MetadataContext;
using Mailing.Module.MailersContext.MetadataContext.Factories;
using Mailing.Module.MailersContext.StatisticsContext;
using Mailing.Module.MailersContext.StatisticsContext.Factories;
using Mailing.Module.Traits;

namespace Mailing.Module.MailersContext.Factories;

internal sealed class MailersFactory(
    IFactoryOf<MailersMetadataFactory.Input, MailerMetadata> metaDataFactory,
    IFactoryOf<MailersStatisticsFactory.Input, MailerStatistics> statisticsFactory)
    : IFactoryOf<MailersFactory.Input, Mailer>
{
    public Mailer Create(Input input)
    {
        return input.Create();
    }

    public sealed class Input(
        MailersMetadataFactory.Input metadata,
        MailersStatisticsFactory.Input statistics)
        :
            FactoryInput,
            ICreatorOf<Mailer>
    {
        public Mailer Create()
        {
            return new Mailer(metadata.Create(), statistics.Create());
        }
    }
}