namespace Mailing.Domain.Postmans;

public static class DecoratingExtensions
{
    public static T With<T>(this T target, Func<T, T> decoratorFn) where T : class =>
        decoratorFn(target);

    public static TInterface With<TImplementation, TInterface>(this TImplementation target,
        Func<TImplementation, TInterface> decoratorFn) where TImplementation : class where TInterface : notnull =>
        decoratorFn(target);
}