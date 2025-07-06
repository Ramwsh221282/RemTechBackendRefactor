namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities;

public sealed class ParserLinkActivity
{
    private readonly bool _active;

    public ParserLinkActivity(bool active)
    {
        _active = active;
    }

    public ParserLinkActivity()
    {
        _active = false;
    }

    public bool Read() => _active;

    public ParserLinkActivity Inactive() => new();

    public ParserLinkActivity Active() => new(true);

    public static implicit operator bool(ParserLinkActivity link) => link._active;
}
