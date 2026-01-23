using System.Text.RegularExpressions;

namespace RemTech.SharedKernel.Core.PrimitivesModule;

public sealed record EmailString
{
	private readonly bool _hasValidFormat;
	public string Value { get; }

	internal EmailString(string value, bool hasValidFormat)
	{
		Value = value;
		_hasValidFormat = hasValidFormat;
	}

	public bool IsValid() => _hasValidFormat;
}

public static partial class EmailStringModule
{
	extension(EmailString)
	{
		public static EmailString Create(string? input)
		{
			if (IsNullOrEmpty(input))
				return new EmailString(string.Empty, false);
			if (!HasValidFormat(input))
				return new EmailString(input!, false);
			return new EmailString(input!, true);
		}

		private static bool MatchesEmailRegex(string input)
		{
			return EmailRegex().IsMatch(input);
		}

		private static bool IsNullOrEmpty(string? input)
		{
			return string.IsNullOrWhiteSpace(input);
		}

		private static bool HasValidFormat(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return false;
			return input.Length <= 256 && MatchesEmailRegex(input);
		}
	}

	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
	private static partial Regex EmailRegex();
}
