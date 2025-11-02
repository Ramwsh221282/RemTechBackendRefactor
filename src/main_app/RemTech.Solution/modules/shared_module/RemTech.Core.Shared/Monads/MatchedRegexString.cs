using System.Text.RegularExpressions;
using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Monads;

public sealed class MatchedRegexString
{
    private readonly string _value;
    private readonly Regex _regex;

    public MatchedRegexString(string value, Regex regex)
    {
        _value = value;
        _regex = regex;
    }

    public Status<string> Matched(Error onFailure)
    {
        if (!_regex.IsMatch(_value))
            return Status<string>.Failure(onFailure);
        return _value;
    }
}