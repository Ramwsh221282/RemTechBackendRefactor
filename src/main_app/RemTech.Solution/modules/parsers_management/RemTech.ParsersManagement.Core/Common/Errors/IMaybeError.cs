using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Errors;

public interface IMaybeError
{
    bool Errored();
    Error Error();
}
