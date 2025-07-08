using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public interface IMaybeError
{
    bool Errored();
    Error Error();
}
