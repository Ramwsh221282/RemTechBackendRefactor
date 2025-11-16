using Identity.Core.SubjectsModule.Contracts;
using Microsoft.Extensions.Options;
using RemTech.BuildingBlocks.DependencyInjection;

namespace Identity.PasswordHash;

public static class RegisterSubjectPasswordHashing
{
    private static HashPassword HashPassword(IOptions<IdentityPasswordHashOptions> hashOptions) =>
        input =>
        {
            int workFactor = hashOptions.Value.WorkFactor;
            return workFactor <= 0 
                ? throw new InvalidOperationException($"{nameof(IdentityPasswordHashOptions)} has invalid work factor.") 
                : BCrypt.Net.BCrypt.HashPassword(input, workFactor);
        };
    
    private static RegisterSubject WithPasswordHashing(RegisterSubject origin, HashPassword hash) =>
        args =>
        {
            string notHashed = args.Password;
            string hashed = hash(notHashed);
            RegisterSubjectUseCaseArgs withHashedPassword = args with { Password = hashed };
            return origin(withHashedPassword);
        };
    
    extension(RegisterSubject origin)
    {
        public RegisterSubject WithPasswordHashing(IServiceProvider sp)
        {
            IOptions<IdentityPasswordHashOptions> options = sp.Resolve<IOptions<IdentityPasswordHashOptions>>();
            HashPassword hash = HashPassword(options);
            return WithPasswordHashing(origin, hash);
        }
    }

    extension(IServiceProvider sp)
    {
        public HashPassword ResolveHashPasswordFunction()
        {
            IOptions<IdentityPasswordHashOptions> options = sp.Resolve<IOptions<IdentityPasswordHashOptions>>();
            return HashPassword(options);
        }
    }
}