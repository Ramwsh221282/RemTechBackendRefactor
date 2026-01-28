using System.Text.RegularExpressions;

namespace RemTech.SharedKernel.Core.PrimitivesModule;

/// <summary>
/// Модуль для работы со строками электронной почты.
/// </summary>
public static partial class EmailStringModule
{
	extension(EmailString)
	{
		/// <summary>
		/// Создает экземпляр <see cref="EmailString"/> из входной строки.
		/// </summary>
		/// <param name="input">Входная строка электронной почты.</param>
		/// <returns>Экземпляр <see cref="EmailString"/>.</returns>
		public static EmailString Create(string? input)
		{
			if (IsNullOrEmpty(input))
			{
				return new EmailString(string.Empty, false);
			}

			return !HasValidFormat(input) ? new EmailString(input, false) : new EmailString(input, true);
		}

		/// <summary>
		/// Проверяет соответствие входной строки регулярному выражению для электронной почты.
		/// </summary>
		/// <param name="input">Входная строка электронной почты.</param>
		/// <returns>True, если строка соответствует регулярному выражению для электронной почты; иначе false.</returns>
		private static bool MatchesEmailRegex(string input) => EmailRegex().IsMatch(input);

		/// <summary>
		/// Проверяет, является ли входная строка пустой или содержит только пробелы.
		/// </summary>
		/// <param name="input">Входная строка электронной почты.</param>
		/// <returns>True, если строка пустая или содержит только пробелы; иначе false.</returns>
		private static bool IsNullOrEmpty(string? input) => string.IsNullOrWhiteSpace(input);

		/// <summary>
		/// Проверяет, имеет ли входная строка допустимый формат электронной почты.
		/// </summary>
		/// <param name="input">Входная строка электронной почты.</param>
		/// <returns>True, если строка имеет допустимый формат электронной почты; иначе false.</returns>
		private static bool HasValidFormat(string? input) =>
			!string.IsNullOrWhiteSpace(input) && input.Length <= 256 && MatchesEmailRegex(input);
	}

	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
	private static partial Regex EmailRegex();
}
