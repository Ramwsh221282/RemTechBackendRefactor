namespace Scrapers.Module.Features.ChangeLinkActivity.Exceptions;

internal sealed class LinkActivitySameException : Exception
{
    public LinkActivitySameException(string linkName)
        : base($"У ссылки уже такая активность.") { }

    public LinkActivitySameException(string linkName, Exception inner)
        : base("У ссылки уже такая активность", inner) { }
}
