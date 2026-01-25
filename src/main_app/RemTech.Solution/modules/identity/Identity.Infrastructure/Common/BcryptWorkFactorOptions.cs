namespace Identity.Infrastructure.Common;

public sealed class BcryptWorkFactorOptions
{
	public int WorkFactor { get; set; }

	public void Validate()
	{
		if (WorkFactor < 4 || WorkFactor > 31)
			throw new InvalidOperationException("Work factor must be between 4 and 31.");
	}
}
