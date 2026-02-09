using Microsoft.Extensions.DependencyInjection;
using ParsingSDK.Parsing;

namespace ParsingSDK;

public static class ParsingDependencyInjection
{
    extension(IServiceCollection services)
    {    
        public void RegisterParserDependencies(bool isDevelopment)
        {                            
            services.RegisterBrowserOptionsByEnvironment(isDevelopment);
            services.AddSingleton<BrowserManager>();                                                   
        }        

        private void RegisterBrowserOptionsByEnvironment(bool isDevelopment)
        {            
            if (isDevelopment)
            {
                services.RegisterConfig();
                return;
            }
            
            RegisterFromEnv();
            services.RegisterConfig();
        }

        private void RegisterConfig()
        {
            services.AddOptions<ScrapingBrowserOptions>().BindConfiguration(nameof(ScrapingBrowserOptions));
        }        
    }

    private static void RegisterFromEnv()
    {
        DotNetEnv.Env.Load();
    }
}