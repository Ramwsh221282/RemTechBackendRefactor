using Mailing.Module.Traits;
using RemTech.Core.Shared.Exceptions;

namespace Mailing.Module.MailersContext.StatisticsContext.Factories;

internal sealed class MailersStatisticsFactory : IFactoryOf<MailersStatisticsFactory.Input, MailerStatistics>
{
    public MailerStatistics Create(Input input)
    {
        input.Validate();
        return input.Convert();
    }

    public sealed class Input(int limit, int currentSend) :
        FactoryInput,
        IValidatable,
        ICreatorOf<MailerStatistics>
    {
        public MailerStatistics Create()
        {
            return new MailerStatistics(limit, currentSend);
        }

        public void Validate()
        {
            if (limit < 0)
                throw new InvalidObjectStateException("Ограничение на отправку сообщений не может быть отрицательным.");
            if (currentSend < 0)
                throw new InvalidObjectStateException("Количество отправленных сообщений не может быть отрицательным.");
        }

        public MailerStatistics Convert()
        {
            return new MailerStatistics(limit, currentSend);
        }
    }
}