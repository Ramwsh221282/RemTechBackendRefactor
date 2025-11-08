namespace Mailers.Persistence.NpgSql.EventHandlers;

public sealed class NpgMailerDeletionEventTarget : CallbackableMailerEventTarget
{
    private readonly NpgSqlMailer _mailer;
    private readonly CancellationToken _ct;
    private readonly Action _onMailerDeleted;

    public NpgMailerDeletionEventTarget(NpgSqlMailer mailer, CancellationToken ct)
    {
        _mailer = mailer;
        _ct = ct;
        _onMailerDeleted = async () =>
        {
            var attempt = AsyncTry.For(() => HandleMailerDeletion());
            await attempt.Attempt();
            attempt.Effect(() => Complete(Unit.Value), () => Complete(Application("Не удается удалить Mailer.")));
        };
    }

    public override void Notify(MailerEvent @event)
    {
        base.Notify(@event);
        _onMailerDeleted.Invoke();
    }

    private async Task HandleMailerDeletion()
    {
        if (!_id.HasValue)
        {
            Complete(Application("Для удаления почтового отправителя нужно предоставить ID."));
            return;
        }
        
        _mailer.WithId(_id);
        await _mailer.Delete(_ct);
        Complete(Unit.Value);
    }
}