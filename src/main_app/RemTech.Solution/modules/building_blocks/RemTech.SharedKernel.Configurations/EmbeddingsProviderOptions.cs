namespace RemTech.SharedKernel.Configurations;

public sealed class EmbeddingsProviderOptions
{
	public string TokenizerPath { get; set; } = string.Empty;
	public string ModelPath { get; set; } = string.Empty;

	public bool Validate()
	{
		if (string.IsNullOrWhiteSpace(TokenizerPath))
			throw new InvalidOperationException("Tokenizer path is empty.");
		return string.IsNullOrWhiteSpace(ModelPath)
			? throw new InvalidOperationException("Model path is empty.")
			: true;
	}
}
