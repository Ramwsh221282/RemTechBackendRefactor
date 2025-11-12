using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Contracts;

public sealed record SubjectQueryArgs(Guid? Id = null, string? Login = null, string? Email = null, bool WithLock = false);

public delegate Task<bool> IsSubjectEmailUnique(string email, CancellationToken ct);
public delegate Task<bool> IsSubjectLoginUnique(string login, CancellationToken ct);
public delegate Task<Result<Unit>> InsertSubject(Subject Subject, CancellationToken ct);
public delegate Task<Result<Unit>> DeleteSubject(Subject Subject, CancellationToken ct);
public delegate Task<Result<Unit>> UpdateSubject(Subject Subject, CancellationToken ct);
public delegate Task<Optional<Subject>> FindSubject(SubjectQueryArgs args, CancellationToken ct);
public delegate Task<IEnumerable<Subject>> FindSubjects(SubjectQueryArgs args, CancellationToken ct);