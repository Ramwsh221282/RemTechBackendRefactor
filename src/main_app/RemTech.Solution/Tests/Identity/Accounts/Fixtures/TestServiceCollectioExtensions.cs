using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.AesEncryption;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Tests.Identity.Accounts.Fixtures;

public static class TestServiceCollectioExtensions
{
    extension(IServiceCollection services)
    {
        public void ReconfigureConfigurationProvider()
        {
            services.RemoveAll<IConfiguration>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            services.AddSingleton(configuration);
        }

        public void ReconfigureAesOptions()
        {
            services.RemoveAll<IOptions<AesEncryptionOptions>>();
            services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
        }
        
        public void ReconfigurePostgreSqlOptions(PostgreSqlContainer container)
        {
            services.RemoveAll<IOptions<NpgSqlOptions>>();
            NpgSqlOptions options = container.CreateDatabaseConfiguration();
            IOptions<NpgSqlOptions> ioptions = Options.Create(options);
            services.AddSingleton(ioptions);    
        }
    }
}