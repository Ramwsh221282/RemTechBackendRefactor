using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using Mailing.Presenters.Inbox.CreateInboxMessage;
using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.UpdateMailer;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Mailing.Features;

public sealed class MailingModuleFeaturesFacade(IServiceProvider sp)
{
    private readonly CreateInboxMessageFeature _createInboxMessageFeature = new(sp);
    private readonly AddMailerFeature _addMailerFeature = new(sp);
    private readonly UpdateMailerFeature _updateMailerFeature = new(sp);
    private readonly EnsureHasInboxMessageProcessed _ensureHasInboxMessageProcessed = new(sp);

    public async Task<bool> EnsureHasInboxMessageProcessed()
    {
        return await _ensureHasInboxMessageProcessed.Invoke();
    }
    
    public async Task<Result<CreateInboxMessageResponse>> CreateInboxMessage(
        string targetEmail,
        string subject,
        string body
    )
    {
        return await _createInboxMessageFeature.Invoke(targetEmail, subject, body);
    }

    public async Task<Result<AddMailerResponse>> AddMailer(
        string email,
        string password
    )
    {
        return await _addMailerFeature.Invoke(email, password);
    }

    public async Task<Result<UpdateMailerResponse>> UpdateMailer(
        Guid mailerId,
        string nextEmail,
        string nextPassword
    )
    {
        return await  _updateMailerFeature.Invoke(mailerId, nextEmail, nextPassword);
    }

    public async Task<bool> MailerEmailEquals(
        Guid mailerId,
        string email
        )
    {
        Mailer? mailer = await TryGetMailer(mailerId);
        if (mailer is null) throw new InvalidOperationException("Mailer does not exists");
        return mailer.Domain.Email.Value == email;
    }
    
    public async Task<bool> MailerPasswordEquals(
        Guid mailerId,
        string password)
    {
        Mailer? mailer = await TryGetMailer(mailerId);
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        DecryptMailerSmtpPasswordProtocol crypto = scope.Resolve<DecryptMailerSmtpPasswordProtocol>();
        if (mailer is null) throw new InvalidOperationException("Mailer does not exists");
        Mailer decrypted = await crypto.WithDecryptedPassword(mailer, ct);
        return decrypted.Config.SmtpPassword == password;
    }

    private async Task<Mailer?> TryGetMailer(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        GetMailerProtocol getMailer = scope.Resolve<GetMailerProtocol>();
        Mailer? mailer = await getMailer.Get(new GetMailerQueryArgs(Id: id), ct);
        return mailer;
    }
}