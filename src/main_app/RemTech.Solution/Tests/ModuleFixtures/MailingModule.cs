namespace Tests.ModuleFixtures;

public sealed class MailingModule
{
    private readonly Lazy<IServiceProvider> _lazyProvider;
    private IServiceProvider Sp => _lazyProvider.Value;

    public MailingModule(Lazy<IServiceProvider> lazyProvider)
    {
        _lazyProvider = lazyProvider;
    }
}