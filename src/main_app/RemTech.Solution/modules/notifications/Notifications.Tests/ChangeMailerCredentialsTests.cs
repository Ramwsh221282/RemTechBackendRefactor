using Notifications.Core.Mailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Tests;

/// <summary>
/// Тесты для изменения учётных данных почтового ящика.
/// </summary>
/// <param name="factory">Фабрика для интеграционных тестов.</param>
public sealed class ChangeMailerCredentialsTests(IntegrationalTestsFactory factory)
	: IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Change_Mailer_Credentials_Success()
	{
		const string email = "testEmail@gmail.com";
		const string password = "testPassword";
		Result<Unit> result1 = await Services.AddMailer(email, password);
		Assert.True(result1.IsSuccess);

		Result<Mailer> mailer = await Services.GetMailerByEmail(email);
		Assert.True(mailer.IsSuccess);

		const string newSmtpPassword = "newSmtpPassword@123";
		const string otherEmail = "newEmail@gmail.com";
		Result<Unit> result2 = await Services.ChangeMailerCredentials(
			mailer.Value.Id.Value,
			otherEmail,
			newSmtpPassword
		);
		Assert.True(result2.IsSuccess);
	}

	[Fact]
	private async Task Change_Mailer_Duplicated_Email_Failure()
	{
		const string email1 = "testEmail@gmail.com";
		const string password1 = "testPassword";
		Result<Unit> result1 = await Services.AddMailer(email1, password1);
		Assert.True(result1.IsSuccess);

		const string email2 = "newEmail@gmail.com";
		const string password2 = "newSmtpPassword@123";
		Result<Unit> result2 = await Services.AddMailer(email2, password2);
		Assert.True(result2.IsSuccess);

		Result<Mailer> mailer = await Services.GetMailerByEmail(email2);
		Assert.True(mailer.IsSuccess);

		const string newSmtpPassword = "newlyNewSmtpPassword@123";
		Result<Unit> result3 = await Services.ChangeMailerCredentials(mailer.Value.Id.Value, email1, newSmtpPassword);
		Assert.True(result3.IsFailure);
	}
}
