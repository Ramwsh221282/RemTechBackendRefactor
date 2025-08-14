using Microsoft.AspNetCore.Http;

namespace Cleaners.Module.Endpoints.Responses;

internal sealed class ResourceDisposingHttpResult(IResult result) : IResult
{
    private readonly List<IDisposable> _disposables = [];

    public ResourceDisposingHttpResult With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await result.ExecuteAsync(httpContext);
        foreach (var disposable in _disposables)
            disposable.Dispose();
    }
}
