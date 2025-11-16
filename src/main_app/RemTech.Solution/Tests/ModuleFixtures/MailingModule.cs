namespace Tests.ModuleFixtures;

public sealed class MailingModule
{
    private readonly IServiceProvider _sp;

    public MailingModule(IServiceProvider sp)
    {
        _sp = sp;
    }
}