using Microsoft.Extensions.DependencyInjection;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;

namespace ParsingSDK;

public static class ParsingDependencyInjection
{
    extension(IServiceCollection services)
    {    
        public void RegisterParserDependencies(bool isDevelopment)
        {                            
            services.RegisterBrowserOptionsByEnvironment(isDevelopment);
            services.AddSingleton<BrowserManagerProvider>();                                                   
        }        

        private void RegisterBrowserOptionsByEnvironment(bool isDevelopment)
        {            
            if (!isDevelopment)
            {
                RegisterFromEnv();                                
            }
                        
            services.RegisterConfig();
            services.RegisterParserStoppingContext(isDevelopment);
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