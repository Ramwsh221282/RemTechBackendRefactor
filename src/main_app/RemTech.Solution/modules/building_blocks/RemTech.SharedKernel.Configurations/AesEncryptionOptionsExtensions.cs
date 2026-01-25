using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class AesEncryptionOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public void AddAesEncryptionOptionsFromAppsettings(string sectionName = nameof(AesEncryptionOptions)) =>
			services.AddOptions<AesEncryptionOptions>().BindConfiguration(sectionName);
	}
}
