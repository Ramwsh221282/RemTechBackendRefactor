using Notifications.Core.PendingEmails;

namespace Notifications.Tests;

/// <summary>
/// Тесты для добавления отложенного письма.
/// </summary>
/// <param name="factory">Фабрика для интеграционных тестов.</param>
public sealed class AddPendingEmailTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Add_Pending_Email_Success_Ensure_Created()
	{
		await Task.Delay(TimeSpan.FromSeconds(10));
		Guid accountId = Guid.NewGuid();
		Guid ticketId = Guid.NewGuid();
		const string email = "testEmail@mail.com";
		const string login = "Test user login";
		await Services.PublishOnNewAccountCreated(accountId, ticketId, email, login);
		await Task.Delay(TimeSpan.FromSeconds(10));
		PendingEmailNotification[] pendingEmails = await Services.GetPendingEmailNotifications();
		Assert.NotEmpty(pendingEmails);
		PendingEmailNotification? notification = pendingEmails.FirstOrDefault(e => e.Recipient == email);
		Assert.NotNull(notification);
	}
}
