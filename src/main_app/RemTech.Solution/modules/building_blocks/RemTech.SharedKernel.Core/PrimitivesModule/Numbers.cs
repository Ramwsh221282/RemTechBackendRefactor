namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class Numbers
{
	public static bool IsNegative(int number) => number < 0;

	public static bool NotNegative(int number) => !IsNegative(number);
}
