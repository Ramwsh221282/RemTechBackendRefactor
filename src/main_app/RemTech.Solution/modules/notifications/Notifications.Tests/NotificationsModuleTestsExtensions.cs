using Microsoft.Extensions.DependencyInjection;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using Notifications.Core.Mailers.Features.AddMailer;
using Notifications.Core.Mailers.Features.ChangeCredentials;
using Notifications.Core.PendingEmails;
using Notifications.Core.PendingEmails.Contracts;
using Notifications.Tests.Fakes;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Tests;

public static class NotificationsModuleTestsExtensions
{
	extension(IServiceProvider sp)
	{
		public Task PublishOnNewAccountCreated(Guid accountId, Guid ticketId, string email, string login)
		{
			FakeAccountRegisteredPublisher publisher = sp.GetRequiredService<FakeAccountRegisteredPublisher>();
			return publisher.Publish(accountId, ticketId, email, login);
		}

		public async Task<PendingEmailNotification[]> GetPendingEmailNotifications()
		{
			await using AsyncServiceScope scope = sp.CreateAsyncScope();
			IPendingEmailNotificationsRepository repository =
				scope.ServiceProvider.GetRequiredService<IPendingEmailNotificationsRepository>();
			PendingEmailNotificationsSpecification specification =
				new PendingEmailNotificationsSpecification().OfNotSentOnly();
			return await repository.GetMany(specification);
		}

		public async Task<Result<Unit>> AddMailer(string email, string smtpPassword)
		{
			await using AsyncServiceScope scope = sp.CreateAsyncScope();
			AddMailerCommand command = new(smtpPassword, email);
			Result<Mailer> result = await scope
				.ServiceProvider.GetRequiredService<ICommandHandler<AddMailerCommand, Mailer>>()
				.Execute(command);
			return result.IsFailure ? result.Error : Unit.Value;
		}

		public async Task<Result<Mailer>> GetMailerByEmail(string email)
		{
			await using AsyncServiceScope scope = sp.CreateAsyncScope();
			IMailersRepository repository = scope.ServiceProvider.GetRequiredService<IMailersRepository>();
			MailersSpecification specification = new MailersSpecification().WithEmail(email);
			Result<Mailer> result = await repository.Get(specification);
			return result;
		}

		public async Task<Result<Unit>> ChangeMailerCredentials(Guid id, string email, string smtpPassword)
		{
			await using AsyncServiceScope scope = sp.CreateAsyncScope();
			ChangeCredentialsCommand command = new(id, smtpPassword, email);
			Result<Mailer> result = await scope
				.ServiceProvider.GetRequiredService<ICommandHandler<ChangeCredentialsCommand, Mailer>>()
				.Execute(command);
			return result.IsFailure ? result.Error : Unit.Value;
		}
	}
}
