using Identity.Core.SubjectsModule.Contracts;

namespace Identity.Persistence.NpgSql.SubjectsModule;

public sealed record NpgSqlSubjectCommands(
    Insert Insert,
    Delete Delete,
    Update Update,
    IsEmailUnique IsEmailUnique,
    IsLoginUnique IsLoginUnique,
    Find FindSingle,
    FindMany FindMany);