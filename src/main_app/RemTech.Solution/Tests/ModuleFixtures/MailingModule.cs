using Mailing.Infrastructure.NpgSql.Inbox;
using Mailing.Infrastructure.NpgSql.Seeder;
using Mailing.Presenters;
using Mailing.Presenters.Inbox.CreateInboxMessage;
using Mailing.Presenters.Mailers.AddMailer;
using Mailing.Presenters.Mailers.TestEmailSending;
using Mailing.Presenters.Mailers.UpdateMailer;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ModuleFixtures;

public sealed class MailingModule(IServiceProvider sp)
{
    public async Task<Result<CreateInboxMessageResponse>> CreateInboxMessage(
        string targetEmail,
        string subject,
        string body)
    {
        CancellationToken ct = CancellationToken.None;
        CreateInboxMessageRequest request = new(targetEmail, subject, body, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse> gateway = 
            scope.Resolve<IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse>>();
        return await gateway.Execute(request);
    }

    public async Task<Result<AddMailerResponse>> AddMailer(
        string smtpPassword,
        string email)
    {
        CancellationToken ct = CancellationToken.None;
        AddMailerRequest request = new(smtpPassword, email, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddMailerRequest, AddMailerResponse> gateway =
            scope.Resolve<IGateway<AddMailerRequest, AddMailerResponse>>();
        return await gateway.Execute(request);
    }

    public async Task<Result<TestEmailSendingResponse>> TestEmailSending(string email, Guid mailerId)
    {
        CancellationToken ct = CancellationToken.None;
        TestEmailSendingRequest request = new(email, mailerId, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<TestEmailSendingRequest, TestEmailSendingResponse> gateway =
            scope.Resolve<IGateway<TestEmailSendingRequest, TestEmailSendingResponse>>();
        return await gateway.Execute(request);
    }

    public async Task<bool> HasInboxMessages()
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        NpgSqlHasInboxMessagesProtocol protocol = scope.Resolve<NpgSqlHasInboxMessagesProtocol>();
        return await protocol.Has(ct);
    }
    
    public async Task SeedInboxMessages(int seedAmount = 10)
    {
        CancellationToken ct = CancellationToken.None;
        InboxMessagesSeeder seeder = sp.Resolve<InboxMessagesSeeder>();
        InboxMessagesSeeder withOtherSeedAmount = seeder.ChangeSeedAmount(seedAmount);
        await withOtherSeedAmount.ExecuteSeeding(ct);
    }
    
    public async Task<Result<UpdateMailerResponse>> UpdateMailer(
        Guid mailerId,
        string newPassword,
        string newEmail
        )
    {
        CancellationToken ct = CancellationToken.None;
        UpdateMailerRequest request = new(mailerId, newEmail, newPassword, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UpdateMailerRequest, UpdateMailerResponse> gateway =
            scope.Resolve<IGateway<UpdateMailerRequest, UpdateMailerResponse>>();
        return await gateway.Execute(request);
    }
}