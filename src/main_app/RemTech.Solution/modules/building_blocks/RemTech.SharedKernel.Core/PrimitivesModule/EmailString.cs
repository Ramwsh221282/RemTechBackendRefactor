namespace RemTech.SharedKernel.Core.PrimitivesModule;

public sealed record EmailString
{
	private readonly bool _hasValidFormat;

	internal EmailString(string value, bool hasValidFormat)
	{
		Value = value;
		_hasValidFormat = hasValidFormat;
	}

	public string Value { get; }

	public bool IsValid() => _hasValidFormat;
}
