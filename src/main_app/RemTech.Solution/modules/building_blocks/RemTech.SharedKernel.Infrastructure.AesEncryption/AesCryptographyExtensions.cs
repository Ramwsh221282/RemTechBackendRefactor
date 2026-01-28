using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RemTech.SharedKernel.Infrastructure.AesEncryption;

/// <summary>
/// Расширения для регистрации AesCryptography в контейнере служб.
/// </summary>
public static class AesCryptographyExtensions
{
	extension(IServiceCollection services)
	{
		public void AddAesCryptography() => services.TryAddSingleton<AesCryptography>();
	}
}
