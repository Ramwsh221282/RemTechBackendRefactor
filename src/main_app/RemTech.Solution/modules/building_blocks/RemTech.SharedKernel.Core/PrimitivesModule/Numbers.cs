namespace RemTech.SharedKernel.Core.PrimitivesModule;

/// <summary>
/// Утилиты для работы с числами.
/// </summary>
public static class Numbers
{
	/// <summary>
	/// Проверяет, является ли число отрицательным.
	/// </summary>
	/// <param name="number">Число для проверки.</param>
	/// <returns>True, если число отрицательное; иначе false.</returns>
	public static bool IsNegative(int number)
	{
		return number < 0;
	}

	/// <summary>
	/// Проверяет, что число не является отрицательным.
	/// </summary>
	/// <param name="number">Число для проверки.</param>
	/// <returns>True, если число не отрицательное; иначе false.</returns>
	public static bool NotNegative(int number)
	{
		return !IsNegative(number);
	}
}
