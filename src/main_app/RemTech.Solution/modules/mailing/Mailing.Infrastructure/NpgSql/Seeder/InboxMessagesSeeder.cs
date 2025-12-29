using Mailing.Core.Common;
using Mailing.Core.Inbox;

namespace Mailing.Infrastructure.NpgSql.Seeder;

public sealed class InboxMessagesSeeder(NpgSqlConnectionFactory factory)
{
    private readonly Lazy<string[]> _emailTemplates = new(() => 
    [
        "user1@example.com",
        "user2@test.org",
        "admin@domain.net",
        "support@service.io",
        "info@company.co"
    ]);

    private readonly Lazy<string[]> _subjectTemplates = new(() => 
    [
        "Welcome to Our Service!",
        "Your Account Verification",
        "Monthly Newsletter",
        "Security Alert",
        "Special Offer Inside!"
    ]);

    private readonly Lazy<string[]> _messageBodyTemplates = new(() => 
    [
        "Thank you for signing up! We're excited to have you with us.",
        "Please verify your email address by clicking the link below.",
        "Here's what's new this month: exciting features and updates!",
        "We detected a new login from an unknown device. Was it you?",
        "Exclusive deal just for you: 20% off your next purchase!"
    ]);
    
    private int _seedAmount = 10;

    public InboxMessagesSeeder ChangeSeedAmount(int seedAmount)
    {
        if (seedAmount <= 0)
            throw new ArgumentException(
                $"{nameof(InboxMessagesSeeder)} {nameof(ChangeSeedAmount)} expects value greater than 0.");
        
        _seedAmount = seedAmount;
        return this;
    }

    public async Task ExecuteSeeding(CancellationToken ct = default)
    {
        await using var session = new NpgSqlSession(factory);
        
        InboxMessage[] messages = GenerateInboxMessages();
        const string sql = """
                           INSERT INTO mailing_module.inbox_messages
                           (id, recipient_email, subject, body)
                           VALUES
                           (@id, @recipient_email, @subject, @body)
                           """;

        var parameters = messages.Select(m =>
            new
            {
                id = m.Id,
                recipient_email = m.TargetEmail.Value,
                subject = m.Subject.Value,
                body = m.Body.Value
            });
        
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
    }

    private InboxMessage[] GenerateInboxMessages()
    {
        Random emailRng = new();
        Random subjectRng = new();
        Random bodyRng = new();
        InboxMessage[] messages = new InboxMessage[_seedAmount];
        
        for (int i = 0; i < _seedAmount; i++)
        {
            string email = RandomArrayValue(emailRng, _emailTemplates.Value);
            string subject = RandomArrayValue(subjectRng, _subjectTemplates.Value);
            string body = RandomArrayValue(bodyRng, _messageBodyTemplates.Value);
            InboxMessage message = new(Guid.NewGuid(), new Email(email), new MessageSubject(subject), new MessageBody(body));
            messages[i] = message;
        }
        
        return  messages;
    }

    private U RandomArrayValue<U>(Random random, U[] array)
    {
        int randomIndex = random.Next(0, array.Length);
        return array[randomIndex];
    }
}