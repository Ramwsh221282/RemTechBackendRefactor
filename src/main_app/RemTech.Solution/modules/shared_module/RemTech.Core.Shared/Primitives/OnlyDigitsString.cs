namespace RemTech.Core.Shared.Primitives;

public sealed class OnlyDigitsString
{
    private readonly string _origin;

    public OnlyDigitsString(string origin)
    {
        _origin = origin;
    }

    public string Read()
    {
        return new string(_origin.Select(c => c).Where(char.IsDigit).ToArray());
    }
    
    public static implicit operator string(OnlyDigitsString onlyDigits) => onlyDigits.Read();
}