using Identity.Domain.Accounts.Features.Authenticate;
using Identity.WebApi.Options;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

/// <summary>
/// Тесты аутентификации.
/// </summary>
/// <param name="factory">Фабрика для создания интеграционных тестов.</param>
public sealed class AuthenticationTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Authenticate_Success()
	{
		await Task.Delay(TimeSpan.FromSeconds(10));
		SuperUserCredentialsOptions credentials = Services.GetSuperUserCredentials();
		Result<AuthenticationResult> result = await Services.AuthenticateByEmail(
			credentials.Email,
			credentials.Password
		);
		Assert.True(result.IsSuccess);
	}

	[Fact]
	private async Task VerifyToken_Success()
	{
		await Task.Delay(TimeSpan.FromSeconds(10));
		SuperUserCredentialsOptions credentials = Services.GetSuperUserCredentials();
		Result<AuthenticationResult> result = await Services.AuthenticateByEmail(
			credentials.Email,
			credentials.Password
		);
		Assert.True(result.IsSuccess);
		string token = result.Value.AccessToken;
		Result<Unit> verification = await Services.VerifyToken(token);
		Assert.True(verification.IsSuccess);
	}
}
