using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Models;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Identity.Infrastructure.Accounts.Commands.GivePermissions;

public sealed class GivePermissionsCacheInvalidator(HybridCache cache, Serilog.ILogger logger)
	: ICacheInvalidator<GivePermissionsCommand, Account>
{
	private HybridCache Cache { get; } = cache;
	private Serilog.ILogger Logger { get; } = logger;

	public async Task InvalidateCache(GivePermissionsCommand command, Account result, CancellationToken ct = default)
	{
		string key = $"get_user_{result.Id.Value}";
		await Cache.RemoveAsync(key, ct);
		Logger.Information("Invalidated cache for key {Key}", key);
	}
}
