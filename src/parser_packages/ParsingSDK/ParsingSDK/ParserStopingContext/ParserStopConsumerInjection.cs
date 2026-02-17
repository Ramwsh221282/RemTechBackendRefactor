using Microsoft.Extensions.DependencyInjection;

namespace ParsingSDK.ParserStopingContext;

public static class ParserStopConsumerInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterParserStoppingContext(bool isDevelopment)
        {
            if (!isDevelopment)
            {
                DotNetEnv.Env.Load();
            }

            services.AddOptions<ParserStopConsumerOptions>().BindConfiguration(nameof(ParserStopConsumerOptions));            
        }
    }
}
