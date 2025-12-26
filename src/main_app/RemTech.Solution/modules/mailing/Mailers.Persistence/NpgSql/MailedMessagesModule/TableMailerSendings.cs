namespace Mailers.Persistence.NpgSql.MailedMessagesModule;

// CREATE TABLE IF NOT EXISTS mailers_module.mailer_sendings
// (
//     id UUID PRIMARY KEY,
//     mailer_id UUID NOT NULL,     
//     subject VARCHAR(256) NOT NULL,
//     body VARCHAR(512),
//     sent_to VARCHAR(256) NOT NULL,
//     sent_on DATE NOT NULL,
//     FOREIGN KEY (mailer_id) REFERENCES mailers_module.mailers(id) ON DELETE CASCADE
// );
public sealed record TableMailerSendings(Guid Id, Guid MailerId, string Subject, string Body, string SentTo, DateTime SentOn);