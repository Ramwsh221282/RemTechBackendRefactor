namespace Brands.Module.Features.GetBrand;

internal sealed class UnableToResolveBrandException : Exception
{
    public UnableToResolveBrandException()
        : base("Невозможно разрешить бренд.") { }

    public UnableToResolveBrandException(Exception inner)
        : base("Невозможно разрешить бренд.", inner) { }
}
