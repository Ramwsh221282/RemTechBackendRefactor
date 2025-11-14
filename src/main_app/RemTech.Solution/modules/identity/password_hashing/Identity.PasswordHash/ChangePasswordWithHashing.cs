using Identity.Core.SubjectsModule.Contracts;
using RemTech.BuildingBlocks.DependencyInjection;

namespace Identity.PasswordHash;

public static class ChangePasswordWithHashing
{
    internal static ChangePassword ChangePassword(ChangePassword origin, HashPassword hash) =>
        (args) =>
        {
            string password = args.NextPassword;
            string hashed = hash(password);
            ChangePasswordArgs withHashed = args with { NextPassword = hashed };
            return origin(withHashed);
        };

    extension(ChangePassword origin)
    {
        public ChangePassword WithPasswordHashing(IServiceProvider sp)
        {
            HashPassword hash = sp.Resolve<HashPassword>();
            return ChangePassword(origin, hash);
        }
    }
}