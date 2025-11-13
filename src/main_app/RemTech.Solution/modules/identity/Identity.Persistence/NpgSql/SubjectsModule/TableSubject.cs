namespace Identity.Persistence.NpgSql.SubjectsModule;

internal sealed record TableSubject(
    Guid SId,
    string SLogin,
    string SEmail,
    string SPassword,
    string permissions,
    bool SActivationDate);