namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class Numbers
{
	public static bool IsNegative(int number)
	{
		return number < 0;
	}

	public static bool NotNegative(int number)
	{
		return !IsNegative(number);
	}
}
