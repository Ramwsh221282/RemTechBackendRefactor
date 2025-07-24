namespace RemTech.Core.Shared.Decorating;

public static class DecoratingExtensions
{
    public static TDecorated WrapBy<TDecoratable, TDecorated>(
        this TDecoratable decoratable,
        Func<TDecoratable, TDecorated> decoratorFactory)
        where TDecoratable : class
        where TDecorated : class
    {
        return decoratorFactory(decoratable);
    }
}