using Identity.Core.SubjectsModule.Contracts;

namespace Identity.Persistence.NpgSql.SubjectsModule;

public sealed record NpgSqlSubjectCommands(
    InsertSubject Insert,
    DeleteSubject Delete,
    UpdateSubject Update,
    IsSubjectEmailUnique IsEmailUnique,
    IsSubjectLoginUnique IsLoginUnique,
    FindSubject FindSingle,
    FindSubjects FindMany);