using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Infrastructure.AesEncryption;

public static class AesCryptographyExtensions
{
    extension(IServiceCollection services)
    {
        public void AddAesCryptography()
        {
            if (!services.EnsureOptionsExists())
                throw new InvalidOperationException("Cannot add Aes Cryptography. AesEncryptionOptions was not registered. Please register it using services.Configure<AesEncryptionOptions>(...) or services.AddOptions<AesEncryptionOptions>().BindConfiguration(...) before calling AddAesCryptography().");
            services.AddSingleton<AesCryptography>();
        }

        private bool EnsureOptionsExists()
        {
            return services.Any(s => s.ServiceType == typeof(IOptions<AesEncryptionOptions>) ||
                                     s.ServiceType == typeof(IConfigureOptions<AesEncryptionOptions>));
        }
    }
}