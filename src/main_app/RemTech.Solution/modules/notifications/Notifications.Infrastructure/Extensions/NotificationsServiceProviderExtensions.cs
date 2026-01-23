using Microsoft.Extensions.DependencyInjection;
using Notifications.Core.PendingEmails.Features.AddPendingEmail;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Extensions;

public static class NotificationsServiceProviderExtensions
{
	extension(IServiceProvider services)
	{
		public async Task<Result<Unit>> CreatePendingMessage(
			AddPendingEmailCommand command,
			CancellationToken ct = default
		)
		{
			await using AsyncServiceScope scope = services.CreateAsyncScope();
			return await scope
				.ServiceProvider.GetRequiredService<ICommandHandler<AddPendingEmailCommand, Unit>>()
				.Execute(command, ct);
		}
	}
}
