using Identity.Domain.Users.Aggregate.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Sessions;

public static class UserClaimsDictionaryExtensions
{
    public static Status<UserId> ExtractId(this UserSessionClaimsDictionary dictionary)
    {
        string? id = dictionary.GetPropertyInfo("Subject")?.Replace("\"", string.Empty);
        return !Guid.TryParse(id, out var subjectId)
            ? Error.Conflict("Некорректный токен.")
            : UserId.Create(subjectId);
    }
}
