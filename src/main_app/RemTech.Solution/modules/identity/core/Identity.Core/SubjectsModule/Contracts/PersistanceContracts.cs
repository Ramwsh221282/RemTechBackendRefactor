using Identity.Core.SubjectsModule.Domain.Subjects;

namespace Identity.Core.SubjectsModule.Contracts;

public sealed record SubjectQueryArgs(
    Guid? Id = null, 
    string? Login = null, 
    string? Email = null, 
    bool WithLock = false);

public sealed record SubjectsStorage(
    Insert Insert, 
    Delete Delete, 
    Update Update,
    IsEmailUnique IsEmailUnique,
    IsLoginUnique IsLoginUnique);

public delegate Task<bool> IsEmailUnique(string email, CancellationToken ct);
public delegate Task<bool> IsLoginUnique(string login, CancellationToken ct);
public delegate Task<Result<Unit>> Insert(Subject Subject, CancellationToken ct);
public delegate Task<Result<Unit>> Delete(Subject Subject, CancellationToken ct);
public delegate Task<Result<Unit>> Update(Subject Subject, CancellationToken ct);
public delegate Task<Optional<Subject>> Find(SubjectQueryArgs args, CancellationToken ct);
public delegate Task<IEnumerable<Subject>> FindMany(SubjectQueryArgs args, CancellationToken ct);