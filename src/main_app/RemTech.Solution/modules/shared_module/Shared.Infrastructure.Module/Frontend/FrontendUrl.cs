namespace Shared.Infrastructure.Module.Frontend;

public sealed class FrontendUrl(string url)
{
    public string Read() => url;
}
