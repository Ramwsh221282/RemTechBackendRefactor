using CompositionRoot.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Quartz;
using RemTech.SharedKernel.Infrastructure.AesEncryption;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests;

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

        public void DontUseQuartzServices()
        {
            services.RemoveAll<ICronScheduleJob>();
            services.RemoveAll<IJob>();
        }
        
        public void ReconfigureAesOptions()
        {
            services.RemoveAll<IOptions<AesEncryptionOptions>>();
            services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
        }

        public void ReconfigureRabbitMqOptions(RabbitMqContainer container)
        {
            services.RemoveAll<IOptions<RabbitMqConnectionOptions>>();
            RabbitMqConnectionOptions options = container.CreateRabbitMqConfiguration();
            IOptions<RabbitMqConnectionOptions> ioptions = Options.Create(options);
            services.AddSingleton(ioptions);
        }

        public void ReconfigureQuartzHostedService()
        {
            services.RemoveAll<QuartzHostedService>();
            services.AddQuartzJobs();
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