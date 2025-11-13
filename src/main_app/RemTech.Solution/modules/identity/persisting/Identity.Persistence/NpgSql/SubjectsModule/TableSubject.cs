namespace Identity.Persistence.NpgSql.SubjectsModule;

internal sealed record TableSubject(
    Guid SId,
    string SLogin,
    string SEmail,
    string SPassword,
    DateTime? SActivationDate,
    string SPermissionsJsonArray);