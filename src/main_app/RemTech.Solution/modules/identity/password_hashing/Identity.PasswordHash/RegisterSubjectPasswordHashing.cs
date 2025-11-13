using Identity.Core.SubjectsModule.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.PasswordHash;

public static class RegisterSubjectPasswordHashing
{
    public static HashPassword HashPassword(IOptions<IdentityPasswordHashOptions> hashOptions) =>
        input =>
        {
            int workFactor = hashOptions.Value.WorkFactor;
            return workFactor <= 0 
                ? throw new InvalidOperationException($"{nameof(IdentityPasswordHashOptions)} has invalid work factor.") 
                : BCrypt.Net.BCrypt.HashPassword(input, workFactor);
        };
    
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithPasswordHashing(HashPassword hash) =>
            args =>
            {
                string notHashed = args.Password;
                string hashed = hash(notHashed);
                RegisterSubjectUseCaseArgs withHashedPassword = args with { Password = hashed };
                return origin(withHashedPassword);
            };
    }

    extension(IServiceCollection services)
    {
        public void AddIdentityPasswordHashing()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            services.AddSingleton(configuration);
            services.AddOptions<IdentityPasswordHashOptions>().BindConfiguration(nameof(IdentityPasswordHashOptions));
            services.AddTransient<HashPassword>(sp => HashPassword(sp.GetRequiredService<IOptions<IdentityPasswordHashOptions>>()));
        }
    }
}