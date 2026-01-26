using System.Text.RegularExpressions;

namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static partial class EmailStringModule
{
	extension(EmailString)
	{
		public static EmailString Create(string? input)
		{
			if (IsNullOrEmpty(input))
				return new EmailString(string.Empty, false);
			return !HasValidFormat(input) ? new EmailString(input!, false) : new EmailString(input!, true);
		}

		private static bool MatchesEmailRegex(string input) => EmailRegex().IsMatch(input);

		private static bool IsNullOrEmpty(string? input) => string.IsNullOrWhiteSpace(input);

		private static bool HasValidFormat(string? input)
		{
			return string.IsNullOrWhiteSpace(input) ? false : input.Length <= 256 && MatchesEmailRegex(input);
		}
	}

	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
	private static partial Regex EmailRegex();
}
