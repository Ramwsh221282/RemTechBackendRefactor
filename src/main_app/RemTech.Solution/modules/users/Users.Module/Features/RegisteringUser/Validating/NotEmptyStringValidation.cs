using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Validating;

internal sealed class NotEmptyStringValidation(string input, Exception onError) : IValidation
{
    public void Check()
    {
        if (string.IsNullOrWhiteSpace(input))
            throw onError;
    }
}
