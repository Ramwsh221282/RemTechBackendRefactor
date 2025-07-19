namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Regexes;

public interface ICtxRegex
{
    string ReadName();
    string ReadValue();
    bool Classified();
}

public sealed class CtxMarkRegex : ICtxRegex
{
    private readonly string _input;
    private string _name;
    private string _value;

    public CtxMarkRegex(string input)
    {
        _input = input;
        _name = string.Empty;
        _value = string.Empty;
    }

    public CtxMarkRegex(CtxMarkRegex regex, string name, string value)
    {
        _input = regex._input;
        _name = name;
        _value = value;
    }

    public string ReadName()
    {
        throw new NotImplementedException();
    }

    public string ReadValue()
    {
        throw new NotImplementedException();
    }

    public bool Classified() =>
        !string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_value);

    public CtxMarkRegex Detect()
    {
        throw new NotImplementedException();
    }
}
