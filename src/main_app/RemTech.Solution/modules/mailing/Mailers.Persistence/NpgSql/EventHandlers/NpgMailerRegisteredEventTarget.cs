namespace Mailers.Persistence.NpgSql.EventHandlers;

public sealed class NpgMailerRegisteredEventTarget : CallbackableMailerEventTarget
{
    private readonly NpgSqlMailer _mailer;
    private readonly CancellationToken _ct;
    private event Action _onRegistered;

    public NpgMailerRegisteredEventTarget(NpgSqlMailer mailer, CancellationToken ct)
    {
        _mailer = mailer;
        _ct = ct;
        _onRegistered += async () =>
        {
            var attempt = await AsyncTry.For(HandleRegistration).Attempt();
            attempt.Effect(() => Complete(Unit.Value), () => Complete(Application("Не удается сохранить почтового отправителя.")));
        };
    }

    public override void Notify(MailerEvent @event)
    {
        base.Notify(@event);
        _onRegistered.Invoke();
    }

    private async Task HandleRegistration()
    {
        if (!AllValuesExist(this))
        {
            Complete(Application("Параметры для обработки создания почтового отправителя не получены."));
            return;
        }

        _mailer
            .WithId(_id)
            .WithEmail(_email)
            .WithSmtpPassword(_password)
            .WithSendLimit(_sendLimit)
            .WithSendAtThisMoment(_sendAtThisMoment);

        if (!await _mailer.IsEmailUnique(_ct))
        {
            Complete(Conflict($"Почтовый отправитель с почтой: {_email.Value} уже существует."));
            return;
        }

        await _mailer.Save(_ct);
        Complete(Unit.Value);
    }
}