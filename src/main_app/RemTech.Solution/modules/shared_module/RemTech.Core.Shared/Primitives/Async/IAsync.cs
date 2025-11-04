using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives.Async;

public interface IAsync<in Y, X> where X : class where Y : class
{
    Task<Status<X>> ExecuteAsync(Y component);
}