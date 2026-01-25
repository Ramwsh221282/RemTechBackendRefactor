namespace RemTech.SharedKernel.Configurations;

public sealed class FrontendOptions
{
	public string Url { get; set; } = string.Empty;

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Url))
			throw new ArgumentNullException($"Frontend URL option is empty. {nameof(FrontendOptions)}");
	}
}
