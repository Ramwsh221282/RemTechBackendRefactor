using Identity.Core.SubjectsModule.Domain.Permissions;
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
    IsLoginUnique IsLoginUnique,
    Find Find,
    FindMany FindMany,
    InsertPermission InsertPermission)
{
    public async Task<Optional<Subject>> GetById(Guid id, CancellationToken ct, bool withLock = default)
    {
        SubjectQueryArgs args = new(Id: id, WithLock: withLock);
        return await Find(args, ct);
    }
}

public delegate Task<bool> IsEmailUnique(string email, CancellationToken ct);
public delegate Task<bool> IsLoginUnique(string login, CancellationToken ct);
public delegate Task<Result<Unit>> Insert(Subject subject, CancellationToken ct);
public delegate Task<Result<Unit>> Delete(Subject subject, CancellationToken ct);
public delegate Task<Result<Unit>> Update(Subject subject, CancellationToken ct);
public delegate Task<Optional<Subject>> Find(SubjectQueryArgs args, CancellationToken ct);
public delegate Task<IEnumerable<Subject>> FindMany(SubjectQueryArgs args, CancellationToken ct);

public delegate Task<Result<Unit>> InsertPermission(
    Subject subject, 
    SubjectPermission permission, 
    CancellationToken ct);

public delegate Task<Result<Unit>> DeletePermission(
    Subject subject, 
    SubjectPermission permission, 
    CancellationToken ct);