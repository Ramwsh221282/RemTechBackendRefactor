namespace Identity.Persistence.NpgSql.SubjectsModule;

internal sealed class TableSubject
{
    public required Guid SId { get; internal init; }
    public required string SLogin { get; internal init; }
    public required string SEmail { get; internal init; }
    public required string SPassword { get; internal init; }
    public required DateTime? SActivationDate { get; internal init; }
    public required string SPermissionsJsonArray { get; internal init; }

    private TableSubject()
    {
        
    }
}