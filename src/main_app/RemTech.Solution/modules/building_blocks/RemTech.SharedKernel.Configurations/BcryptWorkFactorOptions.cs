namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Настройки для работы с Bcrypt.
/// </summary>
public sealed class BcryptWorkFactorOptions
{
	/// <summary>
	/// Фактор работы Bcrypt.
	/// </summary>
	public int WorkFactor { get; set; }

	/// <summary>
	/// Валидировать настройки фактора работы Bcrypt.
	/// </summary>
	/// <exception cref="InvalidOperationException">Выбрасывается, если фактор работы находится вне допустимого диапазона.</exception>
	public void Validate()
	{
		if (WorkFactor is < 4 or > 31)
		{
			throw new InvalidOperationException("Work factor must be between 4 and 31.");
		}
	}
}
