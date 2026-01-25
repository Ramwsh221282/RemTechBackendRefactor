using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Persistence;
using Identity.WebApi.Options;
using Microsoft.Extensions.Options;

namespace Identity.WebApi.BackgroundServices;

public sealed class SuperUserAccountRegistrationOnStartupBackgroundService(
	IServiceProvider services,
	Serilog.ILogger logger
) : BackgroundService
{
	private IServiceProvider Services { get; } = services;
	private Serilog.ILogger Logger { get; } =
		logger.ForContext<SuperUserAccountRegistrationOnStartupBackgroundService>();

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!await Executed(stoppingToken)) { }
	}

	private static (
		SuperUserCredentialsOptions Options,
		IAccountsRepository Repository,
		IPasswordHasher Cryptography
	) GetDependencies(AsyncServiceScope scope) =>
		(
			scope.ServiceProvider.GetRequiredService<IOptions<SuperUserCredentialsOptions>>().Value,
			scope.ServiceProvider.GetRequiredService<IAccountsRepository>(),
			scope.ServiceProvider.GetRequiredService<IPasswordHasher>()
		);

	private static Task AddAccount(
		SuperUserCredentialsOptions options,
		IAccountsRepository repository,
		IPasswordHasher hasher,
		CancellationToken ct
	)
	{
		Account account = Account.Create(
			email: AccountEmail.Create(options.Email),
			login: AccountLogin.Create(options.Login),
			password: AccountPassword.Create(options.Password).Value.HashBy(hasher),
			status: AccountActivationStatus.Activated()
		);
		return repository.Add(account, ct);
	}

	private static Task<bool> AccountExists(
		SuperUserCredentialsOptions options,
		IAccountsRepository repository,
		CancellationToken ct
	)
	{
		AccountSpecification specification = new AccountSpecification().WithEmail(options.Email);
		return repository.Exists(specification, ct: ct);
	}

	private async Task<bool> Executed(CancellationToken ct)
	{
		try
		{
			await using AsyncServiceScope scope = Services.CreateAsyncScope();
			(SuperUserCredentialsOptions options, IAccountsRepository repository, IPasswordHasher cryptography) =
				GetDependencies(scope);

			options.Validate();
			if (await AccountExists(options, repository, ct))
			{
				Logger.Warning("Account already exists {Name} {Email}", options.Login, options.Email);
				return true;
			}

			await AddAccount(options, repository, cryptography, ct);
			return true;
		}
		catch (Exception e)
		{
			Logger.Fatal(e, "Error creating super user account");
			return false;
		}
	}
}
